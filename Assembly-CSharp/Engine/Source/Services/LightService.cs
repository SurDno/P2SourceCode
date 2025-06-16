using System.Collections.Generic;
using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components;

namespace Engine.Source.Services
{
  [GameService(typeof (LightService))]
  public class LightService : IInitialisable, IUpdatable
  {
    private List<LightServiceObject> lights = new List<LightServiceObject>();
    private float checkLightsInterval = 0.5f;
    private float timeToNextCheck;
    private bool playerIsLighted;

    public bool PlayerIsLighted => playerIsLighted;

    public void Initialise()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable(this);
    }

    public void Terminate()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable(this);
    }

    public void RegisterLight(LightServiceObject light, bool enable)
    {
      if (enable && !lights.Contains(light))
        lights.Add(light);
      if (enable || !lights.Contains(light))
        return;
      lights.Remove(light);
    }

    public void ComputeUpdate()
    {
      timeToNextCheck -= Time.deltaTime;
      if (timeToNextCheck >= 0.0)
        return;
      timeToNextCheck = checkLightsInterval;
      UpdateLights();
    }

    private void UpdateLights()
    {
      playerIsLighted = false;
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
      foreach (LightServiceObject light in lights)
      {
        Vector3 position2 = light.transform.position with
        {
          y = 0.0f
        };
        float visibilityRadius = light.VisibilityRadius;
        if ((double) Mathf.Abs(position2.x - position1.x) <= visibilityRadius && (double) Mathf.Abs(position2.z - position1.z) <= visibilityRadius && (double) (position2 - position1).magnitude <= visibilityRadius)
        {
          playerIsLighted = true;
          break;
        }
      }
    }
  }
}
