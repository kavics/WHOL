using System;
using System.Diagnostics.CodeAnalysis;

namespace Whol.Logic.Tests.Implementations;

/// <summary>
/// ITime stub
/// </summary>
[ExcludeFromCodeCoverage]
class TestTime : ITime
{
    public DateTime Now { get; set; }
    public DateTime Today { get; set; }
}