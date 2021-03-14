using System;
using System.Collections.Generic;

namespace Whol.Logic
{
    public interface IStorage
    {
        IEnumerable<WhEvent> LoadEvents();
        IEnumerable<Holiday> LoadHolidays();

        void SaveEvents(IEnumerable<WhEvent> whEvent);
        void SaveHolidays(IEnumerable<Holiday> holidays);
    }
}
