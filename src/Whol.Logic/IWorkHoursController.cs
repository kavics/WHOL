using System;
using System.Collections.Generic;

namespace Whol.Logic
{
    interface IWorkHoursController
    {
        bool IsWorking { get; }
        TimeSpan[] LastDaysWorkTime { get; }
        void StartWork();
        void StopWork();
        TimeSpan GetTodayWorkTime();

        //bool IsHoliday_DELETE { get; }
        //string HolidayDescription_DELETE { get; }
        //void SetHolidays_DELETE(IEnumerable<Holiday> holidays);
    }
}
