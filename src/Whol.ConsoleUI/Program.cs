using System;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Whol.Logic;

namespace Whol.ConsoleUI
{
    class Program
    {
        //private static ITime _time;
        private static IServiceProvider _services;
        private static IEventController _eventController;
        private static IHolidayController _holiday;
        private static readonly Timer Timer = new Timer(Tick, null, TimeSpan.FromSeconds(0.1d), TimeSpan.FromSeconds(1.0d));
        private static bool _running;

        private static void Tick(object state)
        {
            if (!_running)
                return;
            var workTime = _eventController.GetTodayWorkTime();
            Console.Write($"WORK: {workTime:hh\\:mm\\:ss} {_holiday.HolidayDescription} Press <enter> to stop.            \r");
        }

        private static string StartText()
        {
            var dayText = _holiday.IsHoliday ? _holiday.HolidayDescription : "Workday";
            var todayWorkTime = _eventController.GetTodayWorkTime();
            return $"RELAXING. {todayWorkTime:hh\\:mm\\:ss} {dayText}. Type task and <enter> to start work (?<enter>: help):";
        }
        private static string HelpText()
        {
            return $"HELPTEXT (not implemented)";//UNDONE: Whol.ConsoleUI.Program.HelpText is not implemented
        }

        static void Main(/*string[] args*/)
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
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            //_time = _services.GetRequiredService<ITime>();
            _eventController = _services.GetRequiredService<IEventController>();
            _holiday = _services.GetRequiredService<IHolidayController>();
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            if (_running)
                Stop();
            Timer.Dispose();
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
                    if (input == "?")
                    {
                        Console.WriteLine(HelpText());
                    }
                    else
                    {
                        Start(input);
                        _running = true;
                    }
                }
            }
        }

        private static void Start(string task)
        {
            _eventController.StartWork(task);

            var blank = new string(' ', Console.WindowWidth - 1) + "\r";
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            Console.Write(blank);
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            Console.Write(blank);
            Console.WriteLine(task);
        }
        private static void Stop()
        {
            _eventController.StopWork();
            Console.WriteLine(StartText());
        }
    }
}
