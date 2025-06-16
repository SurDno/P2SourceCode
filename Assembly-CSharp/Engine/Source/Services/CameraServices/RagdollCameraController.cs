// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.CameraServices.RagdollCameraController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using UnityEngine;

#nullable disable
namespace Engine.Source.Services.CameraServices
{
  public class RagdollCameraController : ICameraController
  {
    private float smoothTime = 0.05f;
    private float speedSmoothTime = 0.5f;

    public void Initialise()
    {
    }

    public void Shutdown()
    {
    }

    public void Update(IEntity target, GameObject gameObjectTarget)
    {
      if ((Object) gameObjectTarget == (Object) null)
        return;
      this.smoothTime += this.speedSmoothTime * Time.deltaTime;
      Transform cameraTransform = GameCamera.Instance.CameraTransform;
      cameraTransform.position = Vector3.Lerp(cameraTransform.position, gameObjectTarget.transform.position, Time.deltaTime / this.smoothTime);
      cameraTransform.rotation = Quaternion.Slerp(cameraTransform.rotation, gameObjectTarget.transform.rotation, Time.deltaTime / this.smoothTime);
    }
  }
}
