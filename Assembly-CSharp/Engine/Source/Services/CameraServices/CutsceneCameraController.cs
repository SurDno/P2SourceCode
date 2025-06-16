// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.CameraServices.CutsceneCameraController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using UnityEngine;

#nullable disable
namespace Engine.Source.Services.CameraServices
{
  public class CutsceneCameraController : ICameraController
  {
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
      GameCamera.Instance.CameraTransform.position = gameObjectTarget.transform.position;
      GameCamera.Instance.CameraTransform.rotation = gameObjectTarget.transform.rotation;
    }
  }
}
