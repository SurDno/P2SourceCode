﻿// Decompiled with JetBrains decompiler
// Type: VolumetricLight
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.Rendering;

#nullable disable
[RequireComponent(typeof (Light))]
public class VolumetricLight : MonoBehaviour
{
  private Light _light;
  private Material _material;
  private CommandBuffer _commandBuffer;
  private CommandBuffer _cascadeShadowCommandBuffer;
  [Range(0.0f, 1f)]
  public float ScatteringCoef = 0.5f;
  [Range(0.0f, 0.1f)]
  public float ExtinctionCoef = 0.01f;
  [Range(0.0f, 1f)]
  public float SkyboxExtinctionCoef = 0.9f;
  [Range(0.0f, 0.999f)]
  public float MieG = 0.1f;
  public bool HeightFog = false;
  [Range(0.0f, 0.5f)]
  public float HeightScale = 0.1f;
  public float GroundLevel = 0.0f;
  public bool Noise = false;
  public float NoiseScale = 0.015f;
  public float NoiseIntensity = 1f;
  public float NoiseIntensityOffset = 0.3f;
  public Vector2 NoiseVelocity = new Vector2(3f, 3f);
  [Tooltip("")]
  public float MaxRayLength = 400f;
  private Vector4[] _frustumCorners = new Vector4[4];
  private bool _reversedZ = false;

  public event Action<VolumetricLightRenderer, VolumetricLight, CommandBuffer, Matrix4x4> CustomRenderEvent;

  public Light Light => this._light;

  public Material VolumetricMaterial => this._material;

  private void Start()
  {
    if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D11 || SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D12 || SystemInfo.graphicsDeviceType == GraphicsDeviceType.Metal || SystemInfo.graphicsDeviceType == GraphicsDeviceType.PlayStation4 || SystemInfo.graphicsDeviceType == GraphicsDeviceType.Vulkan || SystemInfo.graphicsDeviceType == GraphicsDeviceType.XboxOne)
      this._reversedZ = true;
    this._commandBuffer = new CommandBuffer();
    this._commandBuffer.name = "Light Command Buffer";
    this._cascadeShadowCommandBuffer = new CommandBuffer();
    this._cascadeShadowCommandBuffer.name = "Dir Light Command Buffer";
    this._cascadeShadowCommandBuffer.SetGlobalTexture("_CascadeShadowMapTexture", new RenderTargetIdentifier(BuiltinRenderTextureType.CurrentActive));
    this._light = this.GetComponent<Light>();
    if (this._light.type == LightType.Directional)
    {
      this._light.AddCommandBuffer(LightEvent.BeforeScreenspaceMask, this._commandBuffer);
      this._light.AddCommandBuffer(LightEvent.AfterShadowMap, this._cascadeShadowCommandBuffer);
    }
    else
      this._light.AddCommandBuffer(LightEvent.AfterShadowMap, this._commandBuffer);
    Shader shader = Shader.Find("Sandbox/VolumetricLight");
    this._material = !((UnityEngine.Object) shader == (UnityEngine.Object) null) ? new Material(shader) : throw new Exception("Critical Error: \"Sandbox/VolumetricLight\" shader is missing. Make sure it is included in \"Always Included Shaders\" in ProjectSettings/Graphics.");
  }

  private void OnEnable()
  {
    VolumetricLightRenderer.PreRenderEvent += new Action<VolumetricLightRenderer, Matrix4x4>(this.VolumetricLightRenderer_PreRenderEvent);
    VolumetricLightRenderer.PostRenderEvent += new Action(this.VolumetricLightRenderer_PostRenderEvent);
  }

  private void OnDisable()
  {
    VolumetricLightRenderer.PreRenderEvent -= new Action<VolumetricLightRenderer, Matrix4x4>(this.VolumetricLightRenderer_PreRenderEvent);
    VolumetricLightRenderer.PostRenderEvent -= new Action(this.VolumetricLightRenderer_PostRenderEvent);
  }

  public void OnDestroy() => UnityEngine.Object.Destroy((UnityEngine.Object) this._material);

  private void VolumetricLightRenderer_PostRenderEvent() => this._commandBuffer.Clear();

  private void VolumetricLightRenderer_PreRenderEvent(
    VolumetricLightRenderer renderer,
    Matrix4x4 viewProj)
  {
    if ((UnityEngine.Object) this._light == (UnityEngine.Object) null || (UnityEngine.Object) this._light.gameObject == (UnityEngine.Object) null)
    {
      VolumetricLightRenderer.PreRenderEvent -= new Action<VolumetricLightRenderer, Matrix4x4>(this.VolumetricLightRenderer_PreRenderEvent);
      VolumetricLightRenderer.PostRenderEvent -= new Action(this.VolumetricLightRenderer_PostRenderEvent);
    }
    else
    {
      if (!this._light.isActiveAndEnabled || (double) this._light.intensity <= 0.0 || this._light.color == Color.black)
        return;
      this._material.SetVector("_CameraForward", (Vector4) Camera.current.transform.forward);
      this._material.SetInt("_SampleCount", 3);
      this._material.SetVector("_NoiseVelocity", new Vector4(this.NoiseVelocity.x, this.NoiseVelocity.y) * this.NoiseScale);
      this._material.SetVector("_NoiseData", new Vector4(this.NoiseScale, this.NoiseIntensity, this.NoiseIntensityOffset));
      this._material.SetVector("_MieG", new Vector4((float) (1.0 - (double) this.MieG * (double) this.MieG), (float) (1.0 + (double) this.MieG * (double) this.MieG), 2f * this.MieG, 0.07957747f));
      this._material.SetVector("_VolumetricLight", new Vector4(this.ScatteringCoef, this.ExtinctionCoef, this._light.range, 1f - this.SkyboxExtinctionCoef));
      this._material.SetTexture("_CameraDepthTexture", (Texture) renderer.GetVolumeLightDepthBuffer());
      this._material.SetFloat("_ZTest", 8f);
      if (this.HeightFog)
      {
        this._material.EnableKeyword("HEIGHT_FOG");
        this._material.SetVector("_HeightFog", new Vector4(this.GroundLevel, this.HeightScale));
      }
      else
        this._material.DisableKeyword("HEIGHT_FOG");
      if (this._light.type == LightType.Point)
        this.SetupPointLight(renderer, viewProj);
      else if (this._light.type == LightType.Spot)
      {
        this.SetupSpotLight(renderer, viewProj);
      }
      else
      {
        if (this._light.type != LightType.Directional)
          return;
        this.SetupDirectionalLight(renderer, viewProj);
      }
    }
  }

  private void SetupPointLight(VolumetricLightRenderer renderer, Matrix4x4 viewProj)
  {
    int num1 = 0;
    if (!this.IsCameraInPointLightBounds())
      num1 = 2;
    this._material.SetPass(num1);
    Mesh pointLightMesh = VolumetricLightRenderer.GetPointLightMesh();
    float num2 = this._light.range * 2f;
    Matrix4x4 matrix = Matrix4x4.TRS(this.transform.position, this._light.transform.rotation, new Vector3(num2, num2, num2));
    this._material.SetMatrix("_WorldViewProj", viewProj * matrix);
    this._material.SetMatrix("_WorldView", Camera.current.worldToCameraMatrix * matrix);
    if (this.Noise)
      this._material.EnableKeyword("NOISE");
    else
      this._material.DisableKeyword("NOISE");
    this._material.SetVector("_LightPos", new Vector4(this._light.transform.position.x, this._light.transform.position.y, this._light.transform.position.z, (float) (1.0 / ((double) this._light.range * (double) this._light.range))));
    this._material.SetColor("_LightColor", this._light.color * this._light.intensity);
    if ((UnityEngine.Object) this._light.cookie == (UnityEngine.Object) null)
    {
      this._material.EnableKeyword("POINT");
      this._material.DisableKeyword("POINT_COOKIE");
    }
    else
    {
      this._material.SetMatrix("_MyLightMatrix0", Matrix4x4.TRS(this._light.transform.position, this._light.transform.rotation, Vector3.one).inverse);
      this._material.EnableKeyword("POINT_COOKIE");
      this._material.DisableKeyword("POINT");
      this._material.SetTexture("_LightTexture0", this._light.cookie);
    }
    bool flag = false;
    if ((double) (this._light.transform.position - Camera.current.transform.position).magnitude >= (double) QualitySettings.shadowDistance)
      flag = true;
    if (this._light.shadows != LightShadows.None && !flag)
    {
      this._material.EnableKeyword("SHADOWS_CUBE");
      this._commandBuffer.SetGlobalTexture("_ShadowMapTexture", (RenderTargetIdentifier) BuiltinRenderTextureType.CurrentActive);
      this._commandBuffer.SetRenderTarget((RenderTargetIdentifier) (Texture) renderer.GetVolumeLightBuffer());
      this._commandBuffer.DrawMesh(pointLightMesh, matrix, this._material, 0, num1);
      Action<VolumetricLightRenderer, VolumetricLight, CommandBuffer, Matrix4x4> customRenderEvent = this.CustomRenderEvent;
      if (customRenderEvent == null)
        return;
      customRenderEvent(renderer, this, this._commandBuffer, viewProj);
    }
    else
    {
      this._material.DisableKeyword("SHADOWS_CUBE");
      renderer.GlobalCommandBuffer.DrawMesh(pointLightMesh, matrix, this._material, 0, num1);
      Action<VolumetricLightRenderer, VolumetricLight, CommandBuffer, Matrix4x4> customRenderEvent = this.CustomRenderEvent;
      if (customRenderEvent != null)
        customRenderEvent(renderer, this, renderer.GlobalCommandBuffer, viewProj);
    }
  }

  private void SetupSpotLight(VolumetricLightRenderer renderer, Matrix4x4 viewProj)
  {
    int shaderPass = 1;
    if (!this.IsCameraInSpotLightBounds())
      shaderPass = 3;
    Mesh spotLightMesh = VolumetricLightRenderer.GetSpotLightMesh();
    float range = this._light.range;
    float num = Mathf.Tan((float) (((double) this._light.spotAngle + 1.0) * 0.5 * (Math.PI / 180.0))) * this._light.range;
    Matrix4x4 matrix = Matrix4x4.TRS(this.transform.position, this.transform.rotation, new Vector3(num, num, range));
    Matrix4x4 inverse = Matrix4x4.TRS(this._light.transform.position, this._light.transform.rotation, Vector3.one).inverse;
    this._material.SetMatrix("_MyLightMatrix0", Matrix4x4.TRS(new Vector3(0.5f, 0.5f, 0.0f), Quaternion.identity, new Vector3(-0.5f, -0.5f, 1f)) * Matrix4x4.Perspective(this._light.spotAngle, 1f, 0.0f, 1f) * inverse);
    this._material.SetMatrix("_WorldViewProj", viewProj * matrix);
    this._material.SetVector("_LightPos", new Vector4(this._light.transform.position.x, this._light.transform.position.y, this._light.transform.position.z, (float) (1.0 / ((double) this._light.range * (double) this._light.range))));
    this._material.SetVector("_LightColor", (Vector4) (this._light.color * this._light.intensity));
    Vector3 position = this.transform.position;
    Vector3 forward = this.transform.forward;
    this._material.SetFloat("_PlaneD", -Vector3.Dot(position + forward * this._light.range, forward));
    this._material.SetFloat("_CosAngle", Mathf.Cos((float) (((double) this._light.spotAngle + 1.0) * 0.5 * (Math.PI / 180.0))));
    this._material.SetVector("_ConeApex", new Vector4(position.x, position.y, position.z));
    this._material.SetVector("_ConeAxis", new Vector4(forward.x, forward.y, forward.z));
    this._material.EnableKeyword("SPOT");
    if (this.Noise)
      this._material.EnableKeyword("NOISE");
    else
      this._material.DisableKeyword("NOISE");
    if ((UnityEngine.Object) this._light.cookie == (UnityEngine.Object) null)
      this._material.SetTexture("_LightTexture0", VolumetricLightRenderer.GetDefaultSpotCookie());
    else
      this._material.SetTexture("_LightTexture0", this._light.cookie);
    bool flag = false;
    Vector3 b = position + forward * this._light.range;
    if ((double) VolumetricLight.DistanceToSegment(Camera.current.transform.position, position, b) >= (double) QualitySettings.shadowDistance)
      flag = true;
    if (this._light.shadows != LightShadows.None && !flag)
    {
      Matrix4x4 matrix4x4 = Matrix4x4.TRS(new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity, new Vector3(0.5f, 0.5f, 0.5f)) * (!this._reversedZ ? Matrix4x4.Perspective(this._light.spotAngle, 1f, this._light.shadowNearPlane, this._light.range) : Matrix4x4.Perspective(this._light.spotAngle, 1f, this._light.range, this._light.shadowNearPlane));
      matrix4x4[0, 2] *= -1f;
      matrix4x4[1, 2] *= -1f;
      matrix4x4[2, 2] *= -1f;
      matrix4x4[3, 2] *= -1f;
      this._material.SetMatrix("_MyWorld2Shadow", matrix4x4 * inverse);
      this._material.SetMatrix("_WorldView", matrix4x4 * inverse);
      this._material.EnableKeyword("SHADOWS_DEPTH");
      this._commandBuffer.SetGlobalTexture("_ShadowMapTexture", (RenderTargetIdentifier) BuiltinRenderTextureType.CurrentActive);
      this._commandBuffer.SetRenderTarget((RenderTargetIdentifier) (Texture) renderer.GetVolumeLightBuffer());
      this._commandBuffer.DrawMesh(spotLightMesh, matrix, this._material, 0, shaderPass);
      Action<VolumetricLightRenderer, VolumetricLight, CommandBuffer, Matrix4x4> customRenderEvent = this.CustomRenderEvent;
      if (customRenderEvent == null)
        return;
      customRenderEvent(renderer, this, this._commandBuffer, viewProj);
    }
    else
    {
      this._material.DisableKeyword("SHADOWS_DEPTH");
      renderer.GlobalCommandBuffer.DrawMesh(spotLightMesh, matrix, this._material, 0, shaderPass);
      Action<VolumetricLightRenderer, VolumetricLight, CommandBuffer, Matrix4x4> customRenderEvent = this.CustomRenderEvent;
      if (customRenderEvent != null)
        customRenderEvent(renderer, this, renderer.GlobalCommandBuffer, viewProj);
    }
  }

  private void SetupDirectionalLight(VolumetricLightRenderer renderer, Matrix4x4 viewProj)
  {
    int pass = 4;
    this._material.SetPass(pass);
    if (this.Noise)
      this._material.EnableKeyword("NOISE");
    else
      this._material.DisableKeyword("NOISE");
    this._material.SetVector("_LightDir", new Vector4(this._light.transform.forward.x, this._light.transform.forward.y, this._light.transform.forward.z, (float) (1.0 / ((double) this._light.range * (double) this._light.range))));
    this._material.SetVector("_LightColor", (Vector4) (this._light.color * this._light.intensity));
    this._material.SetFloat("_MaxRayLength", this.MaxRayLength);
    if ((UnityEngine.Object) this._light.cookie == (UnityEngine.Object) null)
    {
      this._material.EnableKeyword("DIRECTIONAL");
      this._material.DisableKeyword("DIRECTIONAL_COOKIE");
    }
    else
    {
      this._material.EnableKeyword("DIRECTIONAL_COOKIE");
      this._material.DisableKeyword("DIRECTIONAL");
      this._material.SetTexture("_LightTexture0", this._light.cookie);
    }
    this._frustumCorners[0] = (Vector4) Camera.current.ViewportToWorldPoint(new Vector3(0.0f, 0.0f, Camera.current.farClipPlane));
    this._frustumCorners[2] = (Vector4) Camera.current.ViewportToWorldPoint(new Vector3(0.0f, 1f, Camera.current.farClipPlane));
    this._frustumCorners[3] = (Vector4) Camera.current.ViewportToWorldPoint(new Vector3(1f, 1f, Camera.current.farClipPlane));
    this._frustumCorners[1] = (Vector4) Camera.current.ViewportToWorldPoint(new Vector3(1f, 0.0f, Camera.current.farClipPlane));
    this._material.SetVectorArray("_FrustumCorners", this._frustumCorners);
    Texture source = (Texture) null;
    if (this._light.shadows != 0)
    {
      this._material.EnableKeyword("SHADOWS_DEPTH");
      this._commandBuffer.Blit(source, (RenderTargetIdentifier) (Texture) renderer.GetVolumeLightBuffer(), this._material, pass);
      Action<VolumetricLightRenderer, VolumetricLight, CommandBuffer, Matrix4x4> customRenderEvent = this.CustomRenderEvent;
      if (customRenderEvent == null)
        return;
      customRenderEvent(renderer, this, this._commandBuffer, viewProj);
    }
    else
    {
      this._material.DisableKeyword("SHADOWS_DEPTH");
      renderer.GlobalCommandBuffer.Blit(source, (RenderTargetIdentifier) (Texture) renderer.GetVolumeLightBuffer(), this._material, pass);
      Action<VolumetricLightRenderer, VolumetricLight, CommandBuffer, Matrix4x4> customRenderEvent = this.CustomRenderEvent;
      if (customRenderEvent != null)
        customRenderEvent(renderer, this, renderer.GlobalCommandBuffer, viewProj);
    }
  }

  private bool IsCameraInPointLightBounds()
  {
    float sqrMagnitude = (this._light.transform.position - Camera.current.transform.position).sqrMagnitude;
    float num = this._light.range + 1f;
    return (double) sqrMagnitude < (double) num * (double) num;
  }

  private bool IsCameraInSpotLightBounds()
  {
    return (double) Vector3.Dot(this._light.transform.forward, Camera.current.transform.position - this._light.transform.position) <= (double) (this._light.range + 1f) && (double) Mathf.Acos(Vector3.Dot(this.transform.forward, (Camera.current.transform.position - this._light.transform.position).normalized)) * 57.295780181884766 <= ((double) this._light.spotAngle + 3.0) * 0.5;
  }

  private static float DistanceToSegment(Vector3 v, Vector3 a, Vector3 b)
  {
    Vector3 vector3_1 = b - a;
    Vector3 vector3_2 = v - a;
    if ((double) Vector3.Dot(vector3_2, vector3_1) <= 0.0)
      return vector3_2.magnitude;
    Vector3 lhs = v - b;
    return (double) Vector3.Dot(lhs, vector3_1) >= 0.0 ? lhs.magnitude : Vector3.Cross(vector3_1, vector3_2).magnitude / vector3_1.magnitude;
  }
}
