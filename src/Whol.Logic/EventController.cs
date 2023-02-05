using System;
using System.Collections.Generic;
using System.Linq;

namespace Whol.Logic;

public class EventController : IEventController
{
    private List<Event>? _events;

    private TimeSpan _todayClosedWorkTime;
    private DateTime _lastStart;

    private readonly ITime _time;
    private readonly IStorage _storage;
    // ReSharper disable once NotAccessedField.Local
    private readonly IUserManager _userManager;
    private List<string>? _taskList;

    public EventController(ITime time, IStorage storage, IUserManager userManager)
    {
        _time = time;
        _storage = storage;
        _userManager = userManager;
        Initialize();
    }
    private void Initialize()
    {
        _events = _storage.LoadEvents().ToList();

        var lastStart = DateTime.MinValue;
        var closedTime = TimeSpan.Zero;
        Event? activeEvent = null;
        _taskList = new List<string>();

        foreach (var @event in _events)
        {
            var today = DateTime.Today;

            switch (@event.EventType)
            {
                case EventType.Start:
                    lastStart = @event.Time;
                    activeEvent = @event;
                    AddTask(@event.Task);
                    break;
                case EventType.Stop:
                    if(@event.Time >= today)
                        closedTime += @event.Time - lastStart;
                    activeEvent = null;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        _todayClosedWorkTime = closedTime;
        _lastStart = lastStart;
        IsWorking = activeEvent != null;
        CurrentTask = activeEvent?.Task;
    }

    private void AddTask(string? task)
    {
        if (task == null)
            return;
        if (_taskList == null)
            _taskList = new List<string>();
        if (_taskList.Contains(task))
            _taskList.Remove(task);
        _taskList.Add(task);
    }

    public bool IsWorking { get; private set; }
    public string? CurrentTask { get; private set; }

    public IEnumerable<string> Tasks => _taskList ?? (IEnumerable<string>)Array.Empty<string>();

    public void StartWork(string? task = null)
    {
        if(IsWorking)
            StopWork();
        if (_lastStart <= _time.Today)
            _todayClosedWorkTime = TimeSpan.Zero;
        _lastStart = _time.Now;
        IsWorking = true;
        CurrentTask = task;
        AddTask(task);
        AddEvent(new Event{Time = _time.Now, EventType = EventType.Start, Task = task});
    }

    public void StopWork()
    {
        var now = _time.Now;

        if (_lastStart < now.Date)
        {
            // 2023-01-02 00:00:00.00000
            var start = new DateTime(now.Year, now.Month, now.Day);
            var end = start.AddMilliseconds(-1.0);
            AddEvent(new Event { Time = end, EventType = EventType.Stop });
            AddEvent(new Event { Time = start, EventType = EventType.Start, Task = CurrentTask });
            _todayClosedWorkTime = now - now.Date;
        }
        else
        {
            _todayClosedWorkTime += now - _lastStart;
        }
        IsWorking = false;
        AddEvent(new Event { Time = now, EventType = EventType.Stop });
    }

    private void AddEvent(Event @event)
    {
        _events ??= new List<Event>();
        _events.Add(@event);
        var today = _time.Today;
        var olderEvents = _events.Where(x => x.Time < today).ToList();
        _events = _events.Where(x => x.Time >= today).ToList();
        if (olderEvents.Any())
        {
            _storage.SaveOlderEvents(olderEvents);
            _storage.SummarizeEvents(olderEvents.First().Time, olderEvents.Last().Time);
        }
        _storage.SaveEvents(_events);
    }

    public TimeSpan GetTodayWorkTime()
    {
        if (!IsWorking)
            return _todayClosedWorkTime;

        var now = _time.Now;
        return _lastStart < now.Date 
            ? now - now.Date 
            : _todayClosedWorkTime + (now - _lastStart);
    }
}