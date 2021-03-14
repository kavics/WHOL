using System;
using System.Collections.Generic;
using System.Text;

namespace Whol.Logic.Tests.Implementations
{
    /// <summary>
    /// IStorage stub
    /// </summary>
    public class TestStorage : IStorage
    {
        private IEnumerable<WhEvent> _lastDayEvents;
        private IEnumerable<Holiday> _holidays;

        public TestStorage(IEnumerable<WhEvent> lastDayEvents, IEnumerable<Holiday> holidays)
        {
            _lastDayEvents = lastDayEvents;
            _holidays = holidays;
        }

        public IEnumerable<WhEvent> LoadEvents()
        {
            return _lastDayEvents ?? new WhEvent[0];
        }

        public IEnumerable<Holiday> LoadHolidays()
        {
            return _holidays ?? new Holiday[0];
        }

        public void SaveEvents(IEnumerable<WhEvent> whEvent)
        {
            throw new NotImplementedException();
        }

        public void SaveHolidays(IEnumerable<Holiday> holidays)
        {
            throw new NotImplementedException();
        }
    }
}
