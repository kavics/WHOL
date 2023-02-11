using System;
using System.Collections.Generic;

namespace Whol.Logic;

public interface IStorage
{
    IEnumerable<Day> LoadEvents();
    void SaveDay(Day day);
    void SummarizeEvents(DateTime starTime, DateTime endTime);

    IEnumerable<Holiday> LoadHolidays();
    void SaveHolidays(IEnumerable<Holiday> holidays);
}