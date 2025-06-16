using System.Collections.Generic;
using Engine.Common;
using Engine.Common.Components.Parameters;
using Engine.Source.Components;
using Engine.Source.Settings.External;
using UnityEngine;

namespace Engine.Behaviours.Engines.Controllers
{
  public static class WeaponUtility
  {
    private static List<RaycastHit> hits = new List<RaycastHit>();

    public static bool ComputeGunJam(GameObject gameObject, IParameter<float> durability)
    {
      Vector3 forward = GameCamera.Instance.CameraTransform.forward;
      Vector3 position = GameCamera.Instance.CameraTransform.position;
      EffectsComponent component1 = null;
      if (ExternalSettingsInstance<ExternalCommonSettings>.Instance.ChildGunJam)
      {
        PhysicsUtility.Raycast(hits, position, forward, float.MaxValue, -1, QueryTriggerInteraction.Ignore);
        for (int index = 0; index < hits.Count; ++index)
        {
          GameObject gameObject1 = hits[index].collider.gameObject;
          if (!((Object) gameObject1 == (Object) gameObject))
          {
            IEntity entity = EntityUtility.GetEntity(gameObject1);
            if (entity != null)
            {
              EffectsComponent component2 = entity.GetComponent<EffectsComponent>();
              if (component2 != null)
              {
                component1 = component2;
              }
              break;
            }
            Debug.LogWarningFormat("{0} can't map entity.", (object) gameObject1.name);
            break;
          }
        }
      }
      if (component1 != null)
      {
        ParametersComponent component3 = component1.GetComponent<ParametersComponent>();
        if (component3 != null)
        {
          IParameter<bool> byName = component3.GetByName<bool>(ParameterNameEnum.IsCombatIgnored);
          if (byName != null && byName.Value)
            return true;
        }
      }
      return (double) Random.value < (durability == null ? 0.0 : (double) Mathf.Pow(1f - durability.Value, 2f));
    }
  }
}
