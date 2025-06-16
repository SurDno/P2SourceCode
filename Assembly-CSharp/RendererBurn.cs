using UnityEngine;
using UnityEngine.Serialization;

public class RendererBurn : MonoBehaviour
{
  [SerializeField]
  [FormerlySerializedAs("FlameSystem")]
  private ParticleSystem flameSystem;
  [SerializeField]
  [FormerlySerializedAs("SmokeSystem")]
  private ParticleSystem smokeSystem;
  [SerializeField]
  [FormerlySerializedAs("SparkSystem")]
  private ParticleSystem sparkSystem;
  [Space]
  [Range(0.0f, 1f)]
  public float Strength;
  public Renderer BurningRenderer;
  [Tooltip("Множитель количества частиц, для объектов разных размеров.")]
  public float SizeMultiplier = 1f;
  private float flameMaxRate;
  private float flameMaxSize;
  private float flameMaxTime;
  private float smokeMaxAlpha;
  private float smokeMaxRate;
  private float sparkMaxRate;
  private Renderer lastBurningRenderer;
  private float lastStrength = 1f;
  private float lastSizeMultiplier = 1f;
  private bool meshAssigned;

  private void Awake()
  {
    if (flameSystem != null)
    {
      ParticleSystem.MinMaxCurve minMaxCurve = flameSystem.emission.rateOverTime;
      flameMaxRate = minMaxCurve.constant;
      ParticleSystem.MainModule main = flameSystem.main;
      minMaxCurve = main.startSize;
      flameMaxSize = minMaxCurve.constant;
      flameMaxTime = main.startLifetime.constant;
    }
    if (smokeSystem != null)
    {
      smokeMaxRate = smokeSystem.emission.rateOverTime.constant;
      smokeMaxAlpha = smokeSystem.main.startColor.color.a;
    }
    if (sparkSystem != null)
      sparkMaxRate = sparkSystem.emission.rateOverTime.constant;
    UpdateSystems();
  }

  private void Update() => UpdateSystems();

  private void UpdateSystems()
  {
    if (lastBurningRenderer != BurningRenderer || meshAssigned && BurningRenderer == null)
    {
      if (BurningRenderer == null)
        BurningRenderer = null;
      if (BurningRenderer is MeshRenderer)
      {
        ParticleSystem.ShapeModule shape;
        if (flameSystem != null)
          shape = flameSystem.shape with
          {
            shapeType = ParticleSystemShapeType.MeshRenderer,
            meshRenderer = (MeshRenderer) BurningRenderer,
            skinnedMeshRenderer = null
          };
        if (smokeSystem != null)
          shape = smokeSystem.shape with
          {
            shapeType = ParticleSystemShapeType.MeshRenderer,
            meshRenderer = (MeshRenderer) BurningRenderer,
            skinnedMeshRenderer = null
          };
        if (sparkSystem != null)
          shape = sparkSystem.shape with
          {
            shapeType = ParticleSystemShapeType.MeshRenderer,
            meshRenderer = (MeshRenderer) BurningRenderer,
            skinnedMeshRenderer = null
          };
        meshAssigned = true;
      }
      else if (BurningRenderer is SkinnedMeshRenderer)
      {
        ParticleSystem.ShapeModule shape;
        if (flameSystem != null)
          shape = flameSystem.shape with
          {
            shapeType = ParticleSystemShapeType.SkinnedMeshRenderer,
            skinnedMeshRenderer = (SkinnedMeshRenderer) BurningRenderer,
            meshRenderer = null
          };
        if (smokeSystem != null)
          shape = smokeSystem.shape with
          {
            shapeType = ParticleSystemShapeType.SkinnedMeshRenderer,
            skinnedMeshRenderer = (SkinnedMeshRenderer) BurningRenderer,
            meshRenderer = null
          };
        if (sparkSystem != null)
          shape = sparkSystem.shape with
          {
            shapeType = ParticleSystemShapeType.SkinnedMeshRenderer,
            skinnedMeshRenderer = (SkinnedMeshRenderer) BurningRenderer,
            meshRenderer = null
          };
        meshAssigned = true;
      }
      else
      {
        if (BurningRenderer != null)
          Debug.LogError("BurningRenderer can only be null, MeshRenderer or SkinnedMeshRenderer, : " + BurningRenderer.GetInfo());
        BurningRenderer = null;
        ParticleSystem.ShapeModule shape;
        if (flameSystem != null)
          shape = flameSystem.shape with
          {
            shapeType = ParticleSystemShapeType.Sphere,
            meshRenderer = null,
            skinnedMeshRenderer = null
          };
        if (smokeSystem != null)
          shape = smokeSystem.shape with
          {
            shapeType = ParticleSystemShapeType.Sphere,
            meshRenderer = null,
            skinnedMeshRenderer = null
          };
        if (sparkSystem != null)
          shape = sparkSystem.shape with
          {
            shapeType = ParticleSystemShapeType.Sphere,
            meshRenderer = null,
            skinnedMeshRenderer = null
          };
        meshAssigned = false;
      }
      lastBurningRenderer = BurningRenderer;
    }
    float num1 = lastBurningRenderer != null ? Strength : 0.0f;
    if (num1 == (double) lastStrength && lastSizeMultiplier == (double) SizeMultiplier)
      return;
    lastStrength = num1;
    lastSizeMultiplier = SizeMultiplier;
    if (lastStrength == 0.0)
    {
      if (flameSystem != null)
        flameSystem.emission.enabled = false;
      if (smokeSystem != null)
        smokeSystem.emission.enabled = false;
      if (sparkSystem != null)
        sparkSystem.emission.enabled = false;
    }
    else
    {
      ParticleSystem.EmissionModule emission;
      ParticleSystem.MainModule main;
      if (flameSystem != null)
      {
        emission = flameSystem.emission with
        {
          enabled = true,
          rateOverTime = lastStrength * flameMaxRate * lastSizeMultiplier
        };
        main = flameSystem.main;
        float num2 = (float) (0.25 + lastStrength * 0.75);
        main.startSize = num2 * flameMaxSize;
        main.startLifetime = num2 * flameMaxTime;
      }
      if (smokeSystem != null)
      {
        emission = smokeSystem.emission with
        {
          enabled = true,
          rateOverTime = smokeMaxRate * lastSizeMultiplier
        };
        main = smokeSystem.main;
        Color color = main.startColor.color with
        {
          a = lastStrength * smokeMaxAlpha
        };
        main.startColor = color;
      }
      if (sparkSystem != null)
        emission = sparkSystem.emission with
        {
          enabled = true,
          rateOverTime = lastStrength * sparkMaxRate * lastSizeMultiplier
        };
    }
  }
}
