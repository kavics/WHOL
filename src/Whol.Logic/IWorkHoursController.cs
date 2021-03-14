using System;

namespace Whol.Logic
{
    interface IWorkHoursController
    {
        bool IsWorking { get; }
        bool IsHoliday { get; }
        TimeSpan[] LastDaysWorkTime { get; }
        void StartWork();
        void StopWork();
        TimeSpan GetTodayWorkTime();
        void SetHoliday(DateTime date, bool isHoliday, string description);
    }
}
