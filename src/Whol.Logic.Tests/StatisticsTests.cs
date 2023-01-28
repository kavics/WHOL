using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Whol.Logic.Tests.Implementations;

namespace Whol.Logic.Tests;

[ExcludeFromCodeCoverage]
[TestClass]
public class StatisticsTests : TestBase
{
    [TestMethod]
    public void Stat_SummarizeTasks()
    {
        var services = GetServices();

        var time = (TestTime)services.GetRequiredService<ITime>();
        var controller = services.GetRequiredService<IEventController>();
        var stat = services.GetRequiredService<IStatisticsController>();

        // ACTION
        var time0 = DateTime.UtcNow;
        time.Now = time0; controller.StartWork();
        time.Now = time.Now.AddMinutes(2.0d); controller.StopWork();
        time.Now = time.Now.AddMinutes(3.0d); controller.StartWork("Task1");
        time.Now = time.Now.AddMinutes(2.0d); controller.StopWork();
        time.Now = time.Now.AddMinutes(3.0d); controller.StartWork("Task1");
        time.Now = time.Now.AddMinutes(2.0d); controller.StopWork();
        time.Now = time.Now.AddMinutes(3.0d); controller.StartWork("Task2");
        time.Now = time.Now.AddMinutes(2.0d); controller.StopWork();

        // ASSERT
        Assert.AreEqual(TimeSpan.FromMinutes(8.0d), controller.GetTodayWorkTime());
        var summaries = stat.SummarizeTasks().ToArray();
        Assert.AreEqual(3, summaries.Length);
        Assert.AreEqual(string.Empty, summaries[0].Name);
        Assert.AreEqual(time0, summaries[0].Start);
        Assert.AreEqual(time0.AddMinutes(2.0d), summaries[0].Stop);
        Assert.AreEqual("Task1", summaries[1].Name);
        Assert.AreEqual(time0.AddMinutes(5.0d), summaries[1].Start);
        Assert.AreEqual(time0.AddMinutes(12.0d), summaries[1].Stop);
        Assert.AreEqual("Task2", summaries[2].Name);
        Assert.AreEqual(time0.AddMinutes(15.0d), summaries[2].Start);
        Assert.AreEqual(time0.AddMinutes(17.0d), summaries[2].Stop);
    }

    [TestMethod]
    public void Stat_SummarizeTasks_DayTransition()
    {
        var services = GetServices();

        var time = (TestTime)services.GetRequiredService<ITime>();
        var controller = services.GetRequiredService<IEventController>();
        var stat = services.GetRequiredService<IStatisticsController>();

        // ACTION
        var time0 = DateTime.Today.AddMinutes(-3.0d);
        time.Now = time0; controller.StartWork();
        time.Now = time.Now.AddMinutes(2.0d); controller.StopWork();
        time.Now = time.Now.AddMinutes(3.0d); controller.StartWork("Task1");
        time.Now = time.Now.AddMinutes(2.0d); controller.StopWork();
        time.Now = time.Now.AddMinutes(3.0d); controller.StartWork("Task1");
        time.Now = time.Now.AddMinutes(2.0d); controller.StopWork();
        time.Now = time.Now.AddMinutes(3.0d); controller.StartWork("Task2");
        time.Now = time.Now.AddMinutes(2.0d); controller.StopWork();

        // ASSERT
        Assert.AreEqual(TimeSpan.FromMinutes(6.0d), controller.GetTodayWorkTime());
        var summaries = stat.SummarizeTasks().ToArray();
        Assert.AreEqual(3, summaries.Length);
        Assert.AreEqual(string.Empty, summaries[0].Name);
        Assert.AreEqual(time0, summaries[0].Start);
        Assert.AreEqual(time0.AddMinutes(2.0d), summaries[0].Stop);
        Assert.AreEqual("Task1", summaries[1].Name);
        Assert.AreEqual(time0.AddMinutes(5.0d), summaries[1].Start);
        Assert.AreEqual(time0.AddMinutes(12.0d), summaries[1].Stop);
        Assert.AreEqual("Task2", summaries[2].Name);
        Assert.AreEqual(time0.AddMinutes(15.0d), summaries[2].Start);
        Assert.AreEqual(time0.AddMinutes(17.0d), summaries[2].Stop);
    }
}