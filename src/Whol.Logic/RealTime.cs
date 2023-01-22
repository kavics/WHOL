using System;

namespace Whol.Logic
{
    public class RealTime : ITime
    {
        public DateTime Now
        {
            get => DateTime.UtcNow;
            set => throw new NotSupportedException();
        }

        public DateTime Today => DateTime.Today;
    }
}
