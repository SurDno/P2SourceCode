// Decompiled with JetBrains decompiler
// Type: UnityStandardAssets.ImageEffects.ContrastStretch
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace UnityStandardAssets.ImageEffects
{
  [ExecuteInEditMode]
  [AddComponentMenu("Image Effects/Color Adjustments/Contrast Stretch")]
  public class ContrastStretch : MonoBehaviour
  {
    [Range(0.0001f, 1f)]
    public float adaptationSpeed = 0.02f;
    [Range(0.0f, 1f)]
    public float limitMinimum = 0.2f;
    [Range(0.0f, 1f)]
    public float limitMaximum = 0.6f;
    private RenderTexture[] adaptRenderTex = new RenderTexture[2];
    private int curAdaptIndex = 0;
    public Shader shaderLum;
    private Material m_materialLum;
    public Shader shaderReduce;
    private Material m_materialReduce;
    public Shader shaderAdapt;
    private Material m_materialAdapt;
    public Shader shaderApply;
    private Material m_materialApply;

    protected Material materialLum
    {
      get
      {
        if ((Object) this.m_materialLum == (Object) null)
        {
          this.m_materialLum = new Material(this.shaderLum);
          this.m_materialLum.hideFlags = HideFlags.HideAndDontSave;
        }
        return this.m_materialLum;
      }
    }

    protected Material materialReduce
    {
      get
      {
        if ((Object) this.m_materialReduce == (Object) null)
        {
          this.m_materialReduce = new Material(this.shaderReduce);
          this.m_materialReduce.hideFlags = HideFlags.HideAndDontSave;
        }
        return this.m_materialReduce;
      }
    }

    protected Material materialAdapt
    {
      get
      {
        if ((Object) this.m_materialAdapt == (Object) null)
        {
          this.m_materialAdapt = new Material(this.shaderAdapt);
          this.m_materialAdapt.hideFlags = HideFlags.HideAndDontSave;
        }
        return this.m_materialAdapt;
      }
    }

    protected Material materialApply
    {
      get
      {
        if ((Object) this.m_materialApply == (Object) null)
        {
          this.m_materialApply = new Material(this.shaderApply);
          this.m_materialApply.hideFlags = HideFlags.HideAndDontSave;
        }
        return this.m_materialApply;
      }
    }

    private void Start()
    {
      if (!SystemInfo.supportsImageEffects)
      {
        this.enabled = false;
      }
      else
      {
        if (this.shaderAdapt.isSupported && this.shaderApply.isSupported && this.shaderLum.isSupported && this.shaderReduce.isSupported)
          return;
        this.enabled = false;
      }
    }

    private void OnEnable()
    {
      for (int index = 0; index < 2; ++index)
      {
        if (!(bool) (Object) this.adaptRenderTex[index])
        {
          this.adaptRenderTex[index] = new RenderTexture(1, 1, 0);
          this.adaptRenderTex[index].hideFlags = HideFlags.HideAndDontSave;
        }
      }
    }

    private void OnDisable()
    {
      for (int index = 0; index < 2; ++index)
      {
        Object.DestroyImmediate((Object) this.adaptRenderTex[index]);
        this.adaptRenderTex[index] = (RenderTexture) null;
      }
      if ((bool) (Object) this.m_materialLum)
        Object.DestroyImmediate((Object) this.m_materialLum);
      if ((bool) (Object) this.m_materialReduce)
        Object.DestroyImmediate((Object) this.m_materialReduce);
      if ((bool) (Object) this.m_materialAdapt)
        Object.DestroyImmediate((Object) this.m_materialAdapt);
      if (!(bool) (Object) this.m_materialApply)
        return;
      Object.DestroyImmediate((Object) this.m_materialApply);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      RenderTexture renderTexture = RenderTexture.GetTemporary(source.width / 1, source.height / 1);
      Graphics.Blit((Texture) source, renderTexture, this.materialLum);
      RenderTexture temporary;
      for (; renderTexture.width > 1 || renderTexture.height > 1; renderTexture = temporary)
      {
        int width = renderTexture.width / 2;
        if (width < 1)
          width = 1;
        int height = renderTexture.height / 2;
        if (height < 1)
          height = 1;
        temporary = RenderTexture.GetTemporary(width, height);
        Graphics.Blit((Texture) renderTexture, temporary, this.materialReduce);
        RenderTexture.ReleaseTemporary(renderTexture);
      }
      this.CalculateAdaptation((Texture) renderTexture);
      this.materialApply.SetTexture("_AdaptTex", (Texture) this.adaptRenderTex[this.curAdaptIndex]);
      Graphics.Blit((Texture) source, destination, this.materialApply);
      RenderTexture.ReleaseTemporary(renderTexture);
    }

    private void CalculateAdaptation(Texture curTexture)
    {
      int curAdaptIndex = this.curAdaptIndex;
      this.curAdaptIndex = (this.curAdaptIndex + 1) % 2;
      float x = Mathf.Clamp(1f - Mathf.Pow(1f - this.adaptationSpeed, 30f * Time.deltaTime), 0.01f, 1f);
      this.materialAdapt.SetTexture("_CurTex", curTexture);
      this.materialAdapt.SetVector("_AdaptParams", new Vector4(x, this.limitMinimum, this.limitMaximum, 0.0f));
      Graphics.SetRenderTarget(this.adaptRenderTex[this.curAdaptIndex]);
      GL.Clear(false, true, Color.black);
      Graphics.Blit((Texture) this.adaptRenderTex[curAdaptIndex], this.adaptRenderTex[this.curAdaptIndex], this.materialAdapt);
    }
  }
}
