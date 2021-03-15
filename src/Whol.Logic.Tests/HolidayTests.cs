using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Whol.Logic.Tests.Implementations;

namespace Whol.Logic.Tests
{
    [TestClass]
    public class HolidayTests : TestBase
    {
        [TestMethod]
        public void Holidays_Loaded()
        {
            var time = new TestTime();
            var storage = new TestStorage(null, null);

            // ACTION
            var controller = CreateController(time, storage);

            // ASSERT
            Assert.IsTrue(storage.HolidaysLoaded);
        }
        [TestMethod]
        public void Holidays_LoadEmpty()
        {
            var time = new TestTime();
            var holidays = new Holiday[0];
            var storage = new TestStorage(null, holidays);

            // ACTION
            var controller = CreateController(time, storage);

            // ASSERT
            Assert.IsTrue(storage.HolidaysLoaded);
            Assert.IsFalse(controller.IsHoliday);
        }
        [TestMethod]
        public void Holidays_Load_TodayIsHoliday()
        {
            var time = new TestTime();
            var today = DateTime.Today;
            time.Today = today;
            var holidays = new Holiday[]
            {
                new Holiday {Day = today.AddDays(-2.0d)},
                new Holiday {Day = today},
                new Holiday {Day = today.AddDays((1.0d))},
            };
            var storage = new TestStorage(null, holidays);

            // ACTION
            var controller = CreateController(time, storage);

            // ASSERT
            Assert.IsTrue(controller.IsHoliday);
        }
        [TestMethod]
        public void Holidays_Load_TodayIsNotHoliday()
        {
            var time = new TestTime();
            var today = DateTime.Today;
            time.Today = today;
            var holidays = new Holiday[]
            {
                new Holiday {Day = today.AddDays(-2.0d)},
                new Holiday {Day = today.AddDays((1.0d))},
            };
            var storage = new TestStorage(null, holidays);

            // ACTION
            var controller = CreateController(time, storage);

            // ASSERT
            Assert.IsFalse(controller.IsHoliday);
        }
    }
}
