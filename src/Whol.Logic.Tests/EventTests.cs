using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
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
            var services = GetServices();
            var storage = (TestStorage)services.GetRequiredService<IStorage>();

            // ACTION
            var _ = services.GetRequiredService<IEventController>();

            // ASSERT
            Assert.IsTrue(storage.EventsLoaded);
        }
        [TestMethod]
        public void Events_LoadEmpty()
        {
            var services = GetServices();

            // ACTION
            var controller = services.GetRequiredService<IEventController>();

            // ASSERT
            Assert.IsFalse(controller.IsWorking);
            Assert.AreEqual(TimeSpan.FromMinutes(0.0d), controller.GetTodayWorkTime());
        }
        [TestMethod]
        public void Events_LoadEmptyToday()
        {
            var time0 = DateTime.Today;
            var lastDayEvents = new []
            {
                new Event {Time = time0.AddMinutes(-10.0d), EventType = EventType.Start},
                new Event {Time = time0.AddMinutes(-2.0d), EventType = EventType.Stop},
            };
            var services = GetServices();
            var storage = (TestStorage)services.GetRequiredService<IStorage>();
            storage.Initialize(lastDayEvents, null);

            // ACTION
            var controller = services.GetRequiredService<IEventController>();

            // ASSERT
            Assert.IsFalse(controller.IsWorking);
            Assert.AreEqual(TimeSpan.FromMinutes(0.0d), controller.GetTodayWorkTime());
        }
        [TestMethod]
        public void Events_LoadStarted()
        {
            var time0 = DateTime.Today;
            var lastDayEvents = new []
            {
                new Event {Time = time0.AddMinutes(-10.0d), EventType = EventType.Start},
                new Event {Time = time0.AddMinutes(-2.0d), EventType = EventType.Stop},
                new Event {Time = time0.AddMinutes(1.0d), EventType = EventType.Start},
            };
            var services = GetServices();
            var time = (TestTime)services.GetRequiredService<ITime>();
            var storage = (TestStorage)services.GetRequiredService<IStorage>();
            storage.Initialize(lastDayEvents, null);

            time.Now = time0.AddMinutes(2.0d);

            // ACTION
            var controller = services.GetRequiredService<IEventController>();

            // ASSERT
            Assert.IsTrue(controller.IsWorking);
            Assert.AreEqual(TimeSpan.FromMinutes(1.0d), controller.GetTodayWorkTime());
        }
        [TestMethod]
        public void Events_LoadStopped()
        {
            var time0 = DateTime.Today;
            var lastDayEvents = new []
            {
                new Event {Time = time0.AddMinutes(-10.0d), EventType = EventType.Start},
                new Event {Time = time0.AddMinutes(-2.0d), EventType = EventType.Stop},
                new Event {Time = time0.AddMinutes(1.0d), EventType = EventType.Start},
                new Event {Time = time0.AddMinutes(2.0d), EventType = EventType.Stop},
            };

            var services = GetServices();
            var time = (TestTime)services.GetRequiredService<ITime>();
            var storage = (TestStorage)services.GetRequiredService<IStorage>();
            storage.Initialize(lastDayEvents, null);

            time.Now = time0.AddMinutes(3.0d);

            // ACTION
            var controller = services.GetRequiredService<IEventController>();

            // ASSERT
            Assert.IsFalse(controller.IsWorking);
            Assert.AreEqual(TimeSpan.FromMinutes(1.0d), controller.GetTodayWorkTime());
        }

        [TestMethod]
        public void Events_LoadToday()
        {
            var time0 = DateTime.Today;
            var lastDayEvents = new []
            {
                new Event {Time = time0.AddMinutes(-10.0d), EventType = EventType.Start},
                new Event {Time = time0.AddMinutes(-2.0d), EventType = EventType.Stop},
                new Event {Time = time0.AddMinutes(1.0d), EventType = EventType.Start},
                new Event {Time = time0.AddMinutes(2.0d), EventType = EventType.Stop},
                new Event {Time = time0.AddMinutes(4.0d), EventType = EventType.Start},
                new Event {Time = time0.AddMinutes(8.0d), EventType = EventType.Stop},
            };
            var services = GetServices();
            var storage = (TestStorage)services.GetRequiredService<IStorage>();
            storage.Initialize(lastDayEvents, null);

            // ACTION
            var controller = services.GetRequiredService<IEventController>();

            // ASSERT
            Assert.AreEqual(TimeSpan.FromMinutes(5.0d), controller.GetTodayWorkTime());
        }

        [TestMethod]
        public void Events_SaveStart()
        {
            var services = GetServices();
            var storage = (TestStorage)services.GetRequiredService<IStorage>();
            var controller = services.GetRequiredService<IEventController>();

            // ACTION
            controller.StartWork();

            // ASSERT
            Assert.IsTrue(storage.EventsSaved);
        }
        [TestMethod]
        public void Events_SaveStop()
        {
            var services = GetServices();
            var time = (TestTime) services.GetRequiredService<ITime>();
            var storage = (TestStorage)services.GetRequiredService<IStorage>();
            var controller = services.GetRequiredService<IEventController>();
            var time0 = DateTime.Today;
            var time1 = time0.AddMinutes(3.0d);
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
            var services = GetServices();
            var time = (TestTime)services.GetRequiredService<ITime>();
            var storage = (TestStorage)services.GetRequiredService<IStorage>();
            var controller = services.GetRequiredService<IEventController>();

            var time0 = DateTime.Today;
            var time1 = time0.AddMinutes(3.0d);
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
            Assert.AreEqual(EventType.Start, events[0].EventType);
            Assert.AreEqual(time1, events[1].Time);
            Assert.AreEqual(EventType.Stop, events[1].EventType);
        }

        [TestMethod]
        public void Events_StartTask()
        {
            var services = GetServices();
            var storage = (TestStorage)services.GetRequiredService<IStorage>();
            var controller = services.GetRequiredService<IEventController>();

            // ACTION
            controller.StartWork("Task1");

            // ASSERT
            var lastEvent = storage.LoadEvents().Last();
            Assert.AreEqual(EventType.Start, lastEvent.EventType);
            Assert.AreEqual("Task1", lastEvent.Task);
        }
        [TestMethod]
        public void Events_LoadStartedTask()
        {
            var services = GetServices();
            var time = (TestTime)services.GetRequiredService<ITime>();
            var storage = (TestStorage)services.GetRequiredService<IStorage>();
            var time0 = DateTime.Today;
            var lastDayEvents = new []
            {
                new Event {Time = time0.AddMinutes(-10.0d), EventType = EventType.Start},
                new Event {Time = time0.AddMinutes(-2.0d), EventType = EventType.Stop},
                new Event {Time = time0.AddMinutes(1.0d), EventType = EventType.Start, Task = "Task1"},
            };
            storage.Initialize(lastDayEvents, null);

            time.Now = time0.AddMinutes(2.0d);

            // ACTION
            var controller = services.GetRequiredService<IEventController>();

            // ASSERT
            Assert.IsTrue(controller.IsWorking);
            Assert.AreEqual("Task1", controller.CurrentTask);
            Assert.AreEqual(TimeSpan.FromMinutes(1.0d), controller.GetTodayWorkTime());
        }
        [TestMethod]
        public void Events_LoadEventList()
        {
            var services = GetServices();
            var time = (TestTime)services.GetRequiredService<ITime>();
            var storage = (TestStorage)services.GetRequiredService<IStorage>();
            var time0 = DateTime.Today;
            var lastDayEvents = new []
            {
                new Event {Time = time0.AddMinutes(-8.0d), EventType = EventType.Start, Task = "Task1"},
                new Event {Time = time0.AddMinutes(-7.0d), EventType = EventType.Stop},
                new Event {Time = time0.AddMinutes(-6.0d), EventType = EventType.Start, Task = "Task2"},
                new Event {Time = time0.AddMinutes(-5.0d), EventType = EventType.Stop},
                new Event {Time = time0.AddMinutes(-4.0d), EventType = EventType.Start, Task = "Task1"},
                new Event {Time = time0.AddMinutes(-3.0d), EventType = EventType.Stop},
                new Event {Time = time0.AddMinutes(-2.0d), EventType = EventType.Start, Task = "Task1"},
                new Event {Time = time0.AddMinutes(-1.0d), EventType = EventType.Stop},
                new Event {Time = time0.AddMinutes(1.0d), EventType = EventType.Start, Task = "Task3"},
            };
            storage.Initialize(lastDayEvents, null);

            time.Now = time0.AddMinutes(2.0d);

            // ACTION
            var controller = services.GetRequiredService<IEventController>();

            // ASSERT
            Assert.AreEqual("Task3", controller.CurrentTask);
            Assert.AreEqual("Task2,Task1,Task3", string.Join(",", controller.Tasks));
        }
        [TestMethod]
        public void Events_StartNewInEventList()
        {
            var services = GetServices();
            var time = (TestTime)services.GetRequiredService<ITime>();
            var storage = (TestStorage)services.GetRequiredService<IStorage>();
            var time0 = DateTime.Today;
            var lastDayEvents = new []
            {
                new Event {Time = time0.AddMinutes(-10.0d), EventType = EventType.Start, Task = "Task1"},
                new Event {Time = time0.AddMinutes(-9.0d), EventType = EventType.Stop},
                new Event {Time = time0.AddMinutes(-8.0d), EventType = EventType.Start, Task = "Task2"},
                new Event {Time = time0.AddMinutes(-7.0d), EventType = EventType.Stop},
                new Event {Time = time0.AddMinutes(-6.0d), EventType = EventType.Start, Task = "Task1"},
                new Event {Time = time0.AddMinutes(-5.0d), EventType = EventType.Stop},
                new Event {Time = time0.AddMinutes(-4.0d), EventType = EventType.Start, Task = "Task1"},
                new Event {Time = time0.AddMinutes(-3.0d), EventType = EventType.Stop},
                new Event {Time = time0.AddMinutes(-2.0d), EventType = EventType.Start, Task = "Task3"},
                new Event {Time = time0.AddMinutes(-1.0d), EventType = EventType.Stop},
            };
            storage.Initialize(lastDayEvents, null);
            time.Now = time0.AddMinutes(2.0d);
            var controller = services.GetRequiredService<IEventController>();

            // ACTION
            controller.StartWork("Task4");

            // ASSERT
            Assert.AreEqual("Task4", controller.CurrentTask);
            Assert.AreEqual("Task2,Task1,Task3,Task4", string.Join(",", controller.Tasks));
        }
        [TestMethod]
        public void Events_StartOlderInEventList()
        {
            var services = GetServices();
            var time = (TestTime)services.GetRequiredService<ITime>();
            var storage = (TestStorage)services.GetRequiredService<IStorage>();
            var time0 = DateTime.Today;
            var lastDayEvents = new []
            {
                new Event {Time = time0.AddMinutes(-10.0d), EventType = EventType.Start, Task = "Task1"},
                new Event {Time = time0.AddMinutes(-9.0d), EventType = EventType.Stop},
                new Event {Time = time0.AddMinutes(-8.0d), EventType = EventType.Start, Task = "Task2"},
                new Event {Time = time0.AddMinutes(-7.0d), EventType = EventType.Stop},
                new Event {Time = time0.AddMinutes(-6.0d), EventType = EventType.Start, Task = "Task1"},
                new Event {Time = time0.AddMinutes(-5.0d), EventType = EventType.Stop},
                new Event {Time = time0.AddMinutes(-4.0d), EventType = EventType.Start, Task = "Task1"},
                new Event {Time = time0.AddMinutes(-3.0d), EventType = EventType.Stop},
                new Event {Time = time0.AddMinutes(-2.0d), EventType = EventType.Start, Task = "Task3"},
                new Event {Time = time0.AddMinutes(-1.0d), EventType = EventType.Stop},
            };
            storage.Initialize(lastDayEvents, null);
            time.Now = time0.AddMinutes(2.0d);
            var controller = services.GetRequiredService<IEventController>();

            // ACTION
            controller.StartWork("Task2");

            // ASSERT
            Assert.AreEqual("Task2", controller.CurrentTask);
            Assert.AreEqual("Task1,Task3,Task2", string.Join(",", controller.Tasks));
        }
    }
}
