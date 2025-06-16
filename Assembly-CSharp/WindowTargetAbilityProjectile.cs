// Decompiled with JetBrains decompiler
// Type: WindowTargetAbilityProjectile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons.Abilities;
using Engine.Source.Components;
using System.Collections.Generic;

#nullable disable
[Factory]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class WindowTargetAbilityProjectile : IAbilityProjectile
{
  public void ComputeTargets(IEntity self, IEntity item, OutsideAbilityTargets targets)
  {
    List<EffectsComponent> effectsComponentList = new List<EffectsComponent>();
    if (ServiceLocator.GetService<UIService>().Active is ITargetInventoryWindow)
      effectsComponentList.Add((ServiceLocator.GetService<UIService>().Active as ITargetInventoryWindow).GetUseTarget().GetComponent<EffectsComponent>());
    targets.Targets = effectsComponentList;
  }
}
