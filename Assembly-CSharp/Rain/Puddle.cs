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
    private Ripple rippleListRoot;
    private CommandBuffer _ldrBuffer;
    private List<Camera> _ldrCameras;
    private CommandBuffer _hdrBuffer;
    private List<Camera> _hdrCameras;
    private MeshRenderer _meshRenderer;

    public bool GetWetness(RaycastHit Hit, out float Wetness)
    {
      Wetness = float.NaN;
      RainManager instance = RainManager.Instance;
      if (instance == null || instance.puddleWetness <= 0.0 || material == null)
        return false;
      Texture2D mainTexture = material.mainTexture as Texture2D;
      if (mainTexture == null)
        return false;
      Color pixelBilinear = mainTexture.GetPixelBilinear(Hit.textureCoord.x, Hit.textureCoord.y);
      Color color1 = material.GetColor("_MainMask");
      PuddleCutter component = GetComponent<PuddleCutter>();
      if (component != null)
        color1 *= component.ColorMask;
      Color color2 = material.GetColor("_WorldMask");
      float num1 = material.GetFloat("_MaxSize");
      float num2 = material.GetFloat("_AuraSize");
      float num3 = 1f - (float) ((1.0 - pixelBilinear.r) * color1.r + (1.0 - pixelBilinear.g) * color1.g + (1.0 - pixelBilinear.b) * color1.b + (1.0 - pixelBilinear.a) * color1.a) + (float) ((color2.r + (double) color2.g + color2.b + color2.a) * 0.5);
      Wetness = num1 * instance.puddleWetness - num3;
      Wetness /= num2;
      return true;
    }

    public static Hit Raycast(Ray ray, float maxDistance)
    {
      RainManager instance = RainManager.Instance;
      RaycastHit hitInfo;
      if (instance == null || instance.puddleWetness <= 0.0 || !Physics.Raycast(ray, out hitInfo, maxDistance, instance.puddleLayers))
        return null;
      Puddle component1 = hitInfo.collider.GetComponent<Puddle>();
      if (component1 == null)
        return null;
      Material material = component1.material;
      if (material == null)
        return null;
      Color pixelBilinear = (material.mainTexture as Texture2D).GetPixelBilinear(hitInfo.textureCoord.x, hitInfo.textureCoord.y);
      Color color1 = material.GetColor("_MainMask");
      PuddleCutter component2 = component1.GetComponent<PuddleCutter>();
      if (component2 != null)
        color1 *= component2.ColorMask;
      Color color2 = material.GetColor("_WorldMask");
      float num1 = material.GetFloat("_MaxSize");
      float num2 = material.GetFloat("_AuraSize");
      float num3 = 1f - (float) ((1.0 - pixelBilinear.r) * color1.r + (1.0 - pixelBilinear.g) * color1.g + (1.0 - pixelBilinear.b) * color1.b + (1.0 - pixelBilinear.a) * color1.a) + (float) ((color2.r + (double) color2.g + color2.b + color2.a) * 0.5);
      float wetness = (num1 * instance.puddleWetness - num3) / num2;
      return new Hit(component1, hitInfo.point, wetness);
    }

    private MeshRenderer meshRenderer
    {
      get
      {
        if (_meshRenderer == null)
          _meshRenderer = GetComponent<MeshRenderer>();
        return _meshRenderer;
      }
    }

    public void AddRipple(Vector3 worldPosition, float startRadius, float endRadius)
    {
      rippleListRoot = new Ripple(rippleListRoot, material, worldPosition, startRadius, endRadius);
      DestroyBuffers();
    }

    private void AssignBuffer(Camera cam)
    {
      if (cam.allowHDR)
      {
        if (!CheckBuffer(true) || _hdrCameras.Contains(cam))
          return;
        cam.AddCommandBuffer(CameraEvent.BeforeReflections, _hdrBuffer);
        _hdrCameras.Add(cam);
      }
      else if (CheckBuffer(false) && !_ldrCameras.Contains(cam))
      {
        cam.AddCommandBuffer(CameraEvent.BeforeReflections, _ldrBuffer);
        _ldrCameras.Add(cam);
      }
    }

    private bool CheckBuffer(bool hdr)
    {
      if (hdr)
      {
        if (_hdrBuffer == null)
        {
          _hdrBuffer = CreateBuffer(true);
          if (_hdrBuffer != null)
            _hdrCameras = new List<Camera>();
        }
        return _hdrBuffer != null;
      }
      if (_ldrBuffer == null)
      {
        _ldrBuffer = CreateBuffer(false);
        if (_ldrBuffer != null)
          _ldrCameras = new List<Camera>();
      }
      return _ldrBuffer != null;
    }

    private CommandBuffer CreateBuffer(bool hdr)
    {
      if (!(bool) (Object) material)
        return null;
      CommandBuffer buffer = new CommandBuffer();
      buffer.name = "Decal " + name;
      RenderTargetIdentifier[] colors = new RenderTargetIdentifier[4]
      {
        BuiltinRenderTextureType.GBuffer0,
        BuiltinRenderTextureType.GBuffer1,
        BuiltinRenderTextureType.GBuffer2,
        BuiltinRenderTextureType.GBuffer3
      };
      if (hdr)
        colors[3] = BuiltinRenderTextureType.CameraTarget;
      buffer.SetRenderTarget(colors, BuiltinRenderTextureType.CameraTarget);
      buffer.DrawRenderer(meshRenderer, material, 0, 0);
      if (rippleListRoot != null)
      {
        buffer.SetRenderTarget(BuiltinRenderTextureType.GBuffer2);
        for (Ripple ripple = rippleListRoot; ripple != null; ripple = ripple.nextNode)
          buffer.DrawRenderer(meshRenderer, ripple.material, 0, 1);
      }
      return buffer;
    }

    private void DestroyBuffers()
    {
      if (_ldrBuffer != null)
      {
        foreach (Camera ldrCamera in _ldrCameras)
        {
          if ((bool) (Object) ldrCamera)
            ldrCamera.RemoveCommandBuffer(CameraEvent.BeforeReflections, _ldrBuffer);
        }
      }
      _ldrBuffer = null;
      _ldrCameras = null;
      if (_hdrBuffer != null)
      {
        foreach (Camera hdrCamera in _hdrCameras)
        {
          if ((bool) (Object) hdrCamera)
            hdrCamera.RemoveCommandBuffer(CameraEvent.BeforeReflections, _hdrBuffer);
        }
      }
      _hdrBuffer = null;
      _hdrCameras = null;
    }

    private void OnBecameInvisible() => DestroyBuffers();

    private void OnDisable() => DestroyBuffers();

    private void OnWillRenderObject()
    {
      if (!enabled || RainManager.Instance == null || RainManager.Instance.puddleWetness <= 0.0 || Camera.current.actualRenderingPath != RenderingPath.DeferredShading)
        return;
      AssignBuffer(Camera.current);
    }

    private void Update()
    {
      if (rippleListRoot == null)
        return;
      bool flag = false;
      Ripple ripple1 = null;
      Ripple ripple2 = rippleListRoot;
      while (ripple2 != null)
      {
        if (ripple2.Update())
        {
          flag = true;
          ripple2 = ripple2.nextNode;
          if (ripple1 != null)
            ripple1.nextNode = ripple2;
          else
            rippleListRoot = ripple2;
        }
        else
        {
          ripple1 = ripple2;
          ripple2 = ripple2.nextNode;
        }
      }
      if (flag)
        DestroyBuffers();
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
