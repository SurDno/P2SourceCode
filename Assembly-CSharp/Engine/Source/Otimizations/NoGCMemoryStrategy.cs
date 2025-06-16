using System.Collections;
using AssetDatabases;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Services;

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
          yield return null;
        SceneController.Disabled = true;
        yield return (object) Resources.UnloadUnusedAssets();
        SceneController.Disabled = false;
        MemoryStrategyService.ResetTime();
        GarbageCollector.GCMode = GarbageCollector.Mode.Enabled;
        yield return null;
        OptimizationUtility.ForceCollect();
        yield return null;
        GarbageCollector.GCMode = GarbageCollector.Mode.Disabled;
      }
    }
  }
}
