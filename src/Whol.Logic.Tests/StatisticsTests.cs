using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Whol.Logic.Tests.Implementations;

namespace Whol.Logic.Tests
{
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
            time.Now = DateTime.UtcNow; controller.StartWork();
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
        }
    }
}
