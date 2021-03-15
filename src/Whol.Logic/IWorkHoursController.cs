using System;
using System.Collections.Generic;

namespace Whol.Logic
{
    interface IWorkHoursController
    {
        bool IsWorking { get; }
        bool IsHoliday { get; }
        string HolidayDescription { get; }
        TimeSpan[] LastDaysWorkTime { get; }
        void StartWork();
        void StopWork();
        TimeSpan GetTodayWorkTime();
        void SetHolidays(IEnumerable<Holiday> holidays);
    }
}
