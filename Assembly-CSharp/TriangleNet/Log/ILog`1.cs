// Decompiled with JetBrains decompiler
// Type: TriangleNet.Log.ILog`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;

#nullable disable
namespace TriangleNet.Log
{
  public interface ILog<T> where T : ILogItem
  {
    IList<T> Data { get; }

    LogLevel Level { get; }

    void Add(T item);

    void Clear();

    void Info(string message);

    void Error(string message, string info);

    void Warning(string message, string info);
  }
}
