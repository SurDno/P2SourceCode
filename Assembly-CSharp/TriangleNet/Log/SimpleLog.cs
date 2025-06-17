using System.Collections.Generic;

namespace TriangleNet.Log
{
  public sealed class SimpleLog : ILog<SimpleLogItem>
  {
    private static readonly SimpleLog instance = new();
    private LogLevel level = LogLevel.Info;
    private List<SimpleLogItem> log = [];

    public static ILog<SimpleLogItem> Instance => instance;

    private SimpleLog()
    {
    }

    public void Add(SimpleLogItem item) => log.Add(item);

    public void Clear() => log.Clear();

    public void Info(string message) => log.Add(new SimpleLogItem(LogLevel.Info, message));

    public void Warning(string message, string location)
    {
      log.Add(new SimpleLogItem(LogLevel.Warning, message, location));
    }

    public void Error(string message, string location)
    {
      log.Add(new SimpleLogItem(LogLevel.Error, message, location));
    }

    public IList<SimpleLogItem> Data => log;

    public LogLevel Level => level;
  }
}
