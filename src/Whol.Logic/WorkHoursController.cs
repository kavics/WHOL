using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Whol.Logic
{
    public class WorkHoursController : IWorkHoursController
    {
        private TimeSpan _todayClosedWorkTime;
        private DateTime _lastStart;

        private readonly ITime _time;
        private readonly IStorage _storage;
        private readonly IUserManager _userManager;
        private readonly User _user;

        public WorkHoursController(ITime time, IStorage storage, IUserManager userManager, User user)
        {
            _time = time;
            _storage = storage;
            _userManager = userManager;
            _user = user;
            var loadingResult = LoadTodayClosedWorkTime(storage);
            _todayClosedWorkTime = loadingResult.closedTime;
            _lastStart = loadingResult.lastStart;
            IsWorking = loadingResult.isWorking;
        }

        private (TimeSpan closedTime, DateTime lastStart, bool isWorking) LoadTodayClosedWorkTime(IStorage storage)
        {
            var lastStart = DateTime.MinValue;
            var closedWorkTime = TimeSpan.Zero;
            var isWorking = false;

            foreach (var whEvent in storage.LoadEvents())
            {
                if (whEvent.Time < DateTime.Today)
                    continue;

                switch (whEvent.EventType)
                {
                    case WhEventType.Created:
                        break;
                    case WhEventType.Start:
                        lastStart = whEvent.Time;
                        isWorking = true;
                        break;
                    case WhEventType.Stop:
                        closedWorkTime += whEvent.Time - lastStart;
                        isWorking = false;
                        break;
                    case WhEventType.Modify:
                        break;
                        throw new ArgumentOutOfRangeException();
                }
            }

            return (closedWorkTime, lastStart, isWorking);
        }

        public bool IsWorking { get; private set; }
        public bool IsHoliday { get; }
        public TimeSpan[] LastDaysWorkTime { get; }

        public void StartWork()
        {
            _lastStart = _time.Now;
            IsWorking = true;
        }

        public void StopWork()
        {
            _todayClosedWorkTime += _time.Now - _lastStart;
            IsWorking = false;
        }

        public TimeSpan GetTodayWorkTime()
        {
            return this.IsWorking
                ? this._todayClosedWorkTime + (this._time.Now - this._lastStart)
                : this._todayClosedWorkTime;
        }

        public void SetHoliday(DateTime date, bool isHoliday, string description)
        {
            throw new NotImplementedException();
        }
    }
}
