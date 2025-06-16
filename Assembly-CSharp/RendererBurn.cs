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
  public float Strength = 0.0f;
  public Renderer BurningRenderer = (Renderer) null;
  [Tooltip("Множитель количества частиц, для объектов разных размеров.")]
  public float SizeMultiplier = 1f;
  private float flameMaxRate;
  private float flameMaxSize;
  private float flameMaxTime;
  private float smokeMaxAlpha;
  private float smokeMaxRate;
  private float sparkMaxRate;
  private Renderer lastBurningRenderer = (Renderer) null;
  private float lastStrength = 1f;
  private float lastSizeMultiplier = 1f;
  private bool meshAssigned = false;

  private void Awake()
  {
    if ((Object) this.flameSystem != (Object) null)
    {
      ParticleSystem.MinMaxCurve minMaxCurve = this.flameSystem.emission.rateOverTime;
      this.flameMaxRate = minMaxCurve.constant;
      ParticleSystem.MainModule main = this.flameSystem.main;
      minMaxCurve = main.startSize;
      this.flameMaxSize = minMaxCurve.constant;
      this.flameMaxTime = main.startLifetime.constant;
    }
    if ((Object) this.smokeSystem != (Object) null)
    {
      this.smokeMaxRate = this.smokeSystem.emission.rateOverTime.constant;
      this.smokeMaxAlpha = this.smokeSystem.main.startColor.color.a;
    }
    if ((Object) this.sparkSystem != (Object) null)
      this.sparkMaxRate = this.sparkSystem.emission.rateOverTime.constant;
    this.UpdateSystems();
  }

  private void Update() => this.UpdateSystems();

  private void UpdateSystems()
  {
    if ((Object) this.lastBurningRenderer != (Object) this.BurningRenderer || this.meshAssigned && (Object) this.BurningRenderer == (Object) null)
    {
      if ((Object) this.BurningRenderer == (Object) null)
        this.BurningRenderer = (Renderer) null;
      if (this.BurningRenderer is MeshRenderer)
      {
        ParticleSystem.ShapeModule shape;
        if ((Object) this.flameSystem != (Object) null)
          shape = this.flameSystem.shape with
          {
            shapeType = ParticleSystemShapeType.MeshRenderer,
            meshRenderer = (MeshRenderer) this.BurningRenderer,
            skinnedMeshRenderer = (SkinnedMeshRenderer) null
          };
        if ((Object) this.smokeSystem != (Object) null)
          shape = this.smokeSystem.shape with
          {
            shapeType = ParticleSystemShapeType.MeshRenderer,
            meshRenderer = (MeshRenderer) this.BurningRenderer,
            skinnedMeshRenderer = (SkinnedMeshRenderer) null
          };
        if ((Object) this.sparkSystem != (Object) null)
          shape = this.sparkSystem.shape with
          {
            shapeType = ParticleSystemShapeType.MeshRenderer,
            meshRenderer = (MeshRenderer) this.BurningRenderer,
            skinnedMeshRenderer = (SkinnedMeshRenderer) null
          };
        this.meshAssigned = true;
      }
      else if (this.BurningRenderer is SkinnedMeshRenderer)
      {
        ParticleSystem.ShapeModule shape;
        if ((Object) this.flameSystem != (Object) null)
          shape = this.flameSystem.shape with
          {
            shapeType = ParticleSystemShapeType.SkinnedMeshRenderer,
            skinnedMeshRenderer = (SkinnedMeshRenderer) this.BurningRenderer,
            meshRenderer = (MeshRenderer) null
          };
        if ((Object) this.smokeSystem != (Object) null)
          shape = this.smokeSystem.shape with
          {
            shapeType = ParticleSystemShapeType.SkinnedMeshRenderer,
            skinnedMeshRenderer = (SkinnedMeshRenderer) this.BurningRenderer,
            meshRenderer = (MeshRenderer) null
          };
        if ((Object) this.sparkSystem != (Object) null)
          shape = this.sparkSystem.shape with
          {
            shapeType = ParticleSystemShapeType.SkinnedMeshRenderer,
            skinnedMeshRenderer = (SkinnedMeshRenderer) this.BurningRenderer,
            meshRenderer = (MeshRenderer) null
          };
        this.meshAssigned = true;
      }
      else
      {
        if ((Object) this.BurningRenderer != (Object) null)
          Debug.LogError((object) ("BurningRenderer can only be null, MeshRenderer or SkinnedMeshRenderer, : " + this.BurningRenderer.GetInfo()));
        this.BurningRenderer = (Renderer) null;
        ParticleSystem.ShapeModule shape;
        if ((Object) this.flameSystem != (Object) null)
          shape = this.flameSystem.shape with
          {
            shapeType = ParticleSystemShapeType.Sphere,
            meshRenderer = (MeshRenderer) null,
            skinnedMeshRenderer = (SkinnedMeshRenderer) null
          };
        if ((Object) this.smokeSystem != (Object) null)
          shape = this.smokeSystem.shape with
          {
            shapeType = ParticleSystemShapeType.Sphere,
            meshRenderer = (MeshRenderer) null,
            skinnedMeshRenderer = (SkinnedMeshRenderer) null
          };
        if ((Object) this.sparkSystem != (Object) null)
          shape = this.sparkSystem.shape with
          {
            shapeType = ParticleSystemShapeType.Sphere,
            meshRenderer = (MeshRenderer) null,
            skinnedMeshRenderer = (SkinnedMeshRenderer) null
          };
        this.meshAssigned = false;
      }
      this.lastBurningRenderer = this.BurningRenderer;
    }
    float num1 = (Object) this.lastBurningRenderer != (Object) null ? this.Strength : 0.0f;
    if ((double) num1 == (double) this.lastStrength && (double) this.lastSizeMultiplier == (double) this.SizeMultiplier)
      return;
    this.lastStrength = num1;
    this.lastSizeMultiplier = this.SizeMultiplier;
    if ((double) this.lastStrength == 0.0)
    {
      if ((Object) this.flameSystem != (Object) null)
        this.flameSystem.emission.enabled = false;
      if ((Object) this.smokeSystem != (Object) null)
        this.smokeSystem.emission.enabled = false;
      if ((Object) this.sparkSystem != (Object) null)
        this.sparkSystem.emission.enabled = false;
    }
    else
    {
      ParticleSystem.EmissionModule emission;
      ParticleSystem.MainModule main;
      if ((Object) this.flameSystem != (Object) null)
      {
        emission = this.flameSystem.emission with
        {
          enabled = true,
          rateOverTime = (ParticleSystem.MinMaxCurve) (this.lastStrength * this.flameMaxRate * this.lastSizeMultiplier)
        };
        main = this.flameSystem.main;
        float num2 = (float) (0.25 + (double) this.lastStrength * 0.75);
        main.startSize = (ParticleSystem.MinMaxCurve) (num2 * this.flameMaxSize);
        main.startLifetime = (ParticleSystem.MinMaxCurve) (num2 * this.flameMaxTime);
      }
      if ((Object) this.smokeSystem != (Object) null)
      {
        emission = this.smokeSystem.emission with
        {
          enabled = true,
          rateOverTime = (ParticleSystem.MinMaxCurve) (this.smokeMaxRate * this.lastSizeMultiplier)
        };
        main = this.smokeSystem.main;
        Color color = main.startColor.color with
        {
          a = this.lastStrength * this.smokeMaxAlpha
        };
        main.startColor = (ParticleSystem.MinMaxGradient) color;
      }
      if ((Object) this.sparkSystem != (Object) null)
        emission = this.sparkSystem.emission with
        {
          enabled = true,
          rateOverTime = (ParticleSystem.MinMaxCurve) (this.lastStrength * this.sparkMaxRate * this.lastSizeMultiplier)
        };
    }
  }
}
