// Decompiled with JetBrains decompiler
// Type: DynamicResolutionCamera
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[RequireComponent(typeof (Camera))]
public class DynamicResolutionCamera : MonoBehaviour
{
  private Camera camera;

  private void Awake() => this.camera = this.GetComponent<Camera>();

  private void OnDisable()
  {
    this.camera.targetTexture = (RenderTexture) null;
    DynamicResolution.Instance.RemoveCamera(this.camera);
  }

  private void OnEnable() => DynamicResolution.Instance.AddCamera(this.camera);

  private void LateUpdate()
  {
    this.camera.targetTexture = DynamicResolution.Instance.GetTargetTexture();
  }
}
