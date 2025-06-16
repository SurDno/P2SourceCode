// Decompiled with JetBrains decompiler
// Type: Cinemachine.StaticPointVirtualCamera
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace Cinemachine
{
  internal class StaticPointVirtualCamera : ICinemachineCamera
  {
    public StaticPointVirtualCamera(CameraState state, string name)
    {
      this.State = state;
      this.Name = name;
    }

    public void SetState(CameraState state) => this.State = state;

    public string Name { get; private set; }

    public string Description => "";

    public int Priority { get; set; }

    public Transform LookAt { get; set; }

    public Transform Follow { get; set; }

    public CameraState State { get; private set; }

    public GameObject VirtualCameraGameObject => (GameObject) null;

    public ICinemachineCamera LiveChildOrSelf => (ICinemachineCamera) this;

    public ICinemachineCamera ParentCamera => (ICinemachineCamera) null;

    public bool IsLiveChild(ICinemachineCamera vcam) => false;

    public void UpdateCameraState(Vector3 worldUp, float deltaTime)
    {
    }

    public void OnTransitionFromCamera(
      ICinemachineCamera fromCam,
      Vector3 worldUp,
      float deltaTime)
    {
    }
  }
}
