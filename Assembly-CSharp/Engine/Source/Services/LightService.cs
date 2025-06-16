using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using System.Collections.Generic;
using UnityEngine;

namespace Engine.Source.Services
{
  [GameService(new System.Type[] {typeof (LightService)})]
  public class LightService : IInitialisable, IUpdatable
  {
    private List<LightServiceObject> lights = new List<LightServiceObject>();
    private float checkLightsInterval = 0.5f;
    private float timeToNextCheck = 0.0f;
    private bool playerIsLighted;

    public bool PlayerIsLighted => this.playerIsLighted;

    public void Initialise()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable((IUpdatable) this);
    }

    public void Terminate()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable((IUpdatable) this);
    }

    public void RegisterLight(LightServiceObject light, bool enable)
    {
      if (enable && !this.lights.Contains(light))
        this.lights.Add(light);
      if (enable || !this.lights.Contains(light))
        return;
      this.lights.Remove(light);
    }

    public void ComputeUpdate()
    {
      this.timeToNextCheck -= Time.deltaTime;
      if ((double) this.timeToNextCheck >= 0.0)
        return;
      this.timeToNextCheck = this.checkLightsInterval;
      this.UpdateLights();
    }

    private void UpdateLights()
    {
      this.playerIsLighted = false;
      IEntity player = ServiceLocator.GetService<ISimulation>().Player;
      if (player == null)
        return;
      LocationItemComponent component = player.GetComponent<LocationItemComponent>();
      if (component == null || component.IsIndoor)
        return;
      GameObject gameObject = ((IEntityView) player)?.GameObject;
      if ((UnityEngine.Object) gameObject == (UnityEngine.Object) null)
        return;
      Vector3 position1 = gameObject.transform.position with
      {
        y = 0.0f
      };
      foreach (LightServiceObject light in this.lights)
      {
        Vector3 position2 = light.transform.position with
        {
          y = 0.0f
        };
        float visibilityRadius = light.VisibilityRadius;
        if ((double) Mathf.Abs(position2.x - position1.x) <= (double) visibilityRadius && (double) Mathf.Abs(position2.z - position1.z) <= (double) visibilityRadius && (double) (position2 - position1).magnitude <= (double) visibilityRadius)
        {
          this.playerIsLighted = true;
          break;
        }
      }
    }
  }
}
