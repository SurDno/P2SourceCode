// Decompiled with JetBrains decompiler
// Type: UnityStandardAssets.ImageEffects.MotionBlur
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
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
      Object.DestroyImmediate((Object) this.accumTexture);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      if ((Object) this.accumTexture == (Object) null || this.accumTexture.width != source.width || this.accumTexture.height != source.height)
      {
        Object.DestroyImmediate((Object) this.accumTexture);
        this.accumTexture = new RenderTexture(source.width, source.height, 0);
        this.accumTexture.hideFlags = HideFlags.HideAndDontSave;
        Graphics.Blit((Texture) source, this.accumTexture);
      }
      if (this.extraBlur)
      {
        RenderTexture temporary = RenderTexture.GetTemporary(source.width / 4, source.height / 4, 0);
        this.accumTexture.MarkRestoreExpected();
        Graphics.Blit((Texture) this.accumTexture, temporary);
        Graphics.Blit((Texture) temporary, this.accumTexture);
        RenderTexture.ReleaseTemporary(temporary);
      }
      this.blurAmount = Mathf.Clamp(this.blurAmount, 0.0f, 0.92f);
      this.material.SetTexture("_MainTex", (Texture) this.accumTexture);
      this.material.SetFloat("_AccumOrig", 1f - this.blurAmount);
      this.accumTexture.MarkRestoreExpected();
      Graphics.Blit((Texture) source, this.accumTexture, this.material);
      Graphics.Blit((Texture) this.accumTexture, destination);
    }
  }
}
