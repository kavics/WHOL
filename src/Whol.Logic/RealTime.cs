using System;

namespace Whol.Logic
{
    public class RealTime : ITime
    {
        public DateTime Now => DateTime.UtcNow;
        public DateTime Today => DateTime.Today;
    }
}
