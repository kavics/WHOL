using System;
using System.Collections.Generic;
using System.Linq;

namespace Whol.Logic
{
    public class EventController : IEventController
    {
        private List<Event> _events;

        private TimeSpan _todayClosedWorkTime;
        private DateTime _lastStart;

        private readonly ITime _time;
        private readonly IStorage _storage;
// ReSharper disable once NotAccessedField.Local
        private readonly IUserManager _userManager;
// ReSharper disable once NotAccessedField.Local
        private readonly User _user;
        private List<string> _taskList;

        public EventController(ITime time, IStorage storage, IUserManager userManager, User user = null)
        {
            _time = time;
            _storage = storage;
            _userManager = userManager;
            _user = user;
            Initialize();
        }
        private void Initialize()
        {
            var events = _storage.LoadEvents().ToList();

            var lastStart = DateTime.MinValue;
            var closedTime = TimeSpan.Zero;
            Event activeEvent = null;
            var tasks = new List<string>();

            foreach (var @event in events)
            {
                var today = DateTime.Today;

                switch (@event.EventType)
                {
                    case EventType.Start:
                        lastStart = @event.Time;
                        activeEvent = @event;
                        AddTask(tasks, @event.Task);
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

            _events = events;
            _todayClosedWorkTime = closedTime;
            _lastStart = lastStart;
            IsWorking = activeEvent != null;
            CurrentTask = activeEvent?.Task;
            _taskList = tasks;
        }

        private void AddTask(List<string> tasks, string task)
        {
            if (task != null)
            {
                if (tasks.Contains(task))
                    tasks.Remove(task);
                tasks.Add(task);
            }
        }

        public bool IsWorking { get; private set; }
        public string CurrentTask { get; private set; }

        public IEnumerable<string> Tasks => _taskList;

        public void StartWork(string task = null)
        {
            _lastStart = _time.Now;
            IsWorking = true;
            CurrentTask = task;
            AddTask(_taskList, task);
            _events.Add(new Event{Time = _time.Now, EventType = EventType.Start, Task = task});
            _storage.SaveEvents(_events);
        }

        public void StopWork()
        {
            _todayClosedWorkTime += _time.Now - _lastStart;
            IsWorking = false;
            _events.Add(new Event { Time = _time.Now, EventType = EventType.Stop });
            _storage.SaveEvents(_events);
        }

        public TimeSpan GetTodayWorkTime()
        {
            return this.IsWorking
                ? this._todayClosedWorkTime + (this._time.Now - this._lastStart)
                : this._todayClosedWorkTime;
        }

    }
}
