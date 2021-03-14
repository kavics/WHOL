using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Whol.Logic.Tests.Implementations;

namespace Whol.Logic.Tests
{
    [TestClass]
    public class BasicTests
    {
        private WorkHoursController CreateControllerForBasicTests(ITime time)
        {
            var storage = new TestStorage();
            var userManager = new TestUserManager();
            var user = new User { Email = "user1@example.com" };
            return new WorkHoursController(time, storage, userManager, user);
        }

        [TestMethod]
        public void StartWorking()
        {
            var time = new TestTime();
            var controller = CreateControllerForBasicTests(time);
            time.Now = DateTime.UtcNow;

            // ACTION
            controller.StartWork();

            // ASSERT
            Assert.IsTrue(controller.IsWorking);
        }
        [TestMethod]
        public void StopWorking()
        {
            var time = new TestTime();
            var controller = CreateControllerForBasicTests(time);

            time.Now = DateTime.UtcNow;
            controller.StartWork();
            time.Now = time.Now.AddMinutes(1.0d);

            // ACTION
            controller.StopWork();

            // ASSERT
            Assert.IsFalse(controller.IsWorking);
        }
        [TestMethod]
        public void WorkingOneMinute()
        {
            var time = new TestTime();
            var storage = new TestStorage();
            var userManager = new TestUserManager();
            var user = new User { Email = "user1@example.com" };
            var controller = new WorkHoursController(time, storage, userManager, user);

            // ACTION
            time.Now = DateTime.UtcNow;
            controller.StartWork();
            time.Now = time.Now.AddMinutes(1.0d);
            controller.StopWork();

            // ASSERT
            var diff = Math.Abs(controller.GetTodayWorkTime().Ticks - TimeSpan.FromMinutes(1.0d).Ticks);
            Assert.IsTrue(diff < TimeSpan.FromMilliseconds(1.0d).Ticks);
        }
        [TestMethod]
        public void WorkingTwoTimes()
        {
            var time = new TestTime();
            var storage = new TestStorage();
            var userManager = new TestUserManager();
            var user = new User { Email = "user1@example.com" };
            var controller = new WorkHoursController(time, storage, userManager, user);

            // ACTION
            time.Now = DateTime.UtcNow;
            controller.StartWork();
            time.Now = time.Now.AddMinutes(1.0d);
            controller.StopWork();
            time.Now = time.Now.AddMinutes(1.0d);
            controller.StartWork();
            time.Now = time.Now.AddMinutes(2.0d);
            controller.StopWork();

            // ASSERT
            var diff = Math.Abs(controller.GetTodayWorkTime().Ticks - TimeSpan.FromMinutes(3.0d).Ticks);
            Assert.IsTrue(diff < TimeSpan.FromMilliseconds(1.0d).Ticks);
        }
        [TestMethod]
        public void GetWorkTimeDuringWorking()
        {
            var time = new TestTime();
            var storage = new TestStorage();
            var userManager = new TestUserManager();
            var user = new User { Email = "user1@example.com" };
            var controller = new WorkHoursController(time, storage, userManager, user);

            // ACTION
            time.Now = DateTime.UtcNow;
            controller.StartWork();
            time.Now = time.Now.AddMinutes(2.0d);

            // ASSERT
            var diff = Math.Abs(controller.GetTodayWorkTime().Ticks - TimeSpan.FromMinutes(2.0d).Ticks);
            Assert.IsTrue(diff < TimeSpan.FromMilliseconds(1.0d).Ticks);
        }
    }
}
