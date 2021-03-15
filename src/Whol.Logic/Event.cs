using System;

namespace Whol.Logic
{
    public enum EventType { Start, Stop }

    public class Event
    {
        public DateTime Time { get; set; }      // UTC!
        public EventType EventType { get; set; }
        public string Description { get; set; } // can be null
        public TimeSpan DeltaTime { get; set; } // can be 
    }
}
