using Cofe.Utility;
using Engine.Common.Services;
using Engine.Source.Settings.External;
using UnityEngine;

namespace Engine.Source.Otimizations
{
  public static class MemoryStrategy
  {
    private static IMemoryStrategy instance;

    public static IMemoryStrategy Instance
    {
      get
      {
        if (MemoryStrategy.instance == null)
        {
          int strategyIndex = PlatformUtility.StrategyIndex;
          if (strategyIndex >= 0 && strategyIndex < ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.MemoryStrategies.Count)
            MemoryStrategy.instance = ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.MemoryStrategies[strategyIndex];
          if (MemoryStrategy.instance == null)
            MemoryStrategy.instance = (IMemoryStrategy) ServiceCache.Factory.Create<EmptyMemoryStrategy>();
          Debug.Log((object) ObjectInfoUtility.GetStream().Append("Current memory strategy : ").Append(TypeUtility.GetTypeName(MemoryStrategy.instance.GetType())));
        }
        return MemoryStrategy.instance;
      }
    }
  }
}
