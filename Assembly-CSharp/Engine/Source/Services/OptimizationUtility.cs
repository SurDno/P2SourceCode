// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.OptimizationUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Runtime;
using System.Text;
using UnityEngine;
using UnityEngine.Profiling;

#nullable disable
namespace Engine.Source.Services
{
  public static class OptimizationUtility
  {
    public static StringBuilder GetMemoryText(this StringBuilder info, long memory)
    {
      long num = (memory >> 10) / 1024L;
      info.Append(num);
      info.Append(" mb");
      return info;
    }

    public static string GetMemoryText(long memory) => ((memory >> 10) / 1024L).ToString() + " mb";

    public static void Alloc(long need)
    {
      Debug.Log((object) ObjectInfoUtility.GetStream().Append("Try allock memory : ").GetMemoryText(need).Append(" , used : ").GetMemoryText(Profiler.GetMonoUsedSizeLong()).Append(" , max : ").GetMemoryText(Profiler.GetMonoHeapSizeLong()));
      try
      {
        long length = 1048576;
        long capacity = need / length + 1L;
        List<int[]> numArrayList = new List<int[]>((int) capacity);
        long num1 = 100;
        long num2 = 0;
        long num3 = capacity * 4L;
        while (true)
        {
          int[] numArray = new int[length];
          numArrayList.Add(numArray);
          num3 += length * 4L;
          if (num3 < need)
          {
            ++num2;
            if (num2 >= num1)
            {
              num2 = 0L;
              Debug.Log((object) ObjectInfoUtility.GetStream().Append("Dump memory, used : ").GetMemoryText(Profiler.GetMonoUsedSizeLong()).Append(" , max : ").GetMemoryText(Profiler.GetMonoHeapSizeLong()));
            }
          }
          else
            break;
        }
        for (int index = 0; index < numArrayList.Count; ++index)
          numArrayList[index] = (int[]) null;
        numArrayList.Clear();
        numArrayList.Capacity = 0;
      }
      catch (Exception ex)
      {
        Debug.LogException(ex);
      }
      OptimizationUtility.ForceCollect();
      Debug.Log((object) ObjectInfoUtility.GetStream().Append("Result allock memory, used : ").GetMemoryText(Profiler.GetMonoUsedSizeLong()).Append(" , max : ").GetMemoryText(Profiler.GetMonoHeapSizeLong()));
    }

    public static void ForceCollect()
    {
      GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
      GC.Collect(2, GCCollectionMode.Forced, true, true);
    }

    public static StringBuilder DumpMemory(this StringBuilder info)
    {
      info.Append("Dump memory :\n").Append("    usedHeapSizeLong : ").GetMemoryText(Profiler.usedHeapSizeLong).Append("\n").Append("    GetAllocatedMemoryForGraphicsDriver() : ").GetMemoryText(Profiler.GetAllocatedMemoryForGraphicsDriver()).Append("\n").Append("    GetMonoHeapSizeLong() : ").GetMemoryText(Profiler.GetMonoHeapSizeLong()).Append("\n").Append("    GetMonoUsedSizeLong() : ").GetMemoryText(Profiler.GetMonoUsedSizeLong()).Append("\n").Append("    GetTempAllocatorSize() : ").GetMemoryText((long) Profiler.GetTempAllocatorSize()).Append("\n").Append("    GetTotalAllocatedMemoryLong() : ").GetMemoryText(Profiler.GetTotalAllocatedMemoryLong()).Append("\n").Append("    GetTotalReservedMemoryLong() : ").GetMemoryText(Profiler.GetTotalReservedMemoryLong()).Append("\n").Append("    GetTotalUnusedReservedMemoryLong() : ").GetMemoryText(Profiler.GetTotalUnusedReservedMemoryLong()).Append("\n");
      return info;
    }

    public static string DumpMemory()
    {
      return ObjectInfoUtility.GetStream().Clear().DumpMemory().ToString();
    }
  }
}
