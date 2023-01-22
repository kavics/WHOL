using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Whol.Logic.Tests.Implementations;

namespace Whol.Logic.Tests
{
    [ExcludeFromCodeCoverage]
    public abstract class TestBase
    {
        protected IServiceProvider GetServices(Action<IServiceCollection> configureServices = null)
        {
            var services = new ServiceCollection()
                .AddSingleton<ITime, TestTime>()
                .AddSingleton<IStorage, TestStorage>()
                .AddSingleton<IUserManager, TestUserManager>()
                .AddSingleton<IEventController, EventController>()
                .AddSingleton<IHolidayController, HolidayController>()
                .AddSingleton<IStatisticsController, StatisticsController>()
                ;
            configureServices?.Invoke(services);
            return services.BuildServiceProvider();
        }
    }
}
