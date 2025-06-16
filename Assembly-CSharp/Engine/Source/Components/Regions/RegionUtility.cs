// Decompiled with JetBrains decompiler
// Type: Engine.Source.Components.Regions.RegionUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Components;
using Engine.Common.Components.Regions;
using Engine.Common.Services;
using Engine.Common.Types;
using Engine.Impl.Services;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace Engine.Source.Components.Regions
{
  public static class RegionUtility
  {
    private static Dictionary<RegionEnum, RegionComponent> regions = new Dictionary<RegionEnum, RegionComponent>();

    public static RegionComponent GetRegionByName(RegionEnum name)
    {
      RegionComponent regionByName;
      RegionUtility.regions.TryGetValue(name, out regionByName);
      return regionByName;
    }

    public static RegionComponent GetRegionByPosition(Vector3 position)
    {
      RegionEnum name = RegionLocator.GetRegionName(position);
      if (name == RegionEnum.None)
        name = ScriptableObjectInstance<GameSettingsData>.Instance.DefaultRegionName;
      RegionComponent regionByName = RegionUtility.GetRegionByName(name);
      if (regionByName == null)
      {
        Debug.LogWarning((object) ("Region not found, name = " + (object) name + " , position : " + (object) position));
        regionByName = RegionUtility.GetRegionByName(ScriptableObjectInstance<GameSettingsData>.Instance.DefaultRegionName);
      }
      return regionByName;
    }

    public static string GetRegionTitle(IRegionComponent region)
    {
      string regionTitle = string.Empty;
      IMapItemComponent component = region.GetComponent<IMapItemComponent>();
      if (component != null)
      {
        LocalizedText title = component.Title;
        if (title != LocalizedText.Empty)
          regionTitle = ServiceLocator.GetService<LocalizationService>().GetText(title);
      }
      if (string.IsNullOrEmpty(regionTitle))
        regionTitle = region.Region.ToString();
      return regionTitle;
    }

    public static void AddRegion(RegionEnum name, RegionComponent region)
    {
      if (name == RegionEnum.None)
        Debug.LogError((object) ("Region type : " + (object) name + " , region : " + region.Owner.GetInfo()));
      if (RegionUtility.regions.ContainsKey(name))
        Debug.LogError((object) ("Region type : " + (object) name + " , already exist, current : " + RegionUtility.regions[name].Owner.GetInfo() + " , new : " + region.Owner.GetInfo()));
      else
        RegionUtility.regions.Add(name, region);
    }

    public static void RemoveRegion(RegionEnum name, RegionComponent region)
    {
      RegionUtility.regions.Remove(name);
    }
  }
}
