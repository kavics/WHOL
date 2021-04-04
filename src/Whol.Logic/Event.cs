using System;

namespace Whol.Logic
{
    public enum EventType { Start, Stop }

    public class Event
    {
        public DateTime Time { get; set; }      // UTC!
        public EventType EventType { get; set; }
        public string Task { get; set; }
    }
}
