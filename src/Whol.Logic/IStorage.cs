using System;
using System.Collections.Generic;

namespace Whol.Logic;

public interface IStorage
{
    IEnumerable<Event> LoadEvents();
    void SaveEvents(IEnumerable<Event> events);
    void SaveOlderEvents(IEnumerable<Event> olderEvents);
    void SummarizeEvents(DateTime starTime, DateTime endTime);

    IEnumerable<Holiday> LoadHolidays();
    void SaveHolidays(IEnumerable<Holiday> holidays);
}