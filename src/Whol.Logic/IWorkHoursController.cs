using System;
using System.Collections.Generic;

namespace Whol.Logic
{
    interface IWorkHoursController
    {
        bool IsWorking { get; }
        void StartWork();
        void StopWork();
        TimeSpan GetTodayWorkTime();
    }
}
