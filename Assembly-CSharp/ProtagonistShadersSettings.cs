// Decompiled with JetBrains decompiler
// Type: ProtagonistShadersSettings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[RequireComponent(typeof (Camera))]
public class ProtagonistShadersSettings : MonoBehaviourInstance<ProtagonistShadersSettings>
{
  [Range(1f, 179f)]
  [SerializeField]
  private float fieldOfView = 60f;
  [Range(0.0f, 1f)]
  [SerializeField]
  private float flatness = 0.9f;
  private Camera camera;

  private Camera Camera
  {
    get
    {
      if ((Object) this.camera == (Object) null)
        this.camera = this.GetComponent<Camera>();
      return this.camera;
    }
  }

  private void OnDisable() => Shader.SetGlobalFloat("_ProtagonistFlatness", 0.0f);

  private void OnPreCull() => this.UpdateShaders();

  private Matrix4x4 ProtagonistProjection()
  {
    return Matrix4x4.Perspective(this.fieldOfView, this.Camera.aspect, this.Camera.nearClipPlane, this.Camera.farClipPlane);
  }

  public Vector3 ProtagonistToWorld(Vector3 position)
  {
    Vector4 vector4_1 = this.Camera.worldToCameraMatrix * new Vector4(position.x, position.y, position.z, 1f);
    Vector4 vector4_2 = this.ProtagonistProjection() * vector4_1;
    vector4_2.z = Mathf.Lerp(vector4_2.z, -1f * vector4_2.w, this.flatness);
    Vector4 vector4_3 = this.Camera.cameraToWorldMatrix * (this.Camera.nonJitteredProjectionMatrix.inverse * vector4_2);
    return (Vector3) (vector4_3 / vector4_3.w);
  }

  private void UpdateShaders()
  {
    Shader.SetGlobalFloat("_ProtagonistFlatness", this.flatness);
    Shader.SetGlobalMatrix("_ProtagonistMatrixVP", GL.GetGPUProjectionMatrix(this.ProtagonistProjection(), true) * this.Camera.worldToCameraMatrix);
  }
}
