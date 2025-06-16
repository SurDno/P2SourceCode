using System;
using System.Collections.Generic;

namespace Engine.Common.Threads;

public class ThreadState<T, T2> {
	public Queue<T> Queue = new();
	public int CountThreads;
	public Action<T, ThreadState<T, T2>> Method;
	public T2 Context;
}