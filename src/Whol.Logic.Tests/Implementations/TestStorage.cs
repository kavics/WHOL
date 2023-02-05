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
    private List<Event> _olderEvents = new();
    private Event[] _lastDayEvents = Array.Empty<Event>();
    private List<Holiday> _holidays = new();
    private ITime _time;

    public TestStorage(ITime time)
    {
        _time = time;
    }

    public bool EventsLoaded { get; set; }
    public bool HolidaysLoaded { get; set; }
    public bool EventsSaved { get; set; }
    public bool HolidaysSaved { get; set; }

    public void Initialize(IEnumerable<Event>? lastDayEvents, IEnumerable<Holiday>? holidays)
    {
        if(lastDayEvents != null)
            _lastDayEvents = lastDayEvents.ToArray();
        if(holidays != null)
            _holidays = holidays.ToList();
    }

    public IEnumerable<Event> LoadEvents()
    {
        EventsLoaded = true;
        var allEvents = _olderEvents.ToList();
        allEvents.AddRange(_lastDayEvents);
        return allEvents;
    }

    public void SaveEvents(IEnumerable<Event> events)
    {
        _lastDayEvents = events.ToArray();
        EventsSaved = true;
    }

    public void SaveOlderEvents(IEnumerable<Event> olderEvents)
    {
        foreach (var olderEvent in olderEvents)
            if (!_olderEvents.Any(x => x.Time == olderEvent.Time && x.EventType == olderEvent.EventType && x.Task == olderEvent.Task))
                _olderEvents.Add(olderEvent);
    }

    public void SummarizeEvents(DateTime starTime, DateTime endTime)
    {
        var olderEvents = _lastDayEvents.Where(e => e.Time >= starTime && e.Time <= endTime).ToArray();
        if (olderEvents.Any())
            _olderEvents.AddRange(olderEvents);
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