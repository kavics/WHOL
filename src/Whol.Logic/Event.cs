using System;
using System.Diagnostics;
using System.Globalization;

namespace Whol.Logic;

public enum EventType { Start, Stop }

[DebuggerDisplay("{ToString()}")]
public class Event
{
    public DateTime Time { get; set; }      // UTC!
    public EventType EventType { get; set; }
    public string? Task { get; set; }
    public override string ToString()
    {
        var t = Time.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.CurrentCulture);
        return $"{t} {EventType} {Task ?? "null"}";
    }
}