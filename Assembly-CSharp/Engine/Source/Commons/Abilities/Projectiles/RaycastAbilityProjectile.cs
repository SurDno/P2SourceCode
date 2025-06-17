using System.Collections.Generic;
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
  public class RaycastAbilityProjectile : IAbilityProjectile
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    protected float hitDistance = 50f;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    protected int bulletsCount = 1;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    protected float minimumAimingDeltaAngle = 0.0f;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    protected float maximumAimingDeltaAngle = 3f;
    private static List<RaycastHit> hits = [];
    private string headTag = "Head";
    private string bodyTag = "Body";
    private string armTag = "Arm";
    private string legTag = "Leg";
    private string ignoredTag = "IgnoredBody";
    private string untagged = "Untagged";
    private List<ShotTargetBodyPartEnum> hitsList;

    public void ComputeTargets(IEntity self, IEntity item, OutsideAbilityTargets targets)
    {
      hitsList = [];
      GameObject gameObject1 = ((IEntityView) self).GameObject;
      if (gameObject1 == null)
        return;
      Vector3 forward = GameCamera.Instance.CameraTransform.forward;
      Vector3 position = GameCamera.Instance.CameraTransform.position;
      IParameter<float> parameter = null;
      if (item != null)
      {
        ParametersComponent component = item.GetComponent<ParametersComponent>();
        if (component != null)
          parameter = component.GetByName<float>(ParameterNameEnum.Durability);
      }
      for (int index1 = 0; index1 < bulletsCount; ++index1)
      {
        Vector3 direction = parameter == null ? forward : ScatterSingleBulletDirection(forward, parameter.Value);
        PhysicsUtility.Raycast(hits, position, direction, hitDistance, -1, QueryTriggerInteraction.Collide);
        for (int index2 = 0; index2 < hits.Count; ++index2)
        {
          GameObject gameObject2 = hits[index2].collider.gameObject;
          if (!(gameObject2 == gameObject1) && !(gameObject2.tag == ignoredTag) && (!hits[index2].collider.isTrigger || gameObject2.layer == ScriptableObjectInstance<GameSettingsData>.Instance.NpcHitCollidersLayer.GetIndex()))
          {
            ShotTargetBodyPartEnum targetBodyPartEnum = ShotTargetBodyPartEnum.Body;
            string tag = gameObject2.tag;
            if (tag == untagged)
            {
              Pivot component = gameObject2.GetComponent<Pivot>();
              if (component != null && component.CollidersSetForSharpshooting)
                continue;
            }
            else
            {
              Pivot component = gameObject2.GetComponent<Pivot>();
              if (tag == headTag || tag == bodyTag || tag == armTag || tag == legTag)
              {
                do
                {
                  if (component == null)
                  {
                    Transform parent = gameObject2.transform.parent;
                    if (!(parent == null))
                    {
                      gameObject2 = parent.gameObject;
                      component = gameObject2.GetComponent<Pivot>();
                    }
                    else
                      break;
                  }
                }
                while (component == null);
              }
              if (!(component == null))
              {
                if (component.CollidersSetForSharpshooting)
                {
                  if (tag == headTag)
                    targetBodyPartEnum = ShotTargetBodyPartEnum.Head;
                  if (tag == bodyTag)
                    targetBodyPartEnum = ShotTargetBodyPartEnum.Body;
                  if (tag == armTag)
                    targetBodyPartEnum = ShotTargetBodyPartEnum.Arm;
                  if (tag == legTag)
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
                hitsList.Add(targetBodyPartEnum);
              }
              break;
            }
            Debug.LogWarningFormat("{0} can't map entity.", gameObject2.name);
            break;
          }
        }
      }
    }

    public ShotTargetBodyPartEnum GetNextTargetBodyPart()
    {
      if (hitsList.Count <= 0)
        return ShotTargetBodyPartEnum.Body;
      ShotTargetBodyPartEnum hits = hitsList[0];
      hitsList.RemoveAt(0);
      return hits;
    }

    private Vector3 ScatterSingleBulletDirection(Vector3 direction, float durability)
    {
      float num = Random.Range(0.0f, minimumAimingDeltaAngle + (float) ((maximumAimingDeltaAngle - (double) minimumAimingDeltaAngle) * (1.0 - durability)));
      Vector2 normalized = Random.insideUnitCircle.normalized;
      return Quaternion.Euler(normalized.x * num, normalized.y * num, 0.0f) * direction;
    }
  }
}
