using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Rain
{
  [ExecuteInEditMode]
  [RequireComponent(typeof (MeshRenderer))]
  public class Puddle : MonoBehaviour
  {
    private const CameraEvent RenderTime = CameraEvent.BeforeReflections;
    public Material material;
    private Ripple rippleListRoot = (Ripple) null;
    private CommandBuffer _ldrBuffer;
    private List<Camera> _ldrCameras;
    private CommandBuffer _hdrBuffer;
    private List<Camera> _hdrCameras;
    private MeshRenderer _meshRenderer;

    public bool GetWetness(RaycastHit Hit, out float Wetness)
    {
      Wetness = float.NaN;
      RainManager instance = RainManager.Instance;
      if ((Object) instance == (Object) null || (double) instance.puddleWetness <= 0.0 || (Object) this.material == (Object) null)
        return false;
      Texture2D mainTexture = this.material.mainTexture as Texture2D;
      if ((Object) mainTexture == (Object) null)
        return false;
      Color pixelBilinear = mainTexture.GetPixelBilinear(Hit.textureCoord.x, Hit.textureCoord.y);
      Color color1 = this.material.GetColor("_MainMask");
      PuddleCutter component = this.GetComponent<PuddleCutter>();
      if ((Object) component != (Object) null)
        color1 *= component.ColorMask;
      Color color2 = this.material.GetColor("_WorldMask");
      float num1 = this.material.GetFloat("_MaxSize");
      float num2 = this.material.GetFloat("_AuraSize");
      float num3 = 1f - (float) ((1.0 - (double) pixelBilinear.r) * (double) color1.r + (1.0 - (double) pixelBilinear.g) * (double) color1.g + (1.0 - (double) pixelBilinear.b) * (double) color1.b + (1.0 - (double) pixelBilinear.a) * (double) color1.a) + (float) (((double) color2.r + (double) color2.g + (double) color2.b + (double) color2.a) * 0.5);
      Wetness = num1 * instance.puddleWetness - num3;
      Wetness /= num2;
      return true;
    }

    public static Puddle.Hit Raycast(Ray ray, float maxDistance)
    {
      RainManager instance = RainManager.Instance;
      RaycastHit hitInfo;
      if ((Object) instance == (Object) null || (double) instance.puddleWetness <= 0.0 || !Physics.Raycast(ray, out hitInfo, maxDistance, (int) instance.puddleLayers))
        return (Puddle.Hit) null;
      Puddle component1 = hitInfo.collider.GetComponent<Puddle>();
      if ((Object) component1 == (Object) null)
        return (Puddle.Hit) null;
      Material material = component1.material;
      if ((Object) material == (Object) null)
        return (Puddle.Hit) null;
      Color pixelBilinear = (material.mainTexture as Texture2D).GetPixelBilinear(hitInfo.textureCoord.x, hitInfo.textureCoord.y);
      Color color1 = material.GetColor("_MainMask");
      PuddleCutter component2 = component1.GetComponent<PuddleCutter>();
      if ((Object) component2 != (Object) null)
        color1 *= component2.ColorMask;
      Color color2 = material.GetColor("_WorldMask");
      float num1 = material.GetFloat("_MaxSize");
      float num2 = material.GetFloat("_AuraSize");
      float num3 = 1f - (float) ((1.0 - (double) pixelBilinear.r) * (double) color1.r + (1.0 - (double) pixelBilinear.g) * (double) color1.g + (1.0 - (double) pixelBilinear.b) * (double) color1.b + (1.0 - (double) pixelBilinear.a) * (double) color1.a) + (float) (((double) color2.r + (double) color2.g + (double) color2.b + (double) color2.a) * 0.5);
      float wetness = (num1 * instance.puddleWetness - num3) / num2;
      return new Puddle.Hit(component1, hitInfo.point, wetness);
    }

    private MeshRenderer meshRenderer
    {
      get
      {
        if ((Object) this._meshRenderer == (Object) null)
          this._meshRenderer = this.GetComponent<MeshRenderer>();
        return this._meshRenderer;
      }
    }

    public void AddRipple(Vector3 worldPosition, float startRadius, float endRadius)
    {
      this.rippleListRoot = new Ripple(this.rippleListRoot, this.material, worldPosition, startRadius, endRadius);
      this.DestroyBuffers();
    }

    private void AssignBuffer(Camera cam)
    {
      if (cam.allowHDR)
      {
        if (!this.CheckBuffer(true) || this._hdrCameras.Contains(cam))
          return;
        cam.AddCommandBuffer(CameraEvent.BeforeReflections, this._hdrBuffer);
        this._hdrCameras.Add(cam);
      }
      else if (this.CheckBuffer(false) && !this._ldrCameras.Contains(cam))
      {
        cam.AddCommandBuffer(CameraEvent.BeforeReflections, this._ldrBuffer);
        this._ldrCameras.Add(cam);
      }
    }

    private bool CheckBuffer(bool hdr)
    {
      if (hdr)
      {
        if (this._hdrBuffer == null)
        {
          this._hdrBuffer = this.CreateBuffer(true);
          if (this._hdrBuffer != null)
            this._hdrCameras = new List<Camera>();
        }
        return this._hdrBuffer != null;
      }
      if (this._ldrBuffer == null)
      {
        this._ldrBuffer = this.CreateBuffer(false);
        if (this._ldrBuffer != null)
          this._ldrCameras = new List<Camera>();
      }
      return this._ldrBuffer != null;
    }

    private CommandBuffer CreateBuffer(bool hdr)
    {
      if (!(bool) (Object) this.material)
        return (CommandBuffer) null;
      CommandBuffer buffer = new CommandBuffer();
      buffer.name = "Decal " + this.name;
      RenderTargetIdentifier[] colors = new RenderTargetIdentifier[4]
      {
        (RenderTargetIdentifier) BuiltinRenderTextureType.GBuffer0,
        (RenderTargetIdentifier) BuiltinRenderTextureType.GBuffer1,
        (RenderTargetIdentifier) BuiltinRenderTextureType.GBuffer2,
        (RenderTargetIdentifier) BuiltinRenderTextureType.GBuffer3
      };
      if (hdr)
        colors[3] = (RenderTargetIdentifier) BuiltinRenderTextureType.CameraTarget;
      buffer.SetRenderTarget(colors, (RenderTargetIdentifier) BuiltinRenderTextureType.CameraTarget);
      buffer.DrawRenderer((Renderer) this.meshRenderer, this.material, 0, 0);
      if (this.rippleListRoot != null)
      {
        buffer.SetRenderTarget((RenderTargetIdentifier) BuiltinRenderTextureType.GBuffer2);
        for (Ripple ripple = this.rippleListRoot; ripple != null; ripple = ripple.nextNode)
          buffer.DrawRenderer((Renderer) this.meshRenderer, ripple.material, 0, 1);
      }
      return buffer;
    }

    private void DestroyBuffers()
    {
      if (this._ldrBuffer != null)
      {
        foreach (Camera ldrCamera in this._ldrCameras)
        {
          if ((bool) (Object) ldrCamera)
            ldrCamera.RemoveCommandBuffer(CameraEvent.BeforeReflections, this._ldrBuffer);
        }
      }
      this._ldrBuffer = (CommandBuffer) null;
      this._ldrCameras = (List<Camera>) null;
      if (this._hdrBuffer != null)
      {
        foreach (Camera hdrCamera in this._hdrCameras)
        {
          if ((bool) (Object) hdrCamera)
            hdrCamera.RemoveCommandBuffer(CameraEvent.BeforeReflections, this._hdrBuffer);
        }
      }
      this._hdrBuffer = (CommandBuffer) null;
      this._hdrCameras = (List<Camera>) null;
    }

    private void OnBecameInvisible() => this.DestroyBuffers();

    private void OnDisable() => this.DestroyBuffers();

    private void OnWillRenderObject()
    {
      if (!this.enabled || (Object) RainManager.Instance == (Object) null || (double) RainManager.Instance.puddleWetness <= 0.0 || Camera.current.actualRenderingPath != RenderingPath.DeferredShading)
        return;
      this.AssignBuffer(Camera.current);
    }

    private void Update()
    {
      if (this.rippleListRoot == null)
        return;
      bool flag = false;
      Ripple ripple1 = (Ripple) null;
      Ripple ripple2 = this.rippleListRoot;
      while (ripple2 != null)
      {
        if (ripple2.Update())
        {
          flag = true;
          ripple2 = ripple2.nextNode;
          if (ripple1 != null)
            ripple1.nextNode = ripple2;
          else
            this.rippleListRoot = ripple2;
        }
        else
        {
          ripple1 = ripple2;
          ripple2 = ripple2.nextNode;
        }
      }
      if (flag)
        this.DestroyBuffers();
    }

    public class Hit
    {
      public Puddle puddle;
      public Vector3 point;
      public float wetness;

      public Hit(Puddle puddle, Vector3 point, float wetness)
      {
        this.puddle = puddle;
        this.point = point;
        this.wetness = wetness;
      }
    }
  }
}
