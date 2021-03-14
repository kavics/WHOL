using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Whol.Logic.Tests.Implementations;

namespace Whol.Logic.Tests
{
    [TestClass]
    public class BasicTests
    {
        private WorkHoursController CreateController(ITime time)
        {
            var storage = new TestStorage(null, null);
            var userManager = new TestUserManager();
            var user = new User { Email = "user1@example.com" };
            return new WorkHoursController(time, storage, userManager, user);
        }

        [TestMethod]
        public void StartWorking()
        {
            var time = new TestTime();
            var controller = CreateController(time);
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
            var controller = CreateController(time);

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
            var controller = CreateController(time);

            // ACTION
            time.Now = DateTime.UtcNow;
            controller.StartWork();
            time.Now = time.Now.AddMinutes(1.0d);
            controller.StopWork();

            // ASSERT
            Assert.AreEqual(TimeSpan.FromMinutes(1.0d), controller.GetTodayWorkTime());
        }
        [TestMethod]
        public void WorkingTwoTimes()
        {
            var time = new TestTime();
            var controller = CreateController(time);

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
            Assert.AreEqual(TimeSpan.FromMinutes(3.0d), controller.GetTodayWorkTime());
        }
        [TestMethod]
        public void GetWorkTimeDuringWorking()
        {
            var time = new TestTime();
            var controller = CreateController(time);

            // ACTION
            time.Now = DateTime.UtcNow;
            controller.StartWork();
            time.Now = time.Now.AddMinutes(2.0d);

            // ASSERT
            Assert.AreEqual(TimeSpan.FromMinutes(2.0d), controller.GetTodayWorkTime());
        }

        /* ============================================================================= */

        private WorkHoursController CreateController(ITime time, IStorage storage)
        {
            var userManager = new TestUserManager();
            var user = new User { Email = "user1@example.com" };
            return new WorkHoursController(time, storage, userManager, user);
        }

        [TestMethod]
        public void LoadEmpty()
        {
            var time = new TestTime();
            var storage = new TestStorage(null, null);

            // ACTION
            var controller = CreateController(time, storage);

            // ASSERT
            Assert.IsFalse(controller.IsWorking);
            Assert.AreEqual(TimeSpan.FromMinutes(0.0d), controller.GetTodayWorkTime());
        }
        [TestMethod]
        public void LoadEmptyToday()
        {
            var time0 = DateTime.Today;

            var lastDayEvents = new WhEvent[]
            {
                new WhEvent {Time = time0.AddMinutes(-10.0d), EventType = WhEventType.Start},
                new WhEvent {Time = time0.AddMinutes(-2.0d), EventType = WhEventType.Stop},
            };

            var time = new TestTime();
            var storage = new TestStorage(lastDayEvents, null);

            // ACTION
            var controller = CreateController(time, storage);

            // ASSERT
            Assert.IsFalse(controller.IsWorking);
            Assert.AreEqual(TimeSpan.FromMinutes(0.0d), controller.GetTodayWorkTime());
        }
        [TestMethod]
        public void LoadStarted()
        {
            var time0 = DateTime.Today;
            var lastDayEvents = new WhEvent[]
            {
                new WhEvent {Time = time0.AddMinutes(-10.0d), EventType = WhEventType.Start},
                new WhEvent {Time = time0.AddMinutes(-2.0d), EventType = WhEventType.Stop},
                new WhEvent {Time = time0.AddMinutes(1.0d), EventType = WhEventType.Start},
            };

            var time = new TestTime();
            time.Now = time0.AddMinutes(2.0d);
            var storage = new TestStorage(lastDayEvents, null);

            // ACTION
            var controller = CreateController(time, storage);

            // ASSERT
            Assert.IsTrue(controller.IsWorking);
            Assert.AreEqual(TimeSpan.FromMinutes(1.0d), controller.GetTodayWorkTime());
        }
        [TestMethod]
        public void LoadStopped()
        {
            var time0 = DateTime.Today;

            var lastDayEvents = new WhEvent[]
            {
                new WhEvent {Time = time0.AddMinutes(-10.0d), EventType = WhEventType.Start},
                new WhEvent {Time = time0.AddMinutes(-2.0d), EventType = WhEventType.Stop},
                new WhEvent {Time = time0.AddMinutes(1.0d), EventType = WhEventType.Start},
                new WhEvent {Time = time0.AddMinutes(2.0d), EventType = WhEventType.Stop},
            };

            var time = new TestTime();
            time.Now = time0.AddMinutes(3.0d);
            var storage = new TestStorage(lastDayEvents, null);

            // ACTION
            var controller = CreateController(time, storage);

            // ASSERT
            Assert.IsFalse(controller.IsWorking);
            Assert.AreEqual(TimeSpan.FromMinutes(1.0d), controller.GetTodayWorkTime());
        }

        [TestMethod]
        public void LoadToday()
        {
            var time0 = DateTime.Today;

            var lastDayEvents = new WhEvent[]
            {
                new WhEvent {Time = time0.AddMinutes(-10.0d), EventType = WhEventType.Start},
                new WhEvent {Time = time0.AddMinutes(-2.0d), EventType = WhEventType.Stop},
                new WhEvent {Time = time0.AddMinutes(1.0d), EventType = WhEventType.Start},
                new WhEvent {Time = time0.AddMinutes(2.0d), EventType = WhEventType.Stop},
                new WhEvent {Time = time0.AddMinutes(4.0d), EventType = WhEventType.Start},
                new WhEvent {Time = time0.AddMinutes(8.0d), EventType = WhEventType.Stop},
            };

            var time = new TestTime();
            var storage = new TestStorage(lastDayEvents, null);

            // ACTION
            var controller = CreateController(time, storage);

            // ASSERT
            Assert.AreEqual(TimeSpan.FromMinutes(5.0d), controller.GetTodayWorkTime());
        }

        [TestMethod]
        public void SaveStart()
        {
            var time = new TestTime {Now = DateTime.Now};
            var storage = new TestStorage(null, null);
            var controller = CreateController(time, storage);

            // ACTION
            controller.StartWork();

            // ASSERT
            Assert.IsTrue(storage.EventsSaved);
        }
        [TestMethod]
        public void SaveStop()
        {
            var time0 = DateTime.Today;
            var time1 = time0.AddMinutes(3.0d);
            var time = new TestTime { Now = time0 };
            var storage = new TestStorage(null, null);
            var controller = CreateController(time, storage);
            controller.StartWork();
            time.Now = time1;
            storage.EventsSaved = false;

            // ACTION
            controller.StopWork();

            // ASSERT
            Assert.IsTrue(storage.EventsSaved);
        }
        [TestMethod]
        public void SaveOnePeriod()
        {
            var time0 = DateTime.Today;
            var time1 = time0.AddMinutes(3.0d);
            var time = new TestTime();
            var storage = new TestStorage(null, null);
            var controller = CreateController(time, storage);
            storage.EventsSaved = false;

            // ACTION
            time.Now = time0;
            controller.StartWork();
            time.Now = time1;
            controller.StopWork();

            // ASSERT
            var events = storage.LoadEvents().ToArray();
            Assert.AreEqual(2, events.Length);
            Assert.AreEqual(time0, events[0].Time);
            Assert.AreEqual(WhEventType.Start, events[0].EventType);
            Assert.AreEqual(time1, events[1].Time);
            Assert.AreEqual(WhEventType.Stop, events[1].EventType);
        }
    }
}
