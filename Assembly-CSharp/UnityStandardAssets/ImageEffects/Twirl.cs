// Decompiled with JetBrains decompiler
// Type: UnityStandardAssets.ImageEffects.Twirl
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace UnityStandardAssets.ImageEffects
{
  [ExecuteInEditMode]
  [AddComponentMenu("Image Effects/Displacement/Twirl")]
  public class Twirl : ImageEffectBase
  {
    public Vector2 radius = new Vector2(0.3f, 0.3f);
    [Range(0.0f, 360f)]
    public float angle = 50f;
    public Vector2 center = new Vector2(0.5f, 0.5f);

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      UnityStandardAssets.ImageEffects.ImageEffects.RenderDistortion(this.material, source, destination, this.angle, this.center, this.radius);
    }
  }
}
