// Decompiled with JetBrains decompiler
// Type: Engine.Common.Threads.ThreadState`2
// Assembly: Engine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3568A167-18A7-4983-8BC2-C25824901591
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Engine.Common.dll

using System;

#nullable disable
namespace Engine.Common.Threads
{
  public class ThreadState<T, T2>
  {
    public System.Collections.Generic.Queue<T> Queue = new System.Collections.Generic.Queue<T>();
    public int CountThreads;
    public Action<T, ThreadState<T, T2>> Method;
    public T2 Context;
  }
}
