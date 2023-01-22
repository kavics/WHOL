using System;
using System.Collections.Generic;

namespace Whol.Logic;

public class StatisticsController : IStatisticsController
{
    private IStorage _storage;

    public StatisticsController(IStorage storage)
    {
        _storage = storage;
    }

    public IEnumerable<TaskSummary> SummarizeTasks()
    {
        var summaries = new Dictionary<string, TaskSummary>();

        var events = _storage.LoadEvents();
        Event? startEvent = null;
        foreach (var @event in events)
        {
            switch (@event.EventType)
            {
                case EventType.Start:
                    startEvent = @event;
                    break;
                case EventType.Stop:
                    CalculateTaskPeriod(startEvent, @event.Time, summaries);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        return summaries.Values;
    }

    private void CalculateTaskPeriod(Event? startEvent, DateTime endTime, Dictionary<string, TaskSummary> summaries)
    {
        if (startEvent == null)
            return;

        var task = startEvent.Task ?? string.Empty;
        if (!summaries.TryGetValue(task, out var summary))
        {
            summary = new TaskSummary {Name = task, Start = startEvent.Time};
            summaries.Add(task, summary);
        }
        summary.Stop = endTime;
        summary.TotalWorkTime += endTime - startEvent.Time;
    }
}