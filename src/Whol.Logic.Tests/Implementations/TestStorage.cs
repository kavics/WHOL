using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Utilities;

namespace Whol.Logic.Tests.Implementations
{
    /// <summary>
    /// IStorage stub
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class TestStorage : IStorage
    {
        private IEnumerable<WhEvent> _lastDayEvents;
        private IEnumerable<Holiday> _holidays;

        public bool EventsLoaded { get; set; }
        public bool HolidaysLoaded { get; set; }
        public bool EventsSaved { get; set; }
        public bool HolidaysSaved { get; set; }

        public TestStorage(IEnumerable<WhEvent> lastDayEvents, IEnumerable<Holiday> holidays)
        {
            _lastDayEvents = lastDayEvents;
            _holidays = holidays;
        }

        public IEnumerable<WhEvent> LoadEvents()
        {
            EventsLoaded = true;
            return _lastDayEvents ?? new WhEvent[0];
        }

        public IEnumerable<Holiday> LoadHolidays()
        {
            HolidaysLoaded = true;
            return _holidays ?? new Holiday[0];
        }

        public void SaveEvents(IEnumerable<WhEvent> whEvent)
        {
            _lastDayEvents = whEvent;
            EventsSaved = true;
        }

        public void SaveHolidays(IEnumerable<Holiday> holidays)
        {
            HolidaysSaved = true;
        }
    }
}
