using System;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Whol.Logic;

namespace Whol.ConsoleUI
{
    class Program
    {
        private static IServiceProvider _services;
        private static IEventController _controller;
        private static Timer _timer = new Timer(Tick, null, TimeSpan.FromSeconds(0.1d), TimeSpan.FromSeconds(1.0d));
        private static bool _running;
        private static int _counter;
        private static void Tick(object state)
        {
            if (!_running)
                return;
            var workTime = _controller.GetTodayWorkTime();
            Console.Write($"WORK: {workTime:hh\\:mm\\:ss} Press <enter> to stop.            \r");
        }

        private static string StartText()
        {
            var todayWorkTime = _controller.GetTodayWorkTime();
            return $"RELAXING. Workday: {todayWorkTime:hh\\:mm\\:ss}. Type task and <enter> to start work (?<enter>: help):";
        }

        static void Main(string[] args)
        {
            BuildServices();
            Initialize();
            RunUi();

            if (_running)
                Stop();
        }
        private static void BuildServices()
        {
            var builder = new ServiceCollection();

            builder.AddSingleton<ITime, RealTime>();
            builder.AddSingleton<IStorage, SimpleFileSystemStorage>();
            builder.AddSingleton<IUserManager, BuiltInUserManager>();
            builder.AddSingleton<IEventController, EventController>();
            builder.AddSingleton<IHolidayController, HolidayController>();
            builder.AddSingleton<IDisk, FsDisk>();
            builder.AddSingleton<IEventSerializer, OneLineEventSerializer>();

            _services = builder.BuildServiceProvider();
        }
        private static void Initialize()
        {
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);
            _controller = _services.GetRequiredService<IEventController>();
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            if (_running)
                Stop();
        }

        private static void RunUi()
        {
            Console.WriteLine(StartText());
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
            _controller.StartWork(task);

            var blank = new string(' ', Console.WindowWidth - 1) + "\r";
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            Console.Write(blank);
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            Console.Write(blank);
            Console.WriteLine(task);
        }
        private static void Stop()
        {
            _controller.StopWork();
            Console.WriteLine(StartText());
        }
    }
}
