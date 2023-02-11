using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Whol.Logic.Tests.Implementations;

/// <summary>
/// IStorage stub
/// </summary>
[ExcludeFromCodeCoverage]
public class TestStorage : IStorage
{
    private readonly object _lock = new object();
    private List<Day> _savedDays = new();
    //private List<Event> _olderEvents = new();
    //private Event[] _lastDayEvents = Array.Empty<Event>();
    private List<Holiday> _holidays = new();
    //private ITime _time;

    //public TestStorage(ITime time)
    //{
    //    _time = time;
    //}

    public bool EventsLoaded { get; set; }
    public bool HolidaysLoaded { get; set; }
    public bool EventsSaved { get; set; }
    public bool HolidaysSaved { get; set; }

    public void Initialize(IEnumerable<Day>? days, IEnumerable<Holiday>? holidays)
    {
        if (days != null)
            _savedDays = days.ToList();
        if (holidays != null)
            _holidays = holidays.ToList();
    }

    //public void SaveEvents(IEnumerable<Event> events)
    //{
    //    _lastDayEvents = events.ToArray();
    //    EventsSaved = true;
    //}

    //public void SaveOlderEvents(IEnumerable<Event> olderEvents)
    //{
    //    foreach (var olderEvent in olderEvents)
    //        if (!_olderEvents.Any(x => x.Time == olderEvent.Time && x.EventType == olderEvent.EventType && x.Task == olderEvent.Task))
    //            _olderEvents.Add(olderEvent);
    //}

    //public void SummarizeEvents(DateTime starTime, DateTime endTime)
    //{
    //    var olderEvents = _lastDayEvents.Where(e => e.Time >= starTime && e.Time <= endTime).ToArray();
    //    if (olderEvents.Any())
    //        _olderEvents.AddRange(olderEvents);
    //}

    public IEnumerable<Day> LoadEvents()
    {
        lock (_lock)
        {
            EventsLoaded = true;
            return _savedDays.Select(d => d.Clone()).ToArray() ?? Array.Empty<Day>();
        }
    }

    public void SaveDay(Day day)
    {
        lock (_lock)
        {
            if (_savedDays.All(d => d.Date != day.Date))
                _savedDays.Add(day);
            EventsSaved = true;
        }
    }

    public void SummarizeEvents(DateTime starTime, DateTime endTime)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Holiday> LoadHolidays()
    {
        HolidaysLoaded = true;
        return _holidays;
    }

    public void SaveHolidays(IEnumerable<Holiday> holidays)
    {
        HolidaysSaved = true;
    }
}