using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
            time.Now = time.Now.AddMinutes(2.0d);

            // ASSERT
            Assert.AreEqual(TimeSpan.FromMinutes(2.0d), controller.GetTodayWorkTime());
        }
    }
}
