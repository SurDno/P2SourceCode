using System.Collections.Generic;
using Engine.Common.Components;
using Engine.Common.Components.Regions;
using Engine.Common.Services;
using Engine.Common.Types;
using Engine.Impl.Services;
using UnityEngine;

namespace Engine.Source.Components.Regions
{
  public static class RegionUtility
  {
    private static Dictionary<RegionEnum, RegionComponent> regions = new();

    public static RegionComponent GetRegionByName(RegionEnum name)
    {
      regions.TryGetValue(name, out RegionComponent regionByName);
      return regionByName;
    }

    public static RegionComponent GetRegionByPosition(Vector3 position)
    {
      RegionEnum name = RegionLocator.GetRegionName(position);
      if (name == RegionEnum.None)
        name = ScriptableObjectInstance<GameSettingsData>.Instance.DefaultRegionName;
      RegionComponent regionByName = GetRegionByName(name);
      if (regionByName == null)
      {
        Debug.LogWarning("Region not found, name = " + name + " , position : " + position);
        regionByName = GetRegionByName(ScriptableObjectInstance<GameSettingsData>.Instance.DefaultRegionName);
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
        Debug.LogError("Region type : " + name + " , region : " + region.Owner.GetInfo());
      if (regions.ContainsKey(name))
        Debug.LogError("Region type : " + name + " , already exist, current : " + regions[name].Owner.GetInfo() + " , new : " + region.Owner.GetInfo());
      else
        regions.Add(name, region);
    }

    public static void RemoveRegion(RegionEnum name, RegionComponent region)
    {
      regions.Remove(name);
    }
  }
}
