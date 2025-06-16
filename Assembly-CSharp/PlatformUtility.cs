// Decompiled with JetBrains decompiler
// Type: PlatformUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Components;
using Engine.Source.Settings.External;

#nullable disable
public static class PlatformUtility
{
  public static string GetPath(string fileName) => fileName;

  public static int StrategyIndex
  {
    get
    {
      return !ScriptableObjectInstance<BuildData>.Instance.Release ? ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.DevelopmentStrategyIndex : ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.ReleaseStrategyIndex;
    }
  }

  public static bool IsChangeLocationLoadingWindow(IRegionComponent region)
  {
    return ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.ChangeLocationLoadingWindow;
  }
}
