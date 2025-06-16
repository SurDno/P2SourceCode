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
  private bool meshAssigned;

  private void Awake()
  {
    if ((Object) flameSystem != (Object) null)
    {
      ParticleSystem.MinMaxCurve minMaxCurve = flameSystem.emission.rateOverTime;
      flameMaxRate = minMaxCurve.constant;
      ParticleSystem.MainModule main = flameSystem.main;
      minMaxCurve = main.startSize;
      flameMaxSize = minMaxCurve.constant;
      flameMaxTime = main.startLifetime.constant;
    }
    if ((Object) smokeSystem != (Object) null)
    {
      smokeMaxRate = smokeSystem.emission.rateOverTime.constant;
      smokeMaxAlpha = smokeSystem.main.startColor.color.a;
    }
    if ((Object) sparkSystem != (Object) null)
      sparkMaxRate = sparkSystem.emission.rateOverTime.constant;
    UpdateSystems();
  }

  private void Update() => UpdateSystems();

  private void UpdateSystems()
  {
    if ((Object) lastBurningRenderer != (Object) BurningRenderer || meshAssigned && (Object) BurningRenderer == (Object) null)
    {
      if ((Object) BurningRenderer == (Object) null)
        BurningRenderer = (Renderer) null;
      if (BurningRenderer is MeshRenderer)
      {
        ParticleSystem.ShapeModule shape;
        if ((Object) flameSystem != (Object) null)
          shape = flameSystem.shape with
          {
            shapeType = ParticleSystemShapeType.MeshRenderer,
            meshRenderer = (MeshRenderer) BurningRenderer,
            skinnedMeshRenderer = (SkinnedMeshRenderer) null
          };
        if ((Object) smokeSystem != (Object) null)
          shape = smokeSystem.shape with
          {
            shapeType = ParticleSystemShapeType.MeshRenderer,
            meshRenderer = (MeshRenderer) BurningRenderer,
            skinnedMeshRenderer = (SkinnedMeshRenderer) null
          };
        if ((Object) sparkSystem != (Object) null)
          shape = sparkSystem.shape with
          {
            shapeType = ParticleSystemShapeType.MeshRenderer,
            meshRenderer = (MeshRenderer) BurningRenderer,
            skinnedMeshRenderer = (SkinnedMeshRenderer) null
          };
        meshAssigned = true;
      }
      else if (BurningRenderer is SkinnedMeshRenderer)
      {
        ParticleSystem.ShapeModule shape;
        if ((Object) flameSystem != (Object) null)
          shape = flameSystem.shape with
          {
            shapeType = ParticleSystemShapeType.SkinnedMeshRenderer,
            skinnedMeshRenderer = (SkinnedMeshRenderer) BurningRenderer,
            meshRenderer = (MeshRenderer) null
          };
        if ((Object) smokeSystem != (Object) null)
          shape = smokeSystem.shape with
          {
            shapeType = ParticleSystemShapeType.SkinnedMeshRenderer,
            skinnedMeshRenderer = (SkinnedMeshRenderer) BurningRenderer,
            meshRenderer = (MeshRenderer) null
          };
        if ((Object) sparkSystem != (Object) null)
          shape = sparkSystem.shape with
          {
            shapeType = ParticleSystemShapeType.SkinnedMeshRenderer,
            skinnedMeshRenderer = (SkinnedMeshRenderer) BurningRenderer,
            meshRenderer = (MeshRenderer) null
          };
        meshAssigned = true;
      }
      else
      {
        if ((Object) BurningRenderer != (Object) null)
          Debug.LogError((object) ("BurningRenderer can only be null, MeshRenderer or SkinnedMeshRenderer, : " + BurningRenderer.GetInfo()));
        BurningRenderer = (Renderer) null;
        ParticleSystem.ShapeModule shape;
        if ((Object) flameSystem != (Object) null)
          shape = flameSystem.shape with
          {
            shapeType = ParticleSystemShapeType.Sphere,
            meshRenderer = (MeshRenderer) null,
            skinnedMeshRenderer = (SkinnedMeshRenderer) null
          };
        if ((Object) smokeSystem != (Object) null)
          shape = smokeSystem.shape with
          {
            shapeType = ParticleSystemShapeType.Sphere,
            meshRenderer = (MeshRenderer) null,
            skinnedMeshRenderer = (SkinnedMeshRenderer) null
          };
        if ((Object) sparkSystem != (Object) null)
          shape = sparkSystem.shape with
          {
            shapeType = ParticleSystemShapeType.Sphere,
            meshRenderer = (MeshRenderer) null,
            skinnedMeshRenderer = (SkinnedMeshRenderer) null
          };
        meshAssigned = false;
      }
      lastBurningRenderer = BurningRenderer;
    }
    float num1 = (Object) lastBurningRenderer != (Object) null ? Strength : 0.0f;
    if (num1 == (double) lastStrength && lastSizeMultiplier == (double) SizeMultiplier)
      return;
    lastStrength = num1;
    lastSizeMultiplier = SizeMultiplier;
    if (lastStrength == 0.0)
    {
      if ((Object) flameSystem != (Object) null)
        flameSystem.emission.enabled = false;
      if ((Object) smokeSystem != (Object) null)
        smokeSystem.emission.enabled = false;
      if ((Object) sparkSystem != (Object) null)
        sparkSystem.emission.enabled = false;
    }
    else
    {
      ParticleSystem.EmissionModule emission;
      ParticleSystem.MainModule main;
      if ((Object) flameSystem != (Object) null)
      {
        emission = flameSystem.emission with
        {
          enabled = true,
          rateOverTime = (ParticleSystem.MinMaxCurve) (lastStrength * flameMaxRate * lastSizeMultiplier)
        };
        main = flameSystem.main;
        float num2 = (float) (0.25 + lastStrength * 0.75);
        main.startSize = (ParticleSystem.MinMaxCurve) (num2 * flameMaxSize);
        main.startLifetime = (ParticleSystem.MinMaxCurve) (num2 * flameMaxTime);
      }
      if ((Object) smokeSystem != (Object) null)
      {
        emission = smokeSystem.emission with
        {
          enabled = true,
          rateOverTime = (ParticleSystem.MinMaxCurve) (smokeMaxRate * lastSizeMultiplier)
        };
        main = smokeSystem.main;
        Color color = main.startColor.color with
        {
          a = lastStrength * smokeMaxAlpha
        };
        main.startColor = (ParticleSystem.MinMaxGradient) color;
      }
      if ((Object) sparkSystem != (Object) null)
        emission = sparkSystem.emission with
        {
          enabled = true,
          rateOverTime = (ParticleSystem.MinMaxCurve) (lastStrength * sparkMaxRate * lastSizeMultiplier)
        };
    }
  }
}
