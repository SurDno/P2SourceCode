using Engine.Common;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Components;
using Inspectors;
using System.Collections.Generic;
using UnityEngine;

namespace Engine.Source.Commons.Abilities.Projectiles
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class CloseCombatAbilityProjectile : IAbilityProjectile
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    protected BlockTypeEnum blocked = BlockTypeEnum.None;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    protected HitOrientationTypeEnum orientation = HitOrientationTypeEnum.None;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    protected float radius = 1.8f;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    protected float angle = 90f;

    public void ComputeTargets(IEntity self, IEntity item, OutsideAbilityTargets targets)
    {
      targets.Targets = new List<EffectsComponent>();
      GameObject gameObject = ((IEntityView) self).GameObject;
      if ((Object) gameObject == (Object) null)
        return;
      EnemyBase component1 = gameObject.GetComponent<EnemyBase>();
      if ((Object) component1 == (Object) null)
        return;
      EnemyBase enemy = component1.Enemy;
      if ((Object) enemy == (Object) null)
        return;
      EngineGameObject component2 = enemy.gameObject.GetComponent<EngineGameObject>();
      if ((Object) component2 == (Object) null)
        return;
      IEntity owner = component2.Owner;
      if (owner == null || !this.CheckBlocked(owner) || !this.CheckHitOrientation(gameObject, enemy.gameObject))
        return;
      EffectsComponent component3 = owner.GetComponent<EffectsComponent>();
      if (component3 == null || (double) (enemy.transform.position - component1.transform.position).magnitude > (double) this.radius)
        return;
      Vector3 direction = enemy.transform.position - component1.transform.position;
      Vector3 vector3 = component1.transform.InverseTransformDirection(direction);
      if ((double) vector3.z > (double) this.radius || (double) vector3.z < 0.0 || (double) Mathf.Abs(vector3.x) > 0.40000000596046448)
        return;
      targets.Targets.Add(component3);
    }

    private bool CheckBlocked(IEntity target)
    {
      if (this.blocked == BlockTypeEnum.None)
        return true;
      ParametersComponent component = target.GetComponent<ParametersComponent>();
      if (component != null)
      {
        IParameter<BlockTypeEnum> byName = component.GetByName<BlockTypeEnum>(ParameterNameEnum.BlockType);
        if (byName != null)
          return this.blocked == byName.Value;
      }
      return true;
    }

    private bool CheckHitOrientation(GameObject gameObject, GameObject target)
    {
      if (this.orientation == HitOrientationTypeEnum.None)
        return true;
      if ((Object) gameObject == (Object) null || (Object) target == (Object) null)
        return false;
      bool flag = (double) Vector3.Dot((gameObject.transform.position - target.transform.position).normalized, -target.transform.forward) > 0.0;
      if (this.orientation == HitOrientationTypeEnum.Back)
        return flag;
      return this.orientation != HitOrientationTypeEnum.Front || !flag;
    }
  }
}
