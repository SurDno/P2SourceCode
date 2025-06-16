using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
  [ExecuteInEditMode]
  [AddComponentMenu("Image Effects/Blur/Motion Blur (Color Accumulation)")]
  [RequireComponent(typeof (Camera))]
  public class MotionBlur : ImageEffectBase
  {
    [Range(0.0f, 0.92f)]
    public float blurAmount = 0.8f;
    public bool extraBlur;
    private RenderTexture accumTexture;

    protected override void Start()
    {
      if (!SystemInfo.supportsRenderTextures)
        enabled = false;
      else
        base.Start();
    }

    protected override void OnDisable()
    {
      base.OnDisable();
      DestroyImmediate(accumTexture);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      if (accumTexture == null || accumTexture.width != source.width || accumTexture.height != source.height)
      {
        DestroyImmediate(accumTexture);
        accumTexture = new RenderTexture(source.width, source.height, 0);
        accumTexture.hideFlags = HideFlags.HideAndDontSave;
        Graphics.Blit(source, accumTexture);
      }
      if (extraBlur)
      {
        RenderTexture temporary = RenderTexture.GetTemporary(source.width / 4, source.height / 4, 0);
        accumTexture.MarkRestoreExpected();
        Graphics.Blit(accumTexture, temporary);
        Graphics.Blit(temporary, accumTexture);
        RenderTexture.ReleaseTemporary(temporary);
      }
      blurAmount = Mathf.Clamp(blurAmount, 0.0f, 0.92f);
      material.SetTexture("_MainTex", accumTexture);
      material.SetFloat("_AccumOrig", 1f - blurAmount);
      accumTexture.MarkRestoreExpected();
      Graphics.Blit(source, accumTexture, material);
      Graphics.Blit(accumTexture, destination);
    }
  }
}
