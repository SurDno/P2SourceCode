// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.CameraServices.TrackingCameraController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Behaviours.Components;
using Engine.Common;
using Engine.Source.Commons;
using UnityEngine;

#nullable disable
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
