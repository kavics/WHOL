using System.Diagnostics.CodeAnalysis;
using Whol.Logic.Tests.Implementations;

namespace Whol.Logic.Tests
{
    [ExcludeFromCodeCoverage]
    public abstract class TestBase
    {
        protected EventController CreateWorkHoursController(ITime time)
        {
            var storage = new TestStorage(null, null);
            var userManager = new TestUserManager();
            var user = new User { Email = "user1@example.com" };
            return new EventController(time, storage, userManager, user);
        }
        protected EventController CreateWorkHoursController(ITime time, IStorage storage)
        {
            var userManager = new TestUserManager();
            var user = new User { Email = "user1@example.com" };
            return new EventController(time, storage, userManager, user);
        }

        protected HolidayController CreateHolidayController(ITime time, IStorage storage)
        {
            var userManager = new TestUserManager();
            var user = new User { Email = "user1@example.com" };
            return new HolidayController(time, storage, userManager, user);
        }
    }
}
