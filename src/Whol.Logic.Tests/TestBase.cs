﻿using Whol.Logic.Tests.Implementations;

namespace Whol.Logic.Tests
{
    public abstract class TestBase
    {
        protected WorkHoursController CreateController(ITime time)
        {
            var storage = new TestStorage(null, null);
            var userManager = new TestUserManager();
            var user = new User { Email = "user1@example.com" };
            return new WorkHoursController(time, storage, userManager, user);
        }
        protected WorkHoursController CreateController(ITime time, IStorage storage)
        {
            var userManager = new TestUserManager();
            var user = new User { Email = "user1@example.com" };
            return new WorkHoursController(time, storage, userManager, user);
        }
    }
}
