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
    public class WorkingTests : TestBase
    {
        [TestMethod]
        public void Working_Start()
        {
            var time = new TestTime();
            var controller = CreateWorkHoursController(time);
            time.Now = DateTime.UtcNow;

            // ACTION
            controller.StartWork();

            // ASSERT
            Assert.IsTrue(controller.IsWorking);
        }
        [TestMethod]
        public void Working_Stop()
        {
            var time = new TestTime();
            var controller = CreateWorkHoursController(time);

            time.Now = DateTime.UtcNow;
            controller.StartWork();
            time.Now = time.Now.AddMinutes(1.0d);

            // ACTION
            controller.StopWork();

            // ASSERT
            Assert.IsFalse(controller.IsWorking);
        }
        [TestMethod]
        public void Working_OneMinute()
        {
            var time = new TestTime();
            var controller = CreateWorkHoursController(time);

            // ACTION
            time.Now = DateTime.UtcNow;
            controller.StartWork();
            time.Now = time.Now.AddMinutes(1.0d);
            controller.StopWork();

            // ASSERT
            Assert.AreEqual(TimeSpan.FromMinutes(1.0d), controller.GetTodayWorkTime());
        }
        [TestMethod]
        public void Working_TwoTimes()
        {
            var time = new TestTime();
            var controller = CreateWorkHoursController(time);

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
        public void Working_GetWorkTimeDuringWorking()
        {
            var time = new TestTime();
            var controller = CreateWorkHoursController(time);

            // ACTION
            time.Now = DateTime.UtcNow;
            controller.StartWork();

            // ASSERT
            time.Now = time.Now.AddMinutes(2.0d);
            Assert.AreEqual(TimeSpan.FromMinutes(2.0d), controller.GetTodayWorkTime());
            time.Now = time.Now.AddMinutes(2.0d);
            Assert.AreEqual(TimeSpan.FromMinutes(4.0d), controller.GetTodayWorkTime());
            time.Now = time.Now.AddMinutes(20.0d);
            Assert.AreEqual(TimeSpan.FromMinutes(24.0d), controller.GetTodayWorkTime());
        }

        [TestMethod]
        public void Working_StartTwiceStopOnce()
        {
            var time = new TestTime();
            var controller = CreateWorkHoursController(time);

            // ACTION
            time.Now = DateTime.UtcNow;
            controller.StartWork();
            time.Now = time.Now.AddMinutes(2.0d);
            controller.StartWork();
            time.Now = time.Now.AddMinutes(3.0d);
            controller.StopWork();

            // ASSERT
            Assert.AreEqual(TimeSpan.FromMinutes(5.0d), controller.GetTodayWorkTime());
        }

        [TestMethod]
        public void Working_DayTransition()
        {
            var time = new TestTime();
            var controller = CreateWorkHoursController(time);

            // ACTION
            time.Now = new DateTime(2000, 1, 1, 22, 30, 0);
            controller.StartWork();
            time.Now = time.Now.AddHours(3.5d);
            controller.StopWork();

            // ASSERT
            Assert.AreEqual(TimeSpan.FromHours(2.0d), controller.GetTodayWorkTime());
        }

    }
}
