using System;
using System.Collections.Generic;
using System.Runtime;
using System.Text;
using UnityEngine;
using UnityEngine.Profiling;

namespace Engine.Source.Services;

public static class OptimizationUtility {
	public static StringBuilder GetMemoryText(this StringBuilder info, long memory) {
		var num = (memory >> 10) / 1024L;
		info.Append(num);
		info.Append(" mb");
		return info;
	}

	public static string GetMemoryText(long memory) {
		return (memory >> 10) / 1024L + " mb";
	}

	public static void Alloc(long need) {
		Debug.Log(ObjectInfoUtility.GetStream().Append("Try allock memory : ").GetMemoryText(need).Append(" , used : ")
			.GetMemoryText(Profiler.GetMonoUsedSizeLong()).Append(" , max : ")
			.GetMemoryText(Profiler.GetMonoHeapSizeLong()));
		try {
			long length = 1048576;
			var capacity = need / length + 1L;
			var numArrayList = new List<int[]>((int)capacity);
			long num1 = 100;
			long num2 = 0;
			var num3 = capacity * 4L;
			while (true) {
				var numArray = new int[length];
				numArrayList.Add(numArray);
				num3 += length * 4L;
				if (num3 < need) {
					++num2;
					if (num2 >= num1) {
						num2 = 0L;
						Debug.Log(ObjectInfoUtility.GetStream().Append("Dump memory, used : ")
							.GetMemoryText(Profiler.GetMonoUsedSizeLong()).Append(" , max : ")
							.GetMemoryText(Profiler.GetMonoHeapSizeLong()));
					}
				} else
					break;
			}

			for (var index = 0; index < numArrayList.Count; ++index)
				numArrayList[index] = null;
			numArrayList.Clear();
			numArrayList.Capacity = 0;
		} catch (Exception ex) {
			Debug.LogException(ex);
		}

		ForceCollect();
		Debug.Log(ObjectInfoUtility.GetStream().Append("Result allock memory, used : ")
			.GetMemoryText(Profiler.GetMonoUsedSizeLong()).Append(" , max : ")
			.GetMemoryText(Profiler.GetMonoHeapSizeLong()));
	}

	public static void ForceCollect() {
		GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
		GC.Collect(2, GCCollectionMode.Forced, true, true);
	}

	public static StringBuilder DumpMemory(this StringBuilder info) {
		info.Append("Dump memory :\n").Append("    usedHeapSizeLong : ").GetMemoryText(Profiler.usedHeapSizeLong)
			.Append("\n").Append("    GetAllocatedMemoryForGraphicsDriver() : ")
			.GetMemoryText(Profiler.GetAllocatedMemoryForGraphicsDriver()).Append("\n")
			.Append("    GetMonoHeapSizeLong() : ").GetMemoryText(Profiler.GetMonoHeapSizeLong()).Append("\n")
			.Append("    GetMonoUsedSizeLong() : ").GetMemoryText(Profiler.GetMonoUsedSizeLong()).Append("\n")
			.Append("    GetTempAllocatorSize() : ").GetMemoryText(Profiler.GetTempAllocatorSize()).Append("\n")
			.Append("    GetTotalAllocatedMemoryLong() : ").GetMemoryText(Profiler.GetTotalAllocatedMemoryLong())
			.Append("\n").Append("    GetTotalReservedMemoryLong() : ")
			.GetMemoryText(Profiler.GetTotalReservedMemoryLong()).Append("\n")
			.Append("    GetTotalUnusedReservedMemoryLong() : ")
			.GetMemoryText(Profiler.GetTotalUnusedReservedMemoryLong()).Append("\n");
		return info;
	}

	public static string DumpMemory() {
		return ObjectInfoUtility.GetStream().Clear().DumpMemory().ToString();
	}
}