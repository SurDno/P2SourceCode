using System;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof (Camera))]
public abstract class TOD_ImageEffect : MonoBehaviour
{
  public TOD_Sky sky = (TOD_Sky) null;
  protected Camera cam = (Camera) null;

  protected Material CreateMaterial(Shader shader)
  {
    if (!(bool) (UnityEngine.Object) shader)
    {
      Debug.Log((object) ("Missing shader in " + ((object) this).ToString()));
      this.enabled = false;
      return (Material) null;
    }
    if (!shader.isSupported)
    {
      Debug.LogError((object) ("The shader " + ((object) shader).ToString() + " on effect " + ((object) this).ToString() + " is not supported on this platform!"));
      this.enabled = false;
      return (Material) null;
    }
    Material material = new Material(shader);
    material.hideFlags = HideFlags.DontSave;
    return material;
  }

  protected void Awake()
  {
    if (!(bool) (UnityEngine.Object) this.cam)
      this.cam = this.GetComponent<Camera>();
    if ((bool) (UnityEngine.Object) this.sky)
      return;
    this.sky = UnityEngine.Object.FindObjectOfType(typeof (TOD_Sky)) as TOD_Sky;
  }

  protected bool CheckSupport(bool needDepth = false, bool needHdr = false)
  {
    if (!(bool) (UnityEngine.Object) this.cam)
      this.cam = this.GetComponent<Camera>();
    if (!(bool) (UnityEngine.Object) this.cam)
      return false;
    if (!(bool) (UnityEngine.Object) this.sky)
      this.sky = UnityEngine.Object.FindObjectOfType(typeof (TOD_Sky)) as TOD_Sky;
    if (!(bool) (UnityEngine.Object) this.sky || !this.sky.Initialized)
      return false;
    if (!SystemInfo.supportsImageEffects || !SystemInfo.supportsRenderTextures)
    {
      Debug.LogWarning((object) ("The image effect " + ((object) this).ToString() + " has been disabled as it's not supported on the current platform."));
      this.enabled = false;
      return false;
    }
    if (needDepth && !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth))
    {
      Debug.LogWarning((object) ("The image effect " + ((object) this).ToString() + " has been disabled as it requires a depth texture."));
      this.enabled = false;
      return false;
    }
    if (needHdr && !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf))
    {
      Debug.LogWarning((object) ("The image effect " + ((object) this).ToString() + " has been disabled as it requires HDR."));
      this.enabled = false;
      return false;
    }
    if (needDepth)
      this.cam.depthTextureMode |= DepthTextureMode.Depth;
    if (needHdr)
      this.cam.allowHDR = true;
    return true;
  }

  protected void DrawBorder(RenderTexture dest, Material material)
  {
    RenderTexture.active = dest;
    bool flag = true;
    GL.PushMatrix();
    GL.LoadOrtho();
    for (int pass = 0; pass < material.passCount; ++pass)
    {
      material.SetPass(pass);
      float y1;
      float y2;
      if (flag)
      {
        y1 = 1f;
        y2 = 0.0f;
      }
      else
      {
        y1 = 0.0f;
        y2 = 1f;
      }
      float x1 = 0.0f;
      float x2 = (float) (0.0 + 1.0 / ((double) dest.width * 1.0));
      float y3 = 0.0f;
      float y4 = 1f;
      GL.Begin(7);
      GL.TexCoord2(0.0f, y1);
      GL.Vertex3(x1, y3, 0.1f);
      GL.TexCoord2(1f, y1);
      GL.Vertex3(x2, y3, 0.1f);
      GL.TexCoord2(1f, y2);
      GL.Vertex3(x2, y4, 0.1f);
      GL.TexCoord2(0.0f, y2);
      GL.Vertex3(x1, y4, 0.1f);
      float x3 = (float) (1.0 - 1.0 / ((double) dest.width * 1.0));
      float x4 = 1f;
      float y5 = 0.0f;
      float y6 = 1f;
      GL.TexCoord2(0.0f, y1);
      GL.Vertex3(x3, y5, 0.1f);
      GL.TexCoord2(1f, y1);
      GL.Vertex3(x4, y5, 0.1f);
      GL.TexCoord2(1f, y2);
      GL.Vertex3(x4, y6, 0.1f);
      GL.TexCoord2(0.0f, y2);
      GL.Vertex3(x3, y6, 0.1f);
      float x5 = 0.0f;
      float x6 = 1f;
      float y7 = 0.0f;
      float y8 = (float) (0.0 + 1.0 / ((double) dest.height * 1.0));
      GL.TexCoord2(0.0f, y1);
      GL.Vertex3(x5, y7, 0.1f);
      GL.TexCoord2(1f, y1);
      GL.Vertex3(x6, y7, 0.1f);
      GL.TexCoord2(1f, y2);
      GL.Vertex3(x6, y8, 0.1f);
      GL.TexCoord2(0.0f, y2);
      GL.Vertex3(x5, y8, 0.1f);
      float x7 = 0.0f;
      float x8 = 1f;
      float y9 = (float) (1.0 - 1.0 / ((double) dest.height * 1.0));
      float y10 = 1f;
      GL.TexCoord2(0.0f, y1);
      GL.Vertex3(x7, y9, 0.1f);
      GL.TexCoord2(1f, y1);
      GL.Vertex3(x8, y9, 0.1f);
      GL.TexCoord2(1f, y2);
      GL.Vertex3(x8, y10, 0.1f);
      GL.TexCoord2(0.0f, y2);
      GL.Vertex3(x7, y10, 0.1f);
      GL.End();
    }
    GL.PopMatrix();
  }

  protected void CustomBlit(
    RenderTexture source,
    RenderTexture dest,
    Material fxMaterial,
    int passNr = 0)
  {
    RenderTexture.active = dest;
    fxMaterial.SetTexture("_MainTex", (Texture) source);
    GL.PushMatrix();
    GL.LoadOrtho();
    fxMaterial.SetPass(passNr);
    GL.Begin(7);
    GL.MultiTexCoord2(0, 0.0f, 0.0f);
    GL.Vertex3(0.0f, 0.0f, 3f);
    GL.MultiTexCoord2(0, 1f, 0.0f);
    GL.Vertex3(1f, 0.0f, 2f);
    GL.MultiTexCoord2(0, 1f, 1f);
    GL.Vertex3(1f, 1f, 1f);
    GL.MultiTexCoord2(0, 0.0f, 1f);
    GL.Vertex3(0.0f, 1f, 0.0f);
    GL.End();
    GL.PopMatrix();
  }

  protected Matrix4x4 FrustumCorners()
  {
    float nearClipPlane = this.cam.nearClipPlane;
    float farClipPlane = this.cam.farClipPlane;
    float fieldOfView = this.cam.fieldOfView;
    float aspect = this.cam.aspect;
    Vector3 forward = this.cam.transform.forward;
    Vector3 right = this.cam.transform.right;
    Vector3 up = this.cam.transform.up;
    Matrix4x4 identity = Matrix4x4.identity;
    float num1 = fieldOfView * 0.5f;
    Vector3 vector3_1 = right * nearClipPlane * Mathf.Tan(num1 * ((float) Math.PI / 180f)) * aspect;
    Vector3 vector3_2 = up * nearClipPlane * Mathf.Tan(num1 * ((float) Math.PI / 180f));
    Vector3 vector3_3 = forward * nearClipPlane - vector3_1 + vector3_2;
    float num2 = vector3_3.magnitude * farClipPlane / nearClipPlane;
    vector3_3.Normalize();
    Vector3 row1 = vector3_3 * num2;
    Vector3 vector3_4 = forward * nearClipPlane + vector3_1 + vector3_2;
    vector3_4.Normalize();
    Vector3 row2 = vector3_4 * num2;
    Vector3 vector3_5 = forward * nearClipPlane + vector3_1 - vector3_2;
    vector3_5.Normalize();
    Vector3 row3 = vector3_5 * num2;
    Vector3 vector3_6 = forward * nearClipPlane - vector3_1 - vector3_2;
    vector3_6.Normalize();
    Vector3 row4 = vector3_6 * num2;
    identity.SetRow(0, (Vector4) row1);
    identity.SetRow(1, (Vector4) row2);
    identity.SetRow(2, (Vector4) row3);
    identity.SetRow(3, (Vector4) row4);
    return identity;
  }
}
