using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Whol.Logic
{
    public class WorkHoursController : IWorkHoursController
    {
        private List<WhEvent> _whEvents;
        private Holiday[] _holidays;

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
            Initialize();
        }

        private void Initialize()
        {
            InitializeHolidays(_storage.LoadHolidays());
            InitializeEvents();
        }

        private void InitializeHolidays(IEnumerable<Holiday> holidays)
        {
            var array = holidays as Holiday[] ?? holidays.ToArray();
            var holiday = array.FirstOrDefault(x => x.Day == _time.Today);
            if (holiday == null)
            {
                IsHoliday = false;
                HolidayDescription = null;
            }
            else
            {
                IsHoliday = true;
                HolidayDescription = holiday.Description ?? "Holiday";
            }
            _holidays = array;
        }
        private void InitializeEvents()
        {
            var whEvents = _storage.LoadEvents().ToList();

            var lastStart = DateTime.MinValue;
            var closedTime = TimeSpan.Zero;
            var isWorking = false;

            foreach (var whEvent in whEvents)
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
                        closedTime += whEvent.Time - lastStart;
                        isWorking = false;
                        break;
                    case WhEventType.Modify:
                        break;
                        throw new ArgumentOutOfRangeException();
                }
            }

            _whEvents = whEvents;
            _todayClosedWorkTime = closedTime;
            _lastStart = lastStart;
            IsWorking = isWorking;
        }

        public bool IsWorking { get; private set; }
        public bool IsHoliday { get; private set; }
        public string HolidayDescription { get; private set; }
        public TimeSpan[] LastDaysWorkTime { get; }

        public void StartWork()
        {
            _lastStart = _time.Now;
            IsWorking = true;
            _whEvents.Add(new WhEvent{Time = _time.Now, EventType = WhEventType.Start});
            _storage.SaveEvents(_whEvents);
        }

        public void StopWork()
        {
            _todayClosedWorkTime += _time.Now - _lastStart;
            IsWorking = false;
            _whEvents.Add(new WhEvent { Time = _time.Now, EventType = WhEventType.Stop });
            _storage.SaveEvents(_whEvents);
        }

        public TimeSpan GetTodayWorkTime()
        {
            return this.IsWorking
                ? this._todayClosedWorkTime + (this._time.Now - this._lastStart)
                : this._todayClosedWorkTime;
        }

        public void SetHolidays(IEnumerable<Holiday> holidays)
        {
            var array = holidays as Holiday[] ?? holidays.ToArray();
            _storage.SaveHolidays(array);
            InitializeHolidays(array);
        }
    }
}
