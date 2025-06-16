using UnityEngine;

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
    get => strength;
    set => strength = Mathf.Clamp01(value);
  }

  public float DecalOpacity
  {
    get => decalOpacity;
    set => decalOpacity = Mathf.Clamp01(value);
  }

  public void Initialize(float strength)
  {
    this.strength = strength;
    randomSeed = new Vector4(strength, Random.value, Random.value, 0.0f);
    smokeMaxAlpha = smokeSystem.main.startColor.color.a;
    sparkMaxRate = sparkSystem.emission.rateOverTime.constant;
    mpb = new MaterialPropertyBlock();
    propertyId = Shader.PropertyToID("_Strength");
    ApplyStrength();
  }

  private void Update() => ApplyStrength();

  private void ApplyStrength()
  {
    if (Strength == 0.0)
    {
      GetComponent<MeshRenderer>().enabled = false;
      ParticleSystem.EmissionModule smokeEmission = smokeSystem.emission;
      smokeEmission.enabled = false;
      ParticleSystem.EmissionModule sparkEmission = sparkSystem.emission;
      sparkEmission.enabled = false;
    }
    else
    {
      randomSeed.x = Mathf.Sqrt(Strength);
      mpb.SetVector(propertyId, randomSeed);
      GetComponent<MeshRenderer>().SetPropertyBlock(mpb);
      GetComponent<MeshRenderer>().enabled = true;
      ParticleSystem.EmissionModule emission = smokeSystem.emission with
      {
        enabled = true
      };
      ParticleSystem.MainModule main = smokeSystem.main;
      Color color = main.startColor.color with
      {
        a = Strength * smokeMaxAlpha
      };
      main.startColor = color;
      emission = sparkSystem.emission with
      {
        enabled = true,
        rateOverTime = Strength * sparkMaxRate
      };
    }
    if (decalOpacity > 0.0)
    {
      decal.Properties[0].Value = decalOpacity;
      if (decal.enabled)
        return;
      decal.enabled = true;
    }
    else if (decal.enabled)
      decal.enabled = false;
  }
}
