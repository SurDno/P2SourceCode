using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
  [ExecuteInEditMode]
  [AddComponentMenu("Image Effects/Blur/Blur")]
  public class Blur : MonoBehaviour
  {
    [Range(0.0f, 10f)]
    public int iterations = 3;
    [Range(0.0f, 1f)]
    public float blurSpread = 0.6f;
    public Shader blurShader;
    private static Material m_Material;

    protected Material material
    {
      get
      {
        if (m_Material == null)
        {
          m_Material = new Material(blurShader);
          m_Material.hideFlags = HideFlags.DontSave;
        }
        return m_Material;
      }
    }

    protected void OnDisable()
    {
      if (!(bool) (Object) m_Material)
        return;
      DestroyImmediate(m_Material);
    }

    protected void Start()
    {
      if (!SystemInfo.supportsImageEffects)
      {
        enabled = false;
      }
      else
      {
        if ((bool) (Object) blurShader && material.shader.isSupported)
          return;
        enabled = false;
      }
    }

    public void FourTapCone(RenderTexture source, RenderTexture dest, int iteration)
    {
      float num = (float) (0.5 + iteration * (double) blurSpread);
      Graphics.BlitMultiTap(source, dest, material, new Vector2(-num, -num), new Vector2(-num, num), new Vector2(num, num), new Vector2(num, -num));
    }

    private void DownSample4x(RenderTexture source, RenderTexture dest)
    {
      float num = 1f;
      Graphics.BlitMultiTap(source, dest, material, new Vector2(-num, -num), new Vector2(-num, num), new Vector2(num, num), new Vector2(num, -num));
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      int width = source.width / 4;
      int height = source.height / 4;
      RenderTexture renderTexture = RenderTexture.GetTemporary(width, height, 0);
      DownSample4x(source, renderTexture);
      for (int iteration = 0; iteration < iterations; ++iteration)
      {
        RenderTexture temporary = RenderTexture.GetTemporary(width, height, 0);
        FourTapCone(renderTexture, temporary, iteration);
        RenderTexture.ReleaseTemporary(renderTexture);
        renderTexture = temporary;
      }
      Graphics.Blit(renderTexture, destination);
      RenderTexture.ReleaseTemporary(renderTexture);
    }
  }
}
