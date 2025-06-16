// Decompiled with JetBrains decompiler
// Type: Engine.Source.Commons.Abilities.Projectiles.RaycastAbilityProjectile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Behaviours.Components;
using Engine.Common;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Components;
using Inspectors;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace Engine.Source.Commons.Abilities.Projectiles
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class RaycastAbilityProjectile : IAbilityProjectile
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
    protected int bulletsCount = 1;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    protected float minimumAimingDeltaAngle = 0.0f;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    protected float maximumAimingDeltaAngle = 3f;
    private static List<RaycastHit> hits = new List<RaycastHit>();
    private string headTag = "Head";
    private string bodyTag = "Body";
    private string armTag = "Arm";
    private string legTag = "Leg";
    private string ignoredTag = "IgnoredBody";
    private string untagged = "Untagged";
    private List<ShotTargetBodyPartEnum> hitsList;

    public void ComputeTargets(IEntity self, IEntity item, OutsideAbilityTargets targets)
    {
      this.hitsList = new List<ShotTargetBodyPartEnum>();
      GameObject gameObject1 = ((IEntityView) self).GameObject;
      if ((Object) gameObject1 == (Object) null)
        return;
      Vector3 forward = GameCamera.Instance.CameraTransform.forward;
      Vector3 position = GameCamera.Instance.CameraTransform.position;
      IParameter<float> parameter = (IParameter<float>) null;
      if (item != null)
      {
        ParametersComponent component = item.GetComponent<ParametersComponent>();
        if (component != null)
          parameter = component.GetByName<float>(ParameterNameEnum.Durability);
      }
      for (int index1 = 0; index1 < this.bulletsCount; ++index1)
      {
        Vector3 direction = parameter == null ? forward : this.ScatterSingleBulletDirection(forward, parameter.Value);
        PhysicsUtility.Raycast(RaycastAbilityProjectile.hits, position, direction, this.hitDistance, -1, QueryTriggerInteraction.Collide);
        for (int index2 = 0; index2 < RaycastAbilityProjectile.hits.Count; ++index2)
        {
          GameObject gameObject2 = RaycastAbilityProjectile.hits[index2].collider.gameObject;
          if (!((Object) gameObject2 == (Object) gameObject1) && !(gameObject2.tag == this.ignoredTag) && (!RaycastAbilityProjectile.hits[index2].collider.isTrigger || gameObject2.layer == ScriptableObjectInstance<GameSettingsData>.Instance.NpcHitCollidersLayer.GetIndex()))
          {
            ShotTargetBodyPartEnum targetBodyPartEnum = ShotTargetBodyPartEnum.Body;
            string tag = gameObject2.tag;
            if (tag == this.untagged)
            {
              Pivot component = gameObject2.GetComponent<Pivot>();
              if ((Object) component != (Object) null && component.CollidersSetForSharpshooting)
                continue;
            }
            else
            {
              Pivot component = gameObject2.GetComponent<Pivot>();
              if (tag == this.headTag || tag == this.bodyTag || tag == this.armTag || tag == this.legTag)
              {
                do
                {
                  if ((Object) component == (Object) null)
                  {
                    Transform parent = gameObject2.transform.parent;
                    if (!((Object) parent == (Object) null))
                    {
                      gameObject2 = parent.gameObject;
                      component = gameObject2.GetComponent<Pivot>();
                    }
                    else
                      break;
                  }
                }
                while ((Object) component == (Object) null);
              }
              if (!((Object) component == (Object) null))
              {
                if (component.CollidersSetForSharpshooting)
                {
                  if (tag == this.headTag)
                    targetBodyPartEnum = ShotTargetBodyPartEnum.Head;
                  if (tag == this.bodyTag)
                    targetBodyPartEnum = ShotTargetBodyPartEnum.Body;
                  if (tag == this.armTag)
                    targetBodyPartEnum = ShotTargetBodyPartEnum.Arm;
                  if (tag == this.legTag)
                    targetBodyPartEnum = ShotTargetBodyPartEnum.Leg;
                }
              }
              else
                break;
            }
            IEntity entity = EntityUtility.GetEntity(gameObject2);
            if (entity != null)
            {
              EffectsComponent component = entity.GetComponent<EffectsComponent>();
              if (component != null)
              {
                targets.Targets.Add(component);
                this.hitsList.Add(targetBodyPartEnum);
                break;
              }
              break;
            }
            Debug.LogWarningFormat("{0} can't map entity.", (object) gameObject2.name);
            break;
          }
        }
      }
    }

    public ShotTargetBodyPartEnum GetNextTargetBodyPart()
    {
      if (this.hitsList.Count <= 0)
        return ShotTargetBodyPartEnum.Body;
      ShotTargetBodyPartEnum hits = this.hitsList[0];
      this.hitsList.RemoveAt(0);
      return hits;
    }

    private Vector3 ScatterSingleBulletDirection(Vector3 direction, float durability)
    {
      float num = Random.Range(0.0f, this.minimumAimingDeltaAngle + (float) (((double) this.maximumAimingDeltaAngle - (double) this.minimumAimingDeltaAngle) * (1.0 - (double) durability)));
      Vector2 normalized = Random.insideUnitCircle.normalized;
      return Quaternion.Euler(normalized.x * num, normalized.y * num, 0.0f) * direction;
    }
  }
}
