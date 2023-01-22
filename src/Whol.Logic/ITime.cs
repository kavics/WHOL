using System;

namespace Whol.Logic;

public interface ITime
{
    DateTime Now { get; }
    DateTime Today { get; }
}