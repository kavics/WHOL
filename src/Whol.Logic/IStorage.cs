using System;
using System.Collections.Generic;

namespace Whol.Logic
{
    public interface IStorage
    {
        IEnumerable<WhEvent> LoadLastDayEvents();
        IEnumerable<Holiday> LoadHolidays();

        void SaveEvent(WhEvent whEvent);
        void SaveHolidays(IEnumerable<Holiday> holidays);
    }
}
