using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Whol.Logic.Tests.Implementations;

namespace Whol.Logic.Tests;

[ExcludeFromCodeCoverage]
[TestClass]
public class HolidayTests : TestBase
{
    [TestMethod]
    public void Holidays_Loaded()
    {
        var services = GetServices();

        // ACTION
        var _ = services.GetRequiredService<IHolidayController>();

        // ASSERT
        var storage = (TestStorage)services.GetRequiredService<IStorage>();
        Assert.IsTrue(storage.HolidaysLoaded);
    }
    [TestMethod]
    public void Holidays_Load_Empty()
    {
        var services = GetServices();

        // ACTION
        var controller = services.GetRequiredService<IHolidayController>();

        // ASSERT
        var storage = (TestStorage)services.GetRequiredService<IStorage>();
        Assert.IsTrue(storage.HolidaysLoaded);
        Assert.IsFalse(controller.IsHoliday);
    }
    [TestMethod]
    public void Holidays_Load_TodayIsHoliday()
    {
        var services = GetServices();
        var time = (TestTime) services.GetRequiredService<ITime>();
        var today = DateTime.Today;
        time.Now = today;
        var holidays = new []
        {
            new Holiday {Day = today.AddDays(-2.0d)},
            new Holiday {Day = today},
            new Holiday {Day = today.AddDays((1.0d))},
        };
        var storage = (TestStorage)services.GetRequiredService<IStorage>();
        storage.Initialize(null, holidays);

        // ACTION
        var controller = (HolidayController)services.GetRequiredService<IHolidayController>();

        // ASSERT
        Assert.IsTrue(controller.IsHoliday);
    }
    [TestMethod]
    public void Holidays_Load_TodayIsNotHoliday()
    {
        var services = GetServices();
        var time = (TestTime)services.GetRequiredService<ITime>();
        var today = DateTime.Today;
        time.Now = today;
        var holidays = new []
        {
            new Holiday {Day = today.AddDays(-2.0d)},
            new Holiday {Day = today.AddDays((1.0d))},
        };
        var storage = (TestStorage)services.GetRequiredService<IStorage>();
        storage.Initialize(null, holidays);

        // ACTION
        var controller = (HolidayController)services.GetRequiredService<IHolidayController>();

        // ASSERT
        Assert.IsFalse(controller.IsHoliday);
    }

    [TestMethod]
    public void Holidays_Edit_AddFirst()
    {
        var services = GetServices();
        var time = (TestTime)services.GetRequiredService<ITime>();
        var today = DateTime.Today;
        time.Now = today;
        var storage = (TestStorage)services.GetRequiredService<IStorage>();
        var controller = (HolidayController)services.GetRequiredService<IHolidayController>();

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
        var services = GetServices();
        var time = (TestTime)services.GetRequiredService<ITime>();
        var today = DateTime.Today;
        time.Now = today;
        var storage = (TestStorage)services.GetRequiredService<IStorage>();
        var holidays = new []
        {
            new Holiday {Day = today.AddDays(-2.0d)},
        };
        storage.Initialize(null, holidays);
        var controller = (HolidayController)services.GetRequiredService<IHolidayController>();

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
        var services = GetServices();
        var time = (TestTime)services.GetRequiredService<ITime>();
        var today = DateTime.Today;
        time.Now = today;
        var storage = (TestStorage)services.GetRequiredService<IStorage>();
        var holidays = new[]
        {
            new Holiday{Day=today.AddDays(-10.0d)},
            new Holiday{Day=today.AddDays(+10.0d)}
        };
        storage.Initialize(null, holidays);
        var controller = (HolidayController)services.GetRequiredService<IHolidayController>();

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
        var services = GetServices();
        var time = (TestTime)services.GetRequiredService<ITime>();
        var today = DateTime.Today;
        time.Now = today;
        var storage = (TestStorage)services.GetRequiredService<IStorage>();
        var holidays = new[]
        {
            new Holiday{Day=today.AddDays(-10.0d)},
            new Holiday{Day=today.AddDays(+10.0d)}
        };
        storage.Initialize(null, holidays);
        var controller = (HolidayController)services.GetRequiredService<IHolidayController>();

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
        var services = GetServices();
        var time = (TestTime)services.GetRequiredService<ITime>();
        var today = DateTime.Today;
        time.Now = today;
        var storage = (TestStorage)services.GetRequiredService<IStorage>();
        var holidays = new[]
        {
            new Holiday{Day=today}
        };
        storage.Initialize(null, holidays);
        var controller = (HolidayController)services.GetRequiredService<IHolidayController>();

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
        var services = GetServices();
        var time = (TestTime)services.GetRequiredService<ITime>();
        var today = DateTime.Today;
        time.Now = today;
        var storage = (TestStorage)services.GetRequiredService<IStorage>();
        var holidays = new[]
        {
            new Holiday{Day=today.AddDays(-10.0d)},
            new Holiday{Day=today},
            new Holiday{Day=today.AddDays(+10.0d)}
        };
        storage.Initialize(null, holidays);
        var controller = (HolidayController)services.GetRequiredService<IHolidayController>();

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
        var services = GetServices();
        var time = (TestTime)services.GetRequiredService<ITime>();
        var today = DateTime.Today;
        time.Now = today;
        var storage = (TestStorage)services.GetRequiredService<IStorage>();
        var holidays = new[]
        {
            new Holiday{Day=today},
        };
        storage.Initialize(null, holidays);

        // ACTION
        var controller = (HolidayController)services.GetRequiredService<IHolidayController>();

        // ASSERT
        Assert.IsTrue(controller.IsHoliday);
        Assert.AreEqual("Holiday", controller.HolidayDescription);
    }
    [TestMethod]
    public void Holidays_Description_NotNull()
    {
        var services = GetServices();
        var time = (TestTime)services.GetRequiredService<ITime>();
        var today = DateTime.Today;
        time.Now = today;
        var storage = (TestStorage)services.GetRequiredService<IStorage>();
        var holidays = new[] { new Holiday { Day = today, Description = "Developers Day" } };
        storage.Initialize(null, holidays);

        // ACTION
        var controller = (HolidayController)services.GetRequiredService<IHolidayController>();

        // ASSERT
        Assert.IsTrue(controller.IsHoliday);
        Assert.AreEqual("Developers Day", controller.HolidayDescription);
    }
    [TestMethod]
    public void Holidays_Description_ChangedToNull()
    {
        var services = GetServices();
        var time = (TestTime)services.GetRequiredService<ITime>();
        var today = DateTime.Today;
        time.Now = today;
        var storage = (TestStorage)services.GetRequiredService<IStorage>();
        var holidays = new[] { new Holiday { Day = today, Description = "Developers Day" } };
        storage.Initialize(null, holidays);
        var controller = (HolidayController)services.GetRequiredService<IHolidayController>();
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
        var services = GetServices();
        var time = (TestTime)services.GetRequiredService<ITime>();
        var today = DateTime.Today;
        time.Now = today;
        var storage = (TestStorage)services.GetRequiredService<IStorage>();
        var holidays = new[] { new Holiday { Day = today, Description = "initial" } };
        storage.Initialize(null, holidays);
        var controller = (HolidayController)services.GetRequiredService<IHolidayController>();
        Assert.IsTrue(controller.IsHoliday);
        Assert.AreEqual("initial", controller.HolidayDescription);

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