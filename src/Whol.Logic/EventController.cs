using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Whol.Logic
{
    public class EventController : IEventController
    {
        private List<Event> _events;

        private TimeSpan _todayClosedWorkTime;
        private DateTime _lastStart;

        private readonly ITime _time;
        private readonly IStorage _storage;
        private readonly IUserManager _userManager;
        private readonly User _user;

        public EventController(ITime time, IStorage storage, IUserManager userManager, User user)
        {
            _time = time;
            _storage = storage;
            _userManager = userManager;
            _user = user;
            Initialize();
        }
        private void Initialize()
        {
            var events = _storage.LoadEvents().ToList();

            var lastStart = DateTime.MinValue;
            var closedTime = TimeSpan.Zero;
            var isWorking = false;

            foreach (var @event in events)
            {
                if (@event.Time < DateTime.Today)
                    continue;

                switch (@event.EventType)
                {
                    case EventType.Start:
                        lastStart = @event.Time;
                        isWorking = true;
                        break;
                    case EventType.Stop:
                        closedTime += @event.Time - lastStart;
                        isWorking = false;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            _events = events;
            _todayClosedWorkTime = closedTime;
            _lastStart = lastStart;
            IsWorking = isWorking;
        }

        public bool IsWorking { get; private set; }

        public void StartWork()
        {
            _lastStart = _time.Now;
            IsWorking = true;
            _events.Add(new Event{Time = _time.Now, EventType = EventType.Start});
            _storage.SaveEvents(_events);
        }

        public void StopWork()
        {
            _todayClosedWorkTime += _time.Now - _lastStart;
            IsWorking = false;
            _events.Add(new Event { Time = _time.Now, EventType = EventType.Stop });
            _storage.SaveEvents(_events);
        }

        public TimeSpan GetTodayWorkTime()
        {
            return this.IsWorking
                ? this._todayClosedWorkTime + (this._time.Now - this._lastStart)
                : this._todayClosedWorkTime;
        }

    }
}
