using System;
using System.Collections.Generic;

namespace Whol.Logic
{
    public interface IStorage
    {
        IEnumerable<Event> LoadEvents();
        IEnumerable<Holiday> LoadHolidays();

        void SaveEvents(IEnumerable<Event> events);
        void SaveHolidays(IEnumerable<Holiday> holidays);
    }
}
