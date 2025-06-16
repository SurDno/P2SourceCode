namespace UnityStandardAssets.ImageEffects
{
  [ExecuteInEditMode]
  [AddComponentMenu("Image Effects/Blur/Motion Blur (Color Accumulation)")]
  [RequireComponent(typeof (Camera))]
  public class MotionBlur : ImageEffectBase
  {
    [Range(0.0f, 0.92f)]
    public float blurAmount = 0.8f;
    public bool extraBlur = false;
    private RenderTexture accumTexture;

    protected override void Start()
    {
      if (!SystemInfo.supportsRenderTextures)
        this.enabled = false;
      else
        base.Start();
    }

    protected override void OnDisable()
    {
      base.OnDisable();
      Object.DestroyImmediate((Object) accumTexture);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      if ((Object) accumTexture == (Object) null || accumTexture.width != source.width || accumTexture.height != source.height)
      {
        Object.DestroyImmediate((Object) accumTexture);
        accumTexture = new RenderTexture(source.width, source.height, 0);
        accumTexture.hideFlags = HideFlags.HideAndDontSave;
        Graphics.Blit((Texture) source, accumTexture);
      }
      if (extraBlur)
      {
        RenderTexture temporary = RenderTexture.GetTemporary(source.width / 4, source.height / 4, 0);
        accumTexture.MarkRestoreExpected();
        Graphics.Blit((Texture) accumTexture, temporary);
        Graphics.Blit((Texture) temporary, accumTexture);
        RenderTexture.ReleaseTemporary(temporary);
      }
      blurAmount = Mathf.Clamp(blurAmount, 0.0f, 0.92f);
      material.SetTexture("_MainTex", (Texture) accumTexture);
      material.SetFloat("_AccumOrig", 1f - blurAmount);
      accumTexture.MarkRestoreExpected();
      Graphics.Blit((Texture) source, accumTexture, material);
      Graphics.Blit((Texture) accumTexture, destination);
    }
  }
}
