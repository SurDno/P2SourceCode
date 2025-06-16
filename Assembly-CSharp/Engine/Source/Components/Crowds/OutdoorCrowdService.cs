// Decompiled with JetBrains decompiler
// Type: Engine.Source.Components.Crowds.OutdoorCrowdService
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Components.Movable;
using Engine.Source.Services;
using Engine.Source.Settings.External;
using Inspectors;
using UnityEngine;

#nullable disable
namespace Engine.Source.Components.Crowds
{
  [GameService(new System.Type[] {typeof (OutdoorCrowdService)})]
  public class OutdoorCrowdService
  {
    [Inspected]
    private int entityCount;
    [Inspected]
    private int limitEntityCount;

    [Inspected]
    private int MaxLimitEntityCount
    {
      get
      {
        return ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.MaxOutdoorCrowdEntityCount;
      }
    }

    private bool IsLimit(AreaEnum area)
    {
      return ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.LimitOutdoorAreaCrowdEntity.Contains(area);
    }

    private int GetMaxEntities(AreaEnum area)
    {
      return this.IsLimit(area) ? this.MaxLimitEntityCount : int.MaxValue;
    }

    public bool CanCreateEntity(AreaEnum area) => this.limitEntityCount < this.GetMaxEntities(area);

    public void OnCreateEntity(AreaEnum area)
    {
      if (!this.CanCreateEntity(area))
        Debug.LogError((object) "Error crowd entity count, context : Outdoor");
      if (this.IsLimit(area))
        ++this.limitEntityCount;
      else
        ++this.entityCount;
    }

    public void OnDestroyEntity(AreaEnum area)
    {
      if (this.IsLimit(area))
        --this.limitEntityCount;
      else
        --this.entityCount;
    }
  }
}
