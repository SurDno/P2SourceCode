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
    private int curAdaptIndex;
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
        if ((Object) m_materialLum == (Object) null)
        {
          m_materialLum = new Material(shaderLum);
          m_materialLum.hideFlags = HideFlags.HideAndDontSave;
        }
        return m_materialLum;
      }
    }

    protected Material materialReduce
    {
      get
      {
        if ((Object) m_materialReduce == (Object) null)
        {
          m_materialReduce = new Material(shaderReduce);
          m_materialReduce.hideFlags = HideFlags.HideAndDontSave;
        }
        return m_materialReduce;
      }
    }

    protected Material materialAdapt
    {
      get
      {
        if ((Object) m_materialAdapt == (Object) null)
        {
          m_materialAdapt = new Material(shaderAdapt);
          m_materialAdapt.hideFlags = HideFlags.HideAndDontSave;
        }
        return m_materialAdapt;
      }
    }

    protected Material materialApply
    {
      get
      {
        if ((Object) m_materialApply == (Object) null)
        {
          m_materialApply = new Material(shaderApply);
          m_materialApply.hideFlags = HideFlags.HideAndDontSave;
        }
        return m_materialApply;
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
        if (shaderAdapt.isSupported && shaderApply.isSupported && shaderLum.isSupported && shaderReduce.isSupported)
          return;
        this.enabled = false;
      }
    }

    private void OnEnable()
    {
      for (int index = 0; index < 2; ++index)
      {
        if (!(bool) (Object) adaptRenderTex[index])
        {
          adaptRenderTex[index] = new RenderTexture(1, 1, 0);
          adaptRenderTex[index].hideFlags = HideFlags.HideAndDontSave;
        }
      }
    }

    private void OnDisable()
    {
      for (int index = 0; index < 2; ++index)
      {
        Object.DestroyImmediate((Object) adaptRenderTex[index]);
        adaptRenderTex[index] = (RenderTexture) null;
      }
      if ((bool) (Object) m_materialLum)
        Object.DestroyImmediate((Object) m_materialLum);
      if ((bool) (Object) m_materialReduce)
        Object.DestroyImmediate((Object) m_materialReduce);
      if ((bool) (Object) m_materialAdapt)
        Object.DestroyImmediate((Object) m_materialAdapt);
      if (!(bool) (Object) m_materialApply)
        return;
      Object.DestroyImmediate((Object) m_materialApply);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      RenderTexture renderTexture = RenderTexture.GetTemporary(source.width / 1, source.height / 1);
      Graphics.Blit((Texture) source, renderTexture, materialLum);
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
        Graphics.Blit((Texture) renderTexture, temporary, materialReduce);
        RenderTexture.ReleaseTemporary(renderTexture);
      }
      CalculateAdaptation((Texture) renderTexture);
      materialApply.SetTexture("_AdaptTex", (Texture) adaptRenderTex[curAdaptIndex]);
      Graphics.Blit((Texture) source, destination, materialApply);
      RenderTexture.ReleaseTemporary(renderTexture);
    }

    private void CalculateAdaptation(Texture curTexture)
    {
      int curAdaptIndex = this.curAdaptIndex;
      this.curAdaptIndex = (this.curAdaptIndex + 1) % 2;
      float x = Mathf.Clamp(1f - Mathf.Pow(1f - adaptationSpeed, 30f * Time.deltaTime), 0.01f, 1f);
      materialAdapt.SetTexture("_CurTex", curTexture);
      materialAdapt.SetVector("_AdaptParams", new Vector4(x, limitMinimum, limitMaximum, 0.0f));
      Graphics.SetRenderTarget(adaptRenderTex[this.curAdaptIndex]);
      GL.Clear(false, true, Color.black);
      Graphics.Blit((Texture) adaptRenderTex[curAdaptIndex], adaptRenderTex[this.curAdaptIndex], materialAdapt);
    }
  }
}
