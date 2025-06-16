// Decompiled with JetBrains decompiler
// Type: TriangleNet.Log.SimpleLogItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;

#nullable disable
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
