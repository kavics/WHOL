using System;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Whol.Logic;

namespace Whol.ConsoleUI
{
    class Program
    {
        private static IServiceProvider _services;
        private static Timer _timer = new Timer(Tick, null, TimeSpan.FromSeconds(0.1d), TimeSpan.FromSeconds(1.0d));
        private static bool _running;
        private static int _counter;
        private static void Tick(object state)
        {
            if (!_running)
                return;

            Console.Write($"WORK: {++_counter}\r");
        }

        private static readonly string StartText = "Relaxing. Type task and <enter> to start work:";
        static void Main(string[] args)
        {
            BuildServices();
            Initialize();
            RunUi();
        }
        private static void BuildServices()
        {
            var builder = new ServiceCollection();

            builder.AddSingleton<ITime, RealTime>();
            builder.AddSingleton<IStorage, SimpleFileSystemStorage>();
            builder.AddSingleton<IUserManager, BuiltInUserManager>();
            builder.AddSingleton<IEventController, EventController>();
            builder.AddSingleton<IDisk, FsDisk>();
            builder.AddSingleton<IEventSerializer, OneLineEventSerializer>();

            _services = builder.BuildServiceProvider();
        }
        private static void Initialize()
        {
            var controller = _services.GetRequiredService<IEventController>();
            var todayWorkTime = controller.GetTodayWorkTime();
        }
        private static void RunUi()
        {
            Console.WriteLine(StartText);
            while (true)
            {
                var input = Console.ReadLine();
                if (input == null)
                {
                    throw new NotImplementedException();
                }
                if (_running)
                {
                    Stop();
                    _running = false;
                }
                else
                {
                    Start(input);
                    _running = true;
                }
            }
        }

        private static void Start(string task)
        {
            var controller = _services.GetRequiredService<IEventController>();
            controller.StartWork(task);

            var blank = new string(' ', Console.WindowWidth - 1) + "\r";
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            Console.Write(blank);
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            Console.Write(blank);
            Console.WriteLine(task);
        }
        private static void Stop()
        {
            var controller = _services.GetRequiredService<IEventController>();
            controller.StopWork();

            Console.WriteLine(StartText);
        }
    }
}
