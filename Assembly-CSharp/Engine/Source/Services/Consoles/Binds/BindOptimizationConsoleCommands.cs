// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.Consoles.Binds.BindOptimizationConsoleCommands
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Meta;
using UnityEngine;
using UnityHeapCrawler;

#nullable disable
namespace Engine.Source.Services.Consoles.Binds
{
  [Initialisable]
  public static class BindOptimizationConsoleCommands
  {
    [ConsoleCommand("gc_collect")]
    private static string GcCollectCommand(string command, ConsoleParameter[] parameters)
    {
      OptimizationUtility.ForceCollect();
      return command;
    }

    [ConsoleCommand("unload_unused_assets")]
    private static string UnloadUnusedAssetsCommand(string command, ConsoleParameter[] parameters)
    {
      Resources.UnloadUnusedAssets();
      return command;
    }

    [ConsoleCommand("dump_memory")]
    public static string DumpMemoryCommand(string command, ConsoleParameter[] parameters)
    {
      new HeapSnapshotCollector().Start();
      return command;
    }
  }
}
