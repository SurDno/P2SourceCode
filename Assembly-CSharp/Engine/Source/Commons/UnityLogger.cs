using System;
using System.Text;
using Cofe.Loggers;

namespace Engine.Source.Commons
{
  public class UnityLogger : ILogger, ILogHandler
  {
    private static ILogHandler defaultHandler;
    private static UnityLogger instance;
    [ThreadStatic]
    private static StringBuilder tmp;

    public static void Initialize()
    {
      if (instance != null)
        return;
      instance = new UnityLogger();
      if (true)
      {
        defaultHandler = Debug.unityLogger.logHandler;
        Debug.unityLogger.logHandler = (ILogHandler) instance;
      }
    }

    public static ILogger Instance => instance;

    public void AddLog(LoggerType type, string text)
    {
      if (type == LoggerType.Warning)
        Debug.LogWarning((object) text);
      else if (type == LoggerType.Error)
        Debug.LogError((object) text);
      else
        Debug.Log((object) text);
    }

    public void LogException(Exception exception, UnityEngine.Object context)
    {
      defaultHandler.LogException(exception, context);
    }

    public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
    {
      if (tmp == null)
        tmp = new StringBuilder(2048);
      tmp.Clear();
      switch (logType)
      {
        case LogType.Error:
          tmp.Append("ERROR | ");
          break;
        case LogType.Assert:
          tmp.Append("ASSERT | ");
          break;
        case LogType.Warning:
          tmp.Append("WARNING | ");
          break;
        case LogType.Exception:
          tmp.Append("EXCEPTION | ");
          break;
        default:
          tmp.Append("INFO | ");
          break;
      }
      DateTime now = DateTime.Now;
      int hour = now.Hour;
      if (hour < 10)
        tmp.Append("0");
      tmp.Append(hour);
      tmp.Append(":");
      int minute = now.Minute;
      if (minute < 10)
        tmp.Append("0");
      tmp.Append(minute);
      tmp.Append(":");
      int second = now.Second;
      if (second < 10)
        tmp.Append("0");
      tmp.Append(second);
      tmp.Append(" | ");
      int frameCount = EngineApplication.FrameCount;
      tmp.Append(frameCount);
      tmp.Append(" | ");
      tmp.Append(format);
      defaultHandler.LogFormat(logType, context, tmp.ToString(), args);
    }
  }
}
