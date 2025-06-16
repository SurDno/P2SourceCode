using Engine.Behaviours.Components;
using Engine.Common;
using Engine.Source.Commons;
using UnityEngine;

namespace Engine.Source.Services.CameraServices
{
  public class TrackingCameraController : ICameraController
  {
    public void Initialise()
    {
    }

    public void Shutdown()
    {
    }

    public void Update(IEntity target, GameObject gameObjectTarget)
    {
      if (target == null)
        return;
      IEntityView entityView = (IEntityView) target;
      if ((Object) entityView.GameObject == (Object) null)
        return;
      Transform transform = entityView.GameObject.transform;
      Pivot component = entityView.GameObject.GetComponent<Pivot>();
      if ((Object) component != (Object) null)
        transform = component.AnchorCameraFPS.transform;
      GameCamera.Instance.CameraTransform.position = transform.position;
      GameCamera.Instance.CameraTransform.rotation = transform.rotation;
    }
  }
}
