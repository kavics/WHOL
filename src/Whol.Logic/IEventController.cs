using System;
using System.Collections.Generic;

namespace Whol.Logic
{
    public interface IEventController
    {
        bool IsWorking { get; }
        string CurrentTask { get; }
        IEnumerable<string> Tasks { get; }
        void StartWork(string task = null);
        void StopWork();
        TimeSpan GetTodayWorkTime();
    }
}
