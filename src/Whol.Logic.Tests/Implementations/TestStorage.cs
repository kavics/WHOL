using System;
using System.Collections.Generic;
using System.Text;

namespace Whol.Logic.Tests.Implementations
{
    public class TestStorage : IStorage
    {
        public IEnumerable<WhEvent> LoadLastDayEvents()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Holiday> LoadHolidays()
        {
            throw new NotImplementedException();
        }

        public void SaveEvent(WhEvent whEvent)
        {
            throw new NotImplementedException();
        }

        public void SaveHolidays(IEnumerable<Holiday> holidays)
        {
            throw new NotImplementedException();
        }
    }
}
