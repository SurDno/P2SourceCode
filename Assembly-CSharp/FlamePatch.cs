// Decompiled with JetBrains decompiler
// Type: FlamePatch
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class FlamePatch : MonoBehaviour
{
  [SerializeField]
  private ParticleSystem smokeSystem;
  [SerializeField]
  private ParticleSystem sparkSystem;
  [SerializeField]
  private DeferredProjector decal;
  [SerializeField]
  private float strength = 1f;
  [SerializeField]
  private float decalOpacity = 1f;
  private MaterialPropertyBlock mpb;
  private int propertyId;
  private float smokeMaxAlpha;
  private float sparkMaxRate;
  private Vector4 randomSeed;

  public float Strength
  {
    get => this.strength;
    set => this.strength = Mathf.Clamp01(value);
  }

  public float DecalOpacity
  {
    get => this.decalOpacity;
    set => this.decalOpacity = Mathf.Clamp01(value);
  }

  public void Initialize(float strength)
  {
    this.strength = strength;
    this.randomSeed = new Vector4(strength, Random.value, Random.value, 0.0f);
    this.smokeMaxAlpha = this.smokeSystem.main.startColor.color.a;
    this.sparkMaxRate = this.sparkSystem.emission.rateOverTime.constant;
    this.mpb = new MaterialPropertyBlock();
    this.propertyId = Shader.PropertyToID("_Strength");
    this.ApplyStrength();
  }

  private void Update() => this.ApplyStrength();

  private void ApplyStrength()
  {
    if ((double) this.Strength == 0.0)
    {
      this.GetComponent<MeshRenderer>().enabled = false;
      this.smokeSystem.emission.enabled = false;
      this.sparkSystem.emission.enabled = false;
    }
    else
    {
      this.randomSeed.x = Mathf.Sqrt(this.Strength);
      this.mpb.SetVector(this.propertyId, this.randomSeed);
      this.GetComponent<MeshRenderer>().SetPropertyBlock(this.mpb);
      this.GetComponent<MeshRenderer>().enabled = true;
      ParticleSystem.EmissionModule emission = this.smokeSystem.emission with
      {
        enabled = true
      };
      ParticleSystem.MainModule main = this.smokeSystem.main;
      Color color = main.startColor.color with
      {
        a = this.Strength * this.smokeMaxAlpha
      };
      main.startColor = (ParticleSystem.MinMaxGradient) color;
      emission = this.sparkSystem.emission with
      {
        enabled = true,
        rateOverTime = (ParticleSystem.MinMaxCurve) (this.Strength * this.sparkMaxRate)
      };
    }
    if ((double) this.decalOpacity > 0.0)
    {
      this.decal.Properties[0].Value = this.decalOpacity;
      if (this.decal.enabled)
        return;
      this.decal.enabled = true;
    }
    else if (this.decal.enabled)
      this.decal.enabled = false;
  }
}
