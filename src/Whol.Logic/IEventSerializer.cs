using System;
using System.Collections.Generic;
using System.Text;

namespace Whol.Logic
{
    public interface IEventSerializer
    {
        string Serialize(Event @event);
        Event Deserialize(string @event);
    }
    public class OneLineEventSerializer : IEventSerializer
    {
        public string Serialize(Event @event)
        {
            return $"{@event.Time:yyyy-MM-dd HH:mm:ss.fffff}\t{@event.EventType}\t{@event.Task}";
        }

        public Event Deserialize(string @event)
        {
            var fields = @event.Split('\t');
            return new Event
            {
                Time = DateTime.Parse(fields[0]),
                EventType = Enum.Parse<EventType>(fields[1], true),
                Task = fields[2]
            };
        }
    }
}
