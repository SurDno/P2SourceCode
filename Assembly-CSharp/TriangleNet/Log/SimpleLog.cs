// Decompiled with JetBrains decompiler
// Type: TriangleNet.Log.SimpleLog
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;

#nullable disable
namespace TriangleNet.Log
{
  public sealed class SimpleLog : ILog<SimpleLogItem>
  {
    private static readonly SimpleLog instance = new SimpleLog();
    private LogLevel level = LogLevel.Info;
    private List<SimpleLogItem> log = new List<SimpleLogItem>();

    public static ILog<SimpleLogItem> Instance => (ILog<SimpleLogItem>) SimpleLog.instance;

    private SimpleLog()
    {
    }

    public void Add(SimpleLogItem item) => this.log.Add(item);

    public void Clear() => this.log.Clear();

    public void Info(string message) => this.log.Add(new SimpleLogItem(LogLevel.Info, message));

    public void Warning(string message, string location)
    {
      this.log.Add(new SimpleLogItem(LogLevel.Warning, message, location));
    }

    public void Error(string message, string location)
    {
      this.log.Add(new SimpleLogItem(LogLevel.Error, message, location));
    }

    public IList<SimpleLogItem> Data => (IList<SimpleLogItem>) this.log;

    public LogLevel Level => this.level;
  }
}
