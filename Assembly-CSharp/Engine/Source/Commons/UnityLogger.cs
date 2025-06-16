// Decompiled with JetBrains decompiler
// Type: Engine.Source.Commons.UnityLogger
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Loggers;
using System;
using System.Text;
using UnityEngine;

#nullable disable
namespace Engine.Source.Commons
{
  public class UnityLogger : Cofe.Loggers.ILogger, ILogHandler
  {
    private static ILogHandler defaultHandler;
    private static UnityLogger instance;
    [ThreadStatic]
    private static StringBuilder tmp;

    public static void Initialize()
    {
      if (UnityLogger.instance != null)
        return;
      UnityLogger.instance = new UnityLogger();
      if (true)
      {
        UnityLogger.defaultHandler = Debug.unityLogger.logHandler;
        Debug.unityLogger.logHandler = (ILogHandler) UnityLogger.instance;
      }
    }

    public static Cofe.Loggers.ILogger Instance => (Cofe.Loggers.ILogger) UnityLogger.instance;

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
      UnityLogger.defaultHandler.LogException(exception, context);
    }

    public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
    {
      if (UnityLogger.tmp == null)
        UnityLogger.tmp = new StringBuilder(2048);
      UnityLogger.tmp.Clear();
      switch (logType)
      {
        case LogType.Error:
          UnityLogger.tmp.Append("ERROR | ");
          break;
        case LogType.Assert:
          UnityLogger.tmp.Append("ASSERT | ");
          break;
        case LogType.Warning:
          UnityLogger.tmp.Append("WARNING | ");
          break;
        case LogType.Exception:
          UnityLogger.tmp.Append("EXCEPTION | ");
          break;
        default:
          UnityLogger.tmp.Append("INFO | ");
          break;
      }
      DateTime now = DateTime.Now;
      int hour = now.Hour;
      if (hour < 10)
        UnityLogger.tmp.Append("0");
      UnityLogger.tmp.Append(hour);
      UnityLogger.tmp.Append(":");
      int minute = now.Minute;
      if (minute < 10)
        UnityLogger.tmp.Append("0");
      UnityLogger.tmp.Append(minute);
      UnityLogger.tmp.Append(":");
      int second = now.Second;
      if (second < 10)
        UnityLogger.tmp.Append("0");
      UnityLogger.tmp.Append(second);
      UnityLogger.tmp.Append(" | ");
      int frameCount = EngineApplication.FrameCount;
      UnityLogger.tmp.Append(frameCount);
      UnityLogger.tmp.Append(" | ");
      UnityLogger.tmp.Append(format);
      UnityLogger.defaultHandler.LogFormat(logType, context, UnityLogger.tmp.ToString(), args);
    }
  }
}
