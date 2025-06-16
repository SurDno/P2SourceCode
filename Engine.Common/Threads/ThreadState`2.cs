using System;

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
