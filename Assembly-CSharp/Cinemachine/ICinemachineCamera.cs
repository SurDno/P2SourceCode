// Decompiled with JetBrains decompiler
// Type: Cinemachine.ICinemachineCamera
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace Cinemachine
{
  public interface ICinemachineCamera
  {
    string Name { get; }

    string Description { get; }

    int Priority { get; set; }

    Transform LookAt { get; set; }

    Transform Follow { get; set; }

    CameraState State { get; }

    GameObject VirtualCameraGameObject { get; }

    ICinemachineCamera LiveChildOrSelf { get; }

    ICinemachineCamera ParentCamera { get; }

    bool IsLiveChild(ICinemachineCamera vcam);

    void UpdateCameraState(Vector3 worldUp, float deltaTime);

    void OnTransitionFromCamera(ICinemachineCamera fromCam, Vector3 worldUp, float deltaTime);
  }
}
