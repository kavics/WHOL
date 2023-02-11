using System;
using System.Collections.Generic;
using System.Linq;

namespace Whol.Logic;

public class Day
{
    private readonly object _lock = new object();
    private List<Event> _events { get; set; } = new List<Event>();

    public DateTime Date { get; }

    public Day(DateTime date, IEnumerable<Event>? events = null)
    {
        Date = date;
        if (events != null)
            _events.AddRange(events);
    }

    public Event[] GetEvents() => _events.ToArray();

    public void AddEvent(Event @event)
    {
        lock (_lock)
        {
            if (@event.Time < Date || @event.Time >= Date.AddDays(1.0d))
                throw new ArgumentException($"The given event {@event.Time:yyyy-MM-dd HH:mm:ss.fffff} is out of day {Date:yyyy-MM-dd} ");
            if (_events.Any(e => GetKey(e) == GetKey(@event)))
                return;
            _events.Add(@event);
            if (_events.Count == 1)
                return;
            if (_events[_events.Count - 1].Time < _events[_events.Count - 2].Time)
                _events = _events.OrderBy(e => e.Time).ToList();
        }
    }

    private string GetKey(Event @event) => $"{@event.Time.Ticks},{@event.EventType},{@event.Task ?? string.Empty}";

    public Day Clone()
    {
        lock (_lock)
        {
            var events = _events.Select(e => e.Clone()).ToArray();
            return new Day(Date, events);
        }
    }
}