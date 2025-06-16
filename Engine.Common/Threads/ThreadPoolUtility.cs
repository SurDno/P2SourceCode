using System;
using System.Collections.Generic;
using System.Threading;
using Cofe.Loggers;

namespace Engine.Common.Threads;

public static class ThreadPoolUtility {
	public static void Compute<T, T2>(
		Action<T, ThreadState<T, T2>> method,
		IEnumerable<T> data,
		int threadCount,
		T2 context) {
		var state = BeginCompute(method, data, threadCount, context);
		Worker(state);
		Wait(state);
	}

	public static ThreadState<T, T2> BeginCompute<T, T2>(
		Action<T, ThreadState<T, T2>> method,
		IEnumerable<T> data,
		int threadCount,
		T2 context) {
		var state = new ThreadState<T, T2>();
		foreach (var obj in data)
			state.Queue.Enqueue(obj);
		state.Method = method;
		state.Context = context;
		for (var index = 0; index < threadCount; ++index)
			if (!ThreadPool.QueueUserWorkItem(Worker<T, T2>, state))
				Logger.AddError("ThreadPool error!");
		return state;
	}

	public static void Wait<T, T2>(ThreadState<T, T2> state) {
		label_0:
		lock (state) {
			if (state.CountThreads != 0)
				goto label_0;
		}
	}

	public static void Worker<T, T2>(ThreadState<T, T2> state) {
		Worker<T, T2>((object)state);
	}

	private static void Worker<T, T2>(object value) {
		var threadState = (ThreadState<T, T2>)value;
		lock (threadState) {
			++threadState.CountThreads;
		}

		while (true) {
			T obj;
			lock (threadState) {
				if (threadState.Queue.Count != 0)
					obj = threadState.Queue.Dequeue();
				else
					break;
			}

			try {
				threadState.Method(obj, threadState);
			} catch (Exception ex) {
				Logger.AddError(ex.ToString());
			}
		}

		lock (threadState) {
			--threadState.CountThreads;
		}
	}
}