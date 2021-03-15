using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Whol.Logic.Tests.Implementations;

namespace Whol.Logic.Tests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class EventTests : TestBase
    {
        [TestMethod]
        public void Events_Loaded()
        {
            var time = new TestTime();
            var storage = new TestStorage(null, null);

            // ACTION
            var controller = CreateWorkHoursController(time, storage);

            // ASSERT
            Assert.IsTrue(storage.EventsLoaded);
        }
        [TestMethod]
        public void Events_LoadEmpty()
        {
            var time = new TestTime();
            var storage = new TestStorage(null, null);

            // ACTION
            var controller = CreateWorkHoursController(time, storage);

            // ASSERT
            Assert.IsFalse(controller.IsWorking);
            Assert.AreEqual(TimeSpan.FromMinutes(0.0d), controller.GetTodayWorkTime());
        }
        [TestMethod]
        public void Events_LoadEmptyToday()
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
            var controller = CreateWorkHoursController(time, storage);

            // ASSERT
            Assert.IsFalse(controller.IsWorking);
            Assert.AreEqual(TimeSpan.FromMinutes(0.0d), controller.GetTodayWorkTime());
        }
        [TestMethod]
        public void Events_LoadStarted()
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
            var controller = CreateWorkHoursController(time, storage);

            // ASSERT
            Assert.IsTrue(controller.IsWorking);
            Assert.AreEqual(TimeSpan.FromMinutes(1.0d), controller.GetTodayWorkTime());
        }
        [TestMethod]
        public void Events_LoadStopped()
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
            var controller = CreateWorkHoursController(time, storage);

            // ASSERT
            Assert.IsFalse(controller.IsWorking);
            Assert.AreEqual(TimeSpan.FromMinutes(1.0d), controller.GetTodayWorkTime());
        }

        [TestMethod]
        public void Events_LoadToday()
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
            var controller = CreateWorkHoursController(time, storage);

            // ASSERT
            Assert.AreEqual(TimeSpan.FromMinutes(5.0d), controller.GetTodayWorkTime());
        }

        [TestMethod]
        public void Events_SaveStart()
        {
            var time = new TestTime { Now = DateTime.Now };
            var storage = new TestStorage(null, null);
            var controller = CreateWorkHoursController(time, storage);

            // ACTION
            controller.StartWork();

            // ASSERT
            Assert.IsTrue(storage.EventsSaved);
        }
        [TestMethod]
        public void Events_SaveStop()
        {
            var time0 = DateTime.Today;
            var time1 = time0.AddMinutes(3.0d);
            var time = new TestTime { Now = time0 };
            var storage = new TestStorage(null, null);
            var controller = CreateWorkHoursController(time, storage);
            controller.StartWork();
            time.Now = time1;
            storage.EventsSaved = false;

            // ACTION
            controller.StopWork();

            // ASSERT
            Assert.IsTrue(storage.EventsSaved);
        }
        [TestMethod]
        public void Events_SaveOnePeriod()
        {
            var time0 = DateTime.Today;
            var time1 = time0.AddMinutes(3.0d);
            var time = new TestTime();
            var storage = new TestStorage(null, null);
            var controller = CreateWorkHoursController(time, storage);
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
