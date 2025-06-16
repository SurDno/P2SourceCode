using System.Collections.Generic;

namespace Rain
{
  [ExecuteInEditMode]
  [RequireComponent(typeof (MeshRenderer))]
  public class Puddle : MonoBehaviour
  {
    private const CameraEvent RenderTime = CameraEvent.BeforeReflections;
    public Material material;
    private Ripple rippleListRoot = null;
    private CommandBuffer _ldrBuffer;
    private List<Camera> _ldrCameras;
    private CommandBuffer _hdrBuffer;
    private List<Camera> _hdrCameras;
    private MeshRenderer _meshRenderer;

    public bool GetWetness(RaycastHit Hit, out float Wetness)
    {
      Wetness = float.NaN;
      RainManager instance = RainManager.Instance;
      if ((Object) instance == (Object) null || instance.puddleWetness <= 0.0 || (Object) material == (Object) null)
        return false;
      Texture2D mainTexture = material.mainTexture as Texture2D;
      if ((Object) mainTexture == (Object) null)
        return false;
      Color pixelBilinear = mainTexture.GetPixelBilinear(Hit.textureCoord.x, Hit.textureCoord.y);
      Color color1 = material.GetColor("_MainMask");
      PuddleCutter component = this.GetComponent<PuddleCutter>();
      if ((Object) component != (Object) null)
        color1 *= component.ColorMask;
      Color color2 = material.GetColor("_WorldMask");
      float num1 = material.GetFloat("_MaxSize");
      float num2 = material.GetFloat("_AuraSize");
      float num3 = 1f - (float) ((1.0 - (double) pixelBilinear.r) * (double) color1.r + (1.0 - (double) pixelBilinear.g) * (double) color1.g + (1.0 - (double) pixelBilinear.b) * (double) color1.b + (1.0 - (double) pixelBilinear.a) * (double) color1.a) + (float) (((double) color2.r + (double) color2.g + (double) color2.b + (double) color2.a) * 0.5);
      Wetness = num1 * instance.puddleWetness - num3;
      Wetness /= num2;
      return true;
    }

    public static Hit Raycast(Ray ray, float maxDistance)
    {
      RainManager instance = RainManager.Instance;
      RaycastHit hitInfo;
      if ((Object) instance == (Object) null || instance.puddleWetness <= 0.0 || !Physics.Raycast(ray, out hitInfo, maxDistance, (int) instance.puddleLayers))
        return null;
      Puddle component1 = hitInfo.collider.GetComponent<Puddle>();
      if ((Object) component1 == (Object) null)
        return null;
      Material material = component1.material;
      if ((Object) material == (Object) null)
        return null;
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
      return new Hit(component1, hitInfo.point, wetness);
    }

    private MeshRenderer meshRenderer
    {
      get
      {
        if ((Object) _meshRenderer == (Object) null)
          _meshRenderer = this.GetComponent<MeshRenderer>();
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
      buffer.DrawRenderer((Renderer) meshRenderer, material, 0, 0);
      if (rippleListRoot != null)
      {
        buffer.SetRenderTarget((RenderTargetIdentifier) BuiltinRenderTextureType.GBuffer2);
        for (Ripple ripple = rippleListRoot; ripple != null; ripple = ripple.nextNode)
          buffer.DrawRenderer((Renderer) meshRenderer, ripple.material, 0, 1);
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
      _ldrBuffer = (CommandBuffer) null;
      _ldrCameras = (List<Camera>) null;
      if (_hdrBuffer != null)
      {
        foreach (Camera hdrCamera in _hdrCameras)
        {
          if ((bool) (Object) hdrCamera)
            hdrCamera.RemoveCommandBuffer(CameraEvent.BeforeReflections, _hdrBuffer);
        }
      }
      _hdrBuffer = (CommandBuffer) null;
      _hdrCameras = (List<Camera>) null;
    }

    private void OnBecameInvisible() => DestroyBuffers();

    private void OnDisable() => DestroyBuffers();

    private void OnWillRenderObject()
    {
      if (!this.enabled || (Object) RainManager.Instance == (Object) null || RainManager.Instance.puddleWetness <= 0.0 || Camera.current.actualRenderingPath != RenderingPath.DeferredShading)
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
