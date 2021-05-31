using System;
using System.Collections.Generic;
using System.Linq;

namespace Whol.Logic
{
    public class HolidayController : IHolidayController
    {
        private Holiday[] _holidays;

        private readonly ITime _time;
        private readonly IStorage _storage;
        private readonly IUserManager _userManager;
        private readonly User _user;
        private DateTime _today;
        private Holiday _currentHoliday;

        public bool IsHoliday
        {
            get
            {
                RefreshCurrentHoliday();
                return _currentHoliday != null;
            }
        }
        public string HolidayDescription
        {
            get
            {
                RefreshCurrentHoliday();
                if (_currentHoliday == null)
                    return null;
                return _currentHoliday.Description ?? "Holiday";
            }
        }

        public HolidayController(ITime time, IStorage storage, IUserManager userManager)
        {
            _time = time;
            _storage = storage;
            _userManager = userManager;
            _user = userManager.LoggedInUser;
            Initialize(_storage.LoadHolidays());
        }
        private void Initialize(IEnumerable<Holiday> holidays)
        {
            var array = holidays as Holiday[] ?? holidays.ToArray();
            _holidays = array;
            _today = DateTime.MinValue;
        }

        private void RefreshCurrentHoliday()
        {
            var today = _time.Today;
            if (today != _today)
            {
                _today = today;
                _currentHoliday = _holidays.FirstOrDefault(x => x.Day == _today);
            }
        }

        public void SetHolidays(IEnumerable<Holiday> holidays)
        {
            var array = holidays as Holiday[] ?? holidays.ToArray();
            _storage.SaveHolidays(array);
            Initialize(array);
        }
    }
}
