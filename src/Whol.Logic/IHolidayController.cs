using System.Collections.Generic;

namespace Whol.Logic;

public interface IHolidayController
{
    bool IsHoliday { get; }
    string HolidayDescription { get; }
    void SetHolidays(IEnumerable<Holiday> holidays);
}