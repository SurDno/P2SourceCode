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
        if (instance == null)
        {
          int strategyIndex = PlatformUtility.StrategyIndex;
          if (strategyIndex >= 0 && strategyIndex < ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.MemoryStrategies.Count)
            instance = ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.MemoryStrategies[strategyIndex];
          if (instance == null)
            instance = ServiceCache.Factory.Create<EmptyMemoryStrategy>();
          Debug.Log(ObjectInfoUtility.GetStream().Append("Current memory strategy : ").Append(TypeUtility.GetTypeName(instance.GetType())));
        }
        return instance;
      }
    }
  }
}
