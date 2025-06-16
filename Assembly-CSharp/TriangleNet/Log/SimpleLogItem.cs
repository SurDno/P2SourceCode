using System;

namespace TriangleNet.Log
{
  public class SimpleLogItem : ILogItem
  {
    private string info;
    private LogLevel level;
    private string message;
    private DateTime time;

    public SimpleLogItem(LogLevel level, string message)
      : this(level, message, "")
    {
    }

    public SimpleLogItem(LogLevel level, string message, string info)
    {
      this.time = DateTime.Now;
      this.level = level;
      this.message = message;
      this.info = info;
    }

    public DateTime Time => this.time;

    public LogLevel Level => this.level;

    public string Message => this.message;

    public string Info => this.info;
  }
}
