using Engine.Behaviours.Components;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Components;
using Inspectors;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Engine.Source.Commons.Abilities.Projectiles
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class FlameAbilityProjectile : IAbilityProjectile
  {
    [DataReadProxy(MemberEnum.None, Name = "EnemyHitRadius")]
    [DataWriteProxy(MemberEnum.None, Name = "EnemyHitRadius")]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    protected float radius = 6f;
    [DataReadProxy(MemberEnum.None, Name = "EnemyHitAngle")]
    [DataWriteProxy(MemberEnum.None, Name = "EnemyHitAngle")]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    protected float hitAngle = 10f;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    protected float flameEffectiveTime = 0.5f;

    public void ComputeTargets(IEntity self, IEntity item, OutsideAbilityTargets targets)
    {
      PivotSanitar component1 = ((IEntityView) self).GameObject.GetComponent<PivotSanitar>();
      if ((UnityEngine.Object) component1 == (UnityEngine.Object) null)
        return;
      targets.Targets = component1.Targets.Select<IEntity, EffectsComponent>((Func<IEntity, EffectsComponent>) (o => o.GetComponent<EffectsComponent>())).Where<EffectsComponent>((Func<EffectsComponent, bool>) (o => o != null)).ToList<EffectsComponent>();
      if (component1.Flamethrower && (UnityEngine.Object) component1.TargetObject != (UnityEngine.Object) null && (double) component1.AimingTime > (double) this.flameEffectiveTime)
      {
        EngineGameObject component2 = component1.TargetObject.GetComponent<EngineGameObject>();
        EffectsComponent component3 = component2?.Owner?.GetComponent<EffectsComponent>();
        if (component3 != null && !targets.Targets.Contains(component3))
        {
          Vector3 forward = ((IEntityView) self).GameObject.transform.forward;
          Vector3 to = component1.TargetObject.position - ((IEntityView) self).GameObject.transform.position;
          if ((double) Mathf.Abs(Vector3.Angle(forward, to)) < (double) this.hitAngle && (double) to.magnitude < (double) this.radius)
          {
            DetectorComponent component4 = self?.GetComponent<DetectorComponent>();
            if (component4 == null)
            {
              targets.Targets.Add(component3);
            }
            else
            {
              DetectableComponent component5 = component2?.Owner?.GetComponent<DetectableComponent>();
              if (component5 == null || component4.Visible.Contains((IDetectableComponent) component5))
                targets.Targets.Add(component3);
            }
          }
        }
      }
      targets.Targets = this.FilterIgnoredTargets(targets.Targets);
    }

    private List<EffectsComponent> FilterIgnoredTargets(List<EffectsComponent> list)
    {
      List<EffectsComponent> effectsComponentList = new List<EffectsComponent>();
      foreach (EffectsComponent effectsComponent in list)
      {
        IParameter<bool> byName = effectsComponent.Owner.GetComponent<ParametersComponent>()?.GetByName<bool>(ParameterNameEnum.IsCombatIgnored);
        if (byName == null || !byName.Value)
          effectsComponentList.Add(effectsComponent);
      }
      return effectsComponentList;
    }
  }
}
