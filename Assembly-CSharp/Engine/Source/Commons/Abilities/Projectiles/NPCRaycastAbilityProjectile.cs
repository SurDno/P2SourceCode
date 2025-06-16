using Engine.Behaviours.Components;
using Engine.Common;
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
  public class NPCRaycastAbilityProjectile : IAbilityProjectile
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    protected float hitDistance = 50f;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    protected int bulletsCount = 5;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    protected float minimumDirectionScatter = 1f;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    protected float maximumDirectionScatter = 10f;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    protected float maximumAimingDeltaAngle = 45f;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    protected float bulletScatter = 10f;
    private static List<RaycastHit> hits = new List<RaycastHit>();

    public void ComputeTargets(IEntity self, IEntity item, OutsideAbilityTargets targets)
    {
      GameObject gameObject1 = ((IEntityView) self).GameObject;
      if ((Object) gameObject1 == (Object) null)
        return;
      Vector3 origin = gameObject1.transform.position;
      Vector3 direction1 = gameObject1.transform.forward;
      gameObject1.GetComponent<Pivot>();
      NPCWeaponService component1 = gameObject1.GetComponent<NPCWeaponService>();
      if ((Object) component1 != (Object) null)
      {
        origin = component1.GetWeaponStartPoint();
        direction1 = component1.GetWeaponAimDirection();
      }
      NPCEnemy component2 = gameObject1.GetComponent<NPCEnemy>();
      if ((Object) component2 != (Object) null)
        direction1 = this.ScatterShotDirection(direction1, component2);
      LayerMask triggerInteractLayer = ScriptableObjectInstance<GameSettingsData>.Instance.TriggerInteractLayer;
      LayerMask ragdollLayer = ScriptableObjectInstance<GameSettingsData>.Instance.RagdollLayer;
      for (int index1 = 0; index1 < this.bulletsCount; ++index1)
      {
        Vector3 direction2 = this.ScatterSingleBulletDirection(direction1);
        PhysicsUtility.Raycast(NPCRaycastAbilityProjectile.hits, origin, direction2, this.hitDistance, -1 ^ (int) triggerInteractLayer ^ (int) ragdollLayer, QueryTriggerInteraction.Ignore);
        for (int index2 = 0; index2 < NPCRaycastAbilityProjectile.hits.Count; ++index2)
        {
          GameObject gameObject2 = NPCRaycastAbilityProjectile.hits[index2].collider.gameObject;
          if (!((Object) gameObject2 == (Object) gameObject1))
          {
            IEntity entity = EntityUtility.GetEntity(gameObject2);
            if (entity != null)
            {
              EffectsComponent component3 = entity.GetComponent<EffectsComponent>();
              if (component3 != null)
                targets.Targets.Add(component3);
              break;
            }
            break;
          }
        }
      }
    }

    private Vector3 ScatterShotDirection(Vector3 direction, NPCEnemy enemy)
    {
      float num = Mathf.Lerp(this.minimumDirectionScatter, this.maximumDirectionScatter, Mathf.Min(enemy.GetAimingRotationDelta() / this.maximumAimingDeltaAngle, 1f));
      return Quaternion.Euler(0.0f, (float) ((double) num / 2.0 - (double) Random.value * (double) num), 0.0f) * direction;
    }

    private Vector3 ScatterSingleBulletDirection(Vector3 direction)
    {
      float bulletScatter = this.bulletScatter;
      return Quaternion.Euler(0.0f, (float) ((double) bulletScatter / 2.0 - (double) Random.value * (double) bulletScatter), 0.0f) * direction;
    }
  }
}
