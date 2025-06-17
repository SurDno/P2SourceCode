using System.Collections.Generic;
using System.Linq;
using Engine.Behaviours.Components;
using Engine.Common;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Components;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Commons.Abilities.Projectiles
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class FlameAbilityProjectile : IAbilityProjectile
  {
    [DataReadProxy(Name = "EnemyHitRadius")]
    [DataWriteProxy(Name = "EnemyHitRadius")]
    [CopyableProxy]
    [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    protected float radius = 6f;
    [DataReadProxy(Name = "EnemyHitAngle")]
    [DataWriteProxy(Name = "EnemyHitAngle")]
    [CopyableProxy]
    [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    protected float hitAngle = 10f;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    protected float flameEffectiveTime = 0.5f;

    public void ComputeTargets(IEntity self, IEntity item, OutsideAbilityTargets targets)
    {
      PivotSanitar component1 = ((IEntityView) self).GameObject.GetComponent<PivotSanitar>();
      if (component1 == null)
        return;
      targets.Targets = component1.Targets.Select(o => o.GetComponent<EffectsComponent>()).Where(o => o != null).ToList();
      if (component1.Flamethrower && component1.TargetObject != null && component1.AimingTime > (double) flameEffectiveTime)
      {
        EngineGameObject component2 = component1.TargetObject.GetComponent<EngineGameObject>();
        EffectsComponent component3 = component2?.Owner?.GetComponent<EffectsComponent>();
        if (component3 != null && !targets.Targets.Contains(component3))
        {
          Vector3 forward = ((IEntityView) self).GameObject.transform.forward;
          Vector3 to = component1.TargetObject.position - ((IEntityView) self).GameObject.transform.position;
          if (Mathf.Abs(Vector3.Angle(forward, to)) < (double) hitAngle && to.magnitude < (double) radius)
          {
            DetectorComponent component4 = self?.GetComponent<DetectorComponent>();
            if (component4 == null)
            {
              targets.Targets.Add(component3);
            }
            else
            {
              DetectableComponent component5 = component2?.Owner?.GetComponent<DetectableComponent>();
              if (component5 == null || component4.Visible.Contains(component5))
                targets.Targets.Add(component3);
            }
          }
        }
      }
      targets.Targets = FilterIgnoredTargets(targets.Targets);
    }

    private List<EffectsComponent> FilterIgnoredTargets(List<EffectsComponent> list)
    {
      List<EffectsComponent> effectsComponentList = [];
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
