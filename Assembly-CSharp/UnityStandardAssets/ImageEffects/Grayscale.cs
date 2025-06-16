// Decompiled with JetBrains decompiler
// Type: UnityStandardAssets.ImageEffects.Grayscale
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace UnityStandardAssets.ImageEffects
{
  [ExecuteInEditMode]
  [AddComponentMenu("Image Effects/Color Adjustments/Grayscale")]
  public class Grayscale : ImageEffectBase
  {
    public Texture textureRamp;
    [Range(-1f, 1f)]
    public float rampOffset;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      this.material.SetTexture("_RampTex", this.textureRamp);
      this.material.SetFloat("_RampOffset", this.rampOffset);
      Graphics.Blit((Texture) source, destination, this.material);
    }
  }
}
