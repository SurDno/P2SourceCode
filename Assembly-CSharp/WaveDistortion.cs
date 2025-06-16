// Decompiled with JetBrains decompiler
// Type: WaveDistortion
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityStandardAssets.ImageEffects;

#nullable disable
[ExecuteInEditMode]
public class WaveDistortion : ImageEffectBase
{
  [SerializeField]
  private float intensity = 0.0f;
  [SerializeField]
  private float noiseScale = 1f;
  [SerializeField]
  private float noiseSpeed = 1f;

  public float Intensity
  {
    get => this.intensity;
    set => this.intensity = value;
  }

  private void OnRenderImage(RenderTexture source, RenderTexture destination)
  {
    if ((double) this.intensity == 0.0)
    {
      Graphics.Blit((Texture) source, destination);
    }
    else
    {
      this.material.SetFloat("_Intensity", this.intensity);
      this.material.SetFloat("_NoiseScale", this.noiseScale);
      this.material.SetFloat("_NoiseSpeed", this.noiseSpeed);
      Graphics.Blit((Texture) source, destination, this.material);
    }
  }
}
