using System.Collections.Generic;
using System.Linq;

namespace Whol.Logic;

public class HolidayController : IHolidayController
{
    private Holiday[] _holidays;

    private readonly ITime _time;
    private readonly IStorage _storage;
    private readonly IUserManager _userManager;

    public HolidayController(ITime time, IStorage storage, IUserManager userManager)
    {
        _time = time;
        _storage = storage;
        _userManager = userManager;
        Initialize(_storage.LoadHolidays());
    }

    private void Initialize(IEnumerable<Holiday> holidays)
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

    public bool IsHoliday { get; private set; }
    public string? HolidayDescription{ get; private set; }
    public void SetHolidays(IEnumerable<Holiday> holidays)
    {
        var array = holidays as Holiday[] ?? holidays.ToArray();
        _storage.SaveHolidays(array);
        Initialize(array);
    }
}