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
        public void Holidays_Load_Empty()
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
            var today = DateTime.Today;
            var time = new TestTime { Today = today };
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
            var today = DateTime.Today;
            var time = new TestTime { Today = today };
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

        [TestMethod]
        public void Holidays_Edit_AddFirst()
        {
            var today = DateTime.Today;
            var time = new TestTime {Today = today};
            var holidays = new Holiday[0];
            var storage = new TestStorage(null, holidays);
            var controller = CreateController(time, storage);

            // ACTION
            controller.SetHolidays(new List<Holiday>
            {
                new Holiday{Day = today}
            });

            // ASSERT
            Assert.IsTrue(storage.HolidaysSaved);
            Assert.IsTrue(controller.IsHoliday);
        }
        [TestMethod]
        public void Holidays_Edit_Add()
        {
            var today = DateTime.Today;
            var time = new TestTime { Today = today };
            var holidays = new Holiday[0];
            var storage = new TestStorage(null, holidays);
            var controller = CreateController(time, storage);

            // ACTION
            controller.SetHolidays(new List<Holiday>
            {
                new Holiday{Day = today}
            });

            // ASSERT
            Assert.IsTrue(storage.HolidaysSaved);
            Assert.IsTrue(controller.IsHoliday);
        }
        [TestMethod]
        public void Holidays_Edit_InsertToday()
        {
            var today = DateTime.Today;
            var time = new TestTime { Today = today };
            var holidays = new[]
            {
                new Holiday{Day=today.AddDays(-10.0d)},
                new Holiday{Day=today.AddDays(+10.0d)}
            };
            var storage = new TestStorage(null, holidays);
            var controller = CreateController(time, storage);

            // ACTION
            controller.SetHolidays(new[]
            {
                new Holiday{Day=today.AddDays(-10.0d)},
                new Holiday{Day=today},
                new Holiday{Day=today.AddDays(+10.0d)}
            });

            // ASSERT
            Assert.IsTrue(storage.HolidaysSaved);
            Assert.IsTrue(controller.IsHoliday);
        }
        [TestMethod]
        public void Holidays_Edit_InsertOther()
        {
            var today = DateTime.Today;
            var time = new TestTime { Today = today };
            var holidays = new[]
            {
                new Holiday{Day=today.AddDays(-10.0d)},
                new Holiday{Day=today.AddDays(+10.0d)}
            };
            var storage = new TestStorage(null, holidays);
            var controller = CreateController(time, storage);

            // ACTION
            controller.SetHolidays(new[]
            {
                new Holiday{Day=today.AddDays(-10.0d)},
                new Holiday{Day=today.AddDays(1.0d)},
                new Holiday{Day=today.AddDays(+10.0d)}
            });

            // ASSERT
            Assert.IsTrue(storage.HolidaysSaved);
            Assert.IsFalse(controller.IsHoliday);
        }
        [TestMethod]
        public void Holidays_Edit_Modify()
        {
            var today = DateTime.Today;
            var time = new TestTime { Today = today };
            var holidays = new[]
            {
                new Holiday{Day=today}
            };
            var storage = new TestStorage(null, holidays);
            var controller = CreateController(time, storage);

            // ACTION
            controller.SetHolidays(new[]
            {
                new Holiday{Day=today, Description = "Developers Day"},
            });

            // ASSERT
            Assert.IsTrue(storage.HolidaysSaved);
            Assert.IsTrue(controller.IsHoliday);
        }
        [TestMethod]
        public void Holidays_Edit_Reset()
        {
            var today = DateTime.Today;
            var time = new TestTime { Today = today };
            var holidays = new[]
            {
                new Holiday{Day=today.AddDays(-10.0d)},
                new Holiday{Day=today},
                new Holiday{Day=today.AddDays(+10.0d)}
            };
            var storage = new TestStorage(null, holidays);
            var controller = CreateController(time, storage);

            // ACTION
            controller.SetHolidays(new[]
            {
                new Holiday{Day=today.AddDays(-10.0d)},
                new Holiday{Day=today.AddDays(+10.0d)}
            });

            // ASSERT
            Assert.IsTrue(storage.HolidaysSaved);
            Assert.IsFalse(controller.IsHoliday);
        }

        [TestMethod]
        public void Holidays_Description_Null()
        {
            var today = DateTime.Today;
            var time = new TestTime { Today = today };
            var holidays = new[] { new Holiday { Day = today } };
            var storage = new TestStorage(null, holidays);

            // ACTION
            var controller = CreateController(time, storage);

            // ASSERT
            Assert.IsTrue(controller.IsHoliday);
            Assert.AreEqual("Holiday", controller.HolidayDescription);
        }
        [TestMethod]
        public void Holidays_Description_NotNull()
        {
            var today = DateTime.Today;
            var time = new TestTime { Today = today };
            var holidays = new[] { new Holiday { Day = today, Description = "Developers Day" } };
            var storage = new TestStorage(null, holidays);

            // ACTION
            var controller = CreateController(time, storage);

            // ASSERT
            Assert.IsTrue(controller.IsHoliday);
            Assert.AreEqual("Developers Day", controller.HolidayDescription);
        }
        [TestMethod]
        public void Holidays_Description_ChangedToNull()
        {
            var today = DateTime.Today;
            var time = new TestTime { Today = today };
            var holidays = new[] { new Holiday { Day = today, Description = "Developers Day" } };
            var storage = new TestStorage(null, holidays);
            var controller = CreateController(time, storage);
            Assert.IsTrue(controller.IsHoliday);
            Assert.AreEqual("Developers Day", controller.HolidayDescription);

            // ACTION
            controller.SetHolidays(new[]
            {
                new Holiday{Day=today, Description = null},
            });

            // ASSERT
            Assert.IsTrue(controller.IsHoliday);
            Assert.AreEqual("Holiday", controller.HolidayDescription);
        }
        [TestMethod]
        public void Holidays_Description_ChangedToNotNull()
        {
            var today = DateTime.Today;
            var time = new TestTime { Today = today };
            var holidays = new[] { new Holiday { Day = today, Description = null } };
            var storage = new TestStorage(null, holidays);
            var controller = CreateController(time, storage);
            Assert.IsTrue(controller.IsHoliday);
            Assert.AreEqual("Holiday", controller.HolidayDescription);

            // ACTION
            controller.SetHolidays(new[]
            {
                new Holiday{Day=today, Description = "Developers Day"},
            });

            // ASSERT
            Assert.IsTrue(controller.IsHoliday);
            Assert.AreEqual("Developers Day", controller.HolidayDescription);
        }
    }
}
