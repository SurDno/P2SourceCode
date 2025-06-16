using System.Collections.Generic;

namespace UnityStandardAssets.ImageEffects
{
  [ExecuteInEditMode]
  [RequireComponent(typeof (Camera))]
  public class PostEffectsBase : MonoBehaviour
  {
    protected bool supportHDRTextures = true;
    protected bool supportDepthTextures = true;
    protected bool supportDX11;
    protected bool isSupported = true;
    private List<Material> createdMaterials = new List<Material>();

    protected Material CheckShaderAndCreateMaterial(Shader shader, Material m2Create)
    {
      if (!(bool) (Object) shader)
      {
        Debug.Log((object) ("Missing shader in " + ToString()));
        this.enabled = false;
        return (Material) null;
      }
      if (shader.isSupported && (Object) m2Create != (Object) null && (Object) m2Create.shader == (Object) shader)
        return m2Create;
      if (!shader.isSupported)
      {
        NotSupported();
        Debug.Log((object) ("The shader " + ((object) shader) + " on effect " + ToString() + " is not supported on this platform!"));
        return (Material) null;
      }
      m2Create = new Material(shader);
      createdMaterials.Add(m2Create);
      m2Create.hideFlags = HideFlags.DontSave;
      return m2Create;
    }

    protected Material CreateMaterial(Shader shader, Material m2Create)
    {
      if (!(bool) (Object) shader)
      {
        Debug.Log((object) ("Missing shader in " + ToString()));
        return (Material) null;
      }
      if ((bool) (Object) m2Create && (Object) m2Create.shader == (Object) shader && shader.isSupported)
        return m2Create;
      if (!shader.isSupported)
        return (Material) null;
      m2Create = new Material(shader);
      createdMaterials.Add(m2Create);
      m2Create.hideFlags = HideFlags.DontSave;
      return m2Create;
    }

    private void OnEnable() => isSupported = true;

    private void OnDestroy() => RemoveCreatedMaterials();

    private void RemoveCreatedMaterials()
    {
      while (createdMaterials.Count > 0)
      {
        Material createdMaterial = createdMaterials[0];
        createdMaterials.RemoveAt(0);
        Object.Destroy((Object) createdMaterial);
      }
    }

    protected bool CheckSupport() => CheckSupport(false);

    public virtual bool CheckResources()
    {
      Debug.LogWarning((object) ("CheckResources () for " + ToString() + " should be overwritten."));
      return isSupported;
    }

    protected void Start()
    {
      supportHDRTextures = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf);
      supportDepthTextures = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth);
      CheckResources();
    }

    protected bool CheckSupport(bool needDepth)
    {
      isSupported = true;
      supportDX11 = SystemInfo.graphicsShaderLevel >= 50 && SystemInfo.supportsComputeShaders;
      if (!SystemInfo.supportsImageEffects || !SystemInfo.supportsRenderTextures)
      {
        NotSupported();
        return false;
      }
      if (needDepth && !supportDepthTextures)
      {
        NotSupported();
        return false;
      }
      if (needDepth)
        this.GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
      return true;
    }

    protected bool CheckSupport(bool needDepth, bool needHdr)
    {
      if (!CheckSupport(needDepth))
        return false;
      if (!needHdr || supportHDRTextures)
        return true;
      NotSupported();
      return false;
    }

    public bool Dx11Support() => supportDX11;

    protected void ReportAutoDisable()
    {
      Debug.LogWarning((object) ("The image effect " + ToString() + " has been disabled as it's not supported on the current platform."));
    }

    private bool CheckShader(Shader shader)
    {
      Debug.Log((object) ("The shader " + ((object) shader) + " on effect " + ToString() + " is not part of the Unity 3.2+ effects suite anymore. For best performance and quality, please ensure you are using the latest Standard Assets Image Effects (Pro only) package."));
      if (shader.isSupported)
        return false;
      NotSupported();
      return false;
    }

    protected void NotSupported()
    {
      this.enabled = false;
      isSupported = false;
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
  }
}
