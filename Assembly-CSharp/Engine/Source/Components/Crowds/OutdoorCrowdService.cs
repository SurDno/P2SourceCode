using Engine.Common.Components.Movable;
using Engine.Source.Services;
using Engine.Source.Settings.External;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Components.Crowds
{
  [GameService(typeof (OutdoorCrowdService))]
  public class OutdoorCrowdService
  {
    [Inspected]
    private int entityCount;
    [Inspected]
    private int limitEntityCount;

    [Inspected]
    private int MaxLimitEntityCount => ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.MaxOutdoorCrowdEntityCount;

    private bool IsLimit(AreaEnum area)
    {
      return ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.LimitOutdoorAreaCrowdEntity.Contains(area);
    }

    private int GetMaxEntities(AreaEnum area)
    {
      return IsLimit(area) ? MaxLimitEntityCount : int.MaxValue;
    }

    public bool CanCreateEntity(AreaEnum area) => limitEntityCount < GetMaxEntities(area);

    public void OnCreateEntity(AreaEnum area)
    {
      if (!CanCreateEntity(area))
        Debug.LogError("Error crowd entity count, context : Outdoor");
      if (IsLimit(area))
        ++limitEntityCount;
      else
        ++entityCount;
    }

    public void OnDestroyEntity(AreaEnum area)
    {
      if (IsLimit(area))
        --limitEntityCount;
      else
        --entityCount;
    }
  }
}
