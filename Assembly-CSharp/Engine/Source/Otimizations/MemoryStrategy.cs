// Decompiled with JetBrains decompiler
// Type: Engine.Source.Otimizations.MemoryStrategy
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Utility;
using Engine.Common.Services;
using Engine.Source.Settings.External;
using UnityEngine;

#nullable disable
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
