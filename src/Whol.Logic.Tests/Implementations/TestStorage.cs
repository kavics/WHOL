using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Whol.Logic.Tests.Implementations;

/// <summary>
/// IStorage stub
/// </summary>
[ExcludeFromCodeCoverage]
public class TestStorage : IStorage
{
    private IEnumerable<Event>? _lastDayEvents;
    private IEnumerable<Holiday>? _holidays;

    public bool EventsLoaded { get; set; }
    public bool HolidaysLoaded { get; set; }
    public bool EventsSaved { get; set; }
    public bool HolidaysSaved { get; set; }

    public void Initialize(IEnumerable<Event>? lastDayEvents, IEnumerable<Holiday>? holidays)
    {
        _lastDayEvents = lastDayEvents;
        _holidays = holidays;
    }

    public IEnumerable<Event> LoadEvents()
    {
        EventsLoaded = true;
        return _lastDayEvents ?? Array.Empty<Event>();
    }

    public IEnumerable<Holiday> LoadHolidays()
    {
        HolidaysLoaded = true;
        return _holidays ?? Array.Empty<Holiday>();
    }

    public void SaveEvents(IEnumerable<Event> events)
    {
        _lastDayEvents = events;
        EventsSaved = true;
    }

    public void SaveHolidays(IEnumerable<Holiday> holidays)
    {
        HolidaysSaved = true;
    }
}