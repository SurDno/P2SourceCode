using Cofe.Meta;
using UnityEngine;
using UnityHeapCrawler;

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
