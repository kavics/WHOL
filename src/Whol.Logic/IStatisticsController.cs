using System;
using System.Collections.Generic;

namespace Whol.Logic;

public class TaskSummary
{
    public string? Name { get; set; }
    public DateTime Start { get; set; }
    public DateTime Stop { get; set; }
    public TimeSpan TotalWorkTime { get; set; }
}

public interface IStatisticsController
{
    IEnumerable<TaskSummary> SummarizeTasks();
}