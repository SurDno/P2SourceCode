using AssetDatabases;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Services;
using System.Collections;
using UnityEngine;
using UnityEngine.Scripting;

namespace Engine.Source.Otimizations
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class NoGCMemoryStrategy : IMemoryStrategy
  {
    public IEnumerator Compute(MemoryStrategyContextEnum context)
    {
      if (context == MemoryStrategyContextEnum.ApplicationStart)
        GarbageCollector.GCMode = GarbageCollector.Mode.Disabled;
      else if (context == MemoryStrategyContextEnum.StartGame || context == MemoryStrategyContextEnum.EntryToIndoor || context == MemoryStrategyContextEnum.ChangeLocation || context == MemoryStrategyContextEnum.Time)
      {
        while (!SceneController.CanLoad)
          yield return (object) null;
        SceneController.Disabled = true;
        yield return (object) Resources.UnloadUnusedAssets();
        SceneController.Disabled = false;
        MemoryStrategyService.ResetTime();
        GarbageCollector.GCMode = GarbageCollector.Mode.Enabled;
        yield return (object) null;
        OptimizationUtility.ForceCollect();
        yield return (object) null;
        GarbageCollector.GCMode = GarbageCollector.Mode.Disabled;
      }
    }
  }
}
