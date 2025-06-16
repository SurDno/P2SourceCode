using System;

namespace TriangleNet.Log
{
  public interface ILogItem
  {
    DateTime Time { get; }

    LogLevel Level { get; }

    string Message { get; }

    string Info { get; }
  }
}
