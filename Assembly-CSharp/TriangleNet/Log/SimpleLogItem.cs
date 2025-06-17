using System;

namespace TriangleNet.Log
{
  public class SimpleLogItem(LogLevel level, string message, string info) : ILogItem 
  {
    private DateTime time = DateTime.Now;

    public SimpleLogItem(LogLevel level, string message)
      : this(level, message, "")
    {
    }

    public DateTime Time => time;

    public LogLevel Level => level;

    public string Message => message;

    public string Info => info;
  }
}
