using System;

namespace Whol.Logic
{
    public enum WhEventType { Created, Start, Stop, Modify }

    public class WhEvent
    {
        public DateTime Time { get; set; }      // UTC!
        public string WhEventType { get; set; }
        public string Description { get; set; } // can be null
        public TimeSpan DeltaTime { get; set; } // can be 
    }
}
