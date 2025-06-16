using Engine.Source.Commons;
using Engine.Source.Settings;

[CreateAssetMenu(menuName = "Data/Graphic Settings Preset")]
public class GraphicSettingsPreset : ScriptableObject
{
  [SerializeField]
  private bool additionalReflections;
  [SerializeField]
  private float aiLodDistance;
  [SerializeField]
  private bool anisotropicFiltering;
  [SerializeField]
  private bool antialiasing;
  [SerializeField]
  private bool aoFields;
  [SerializeField]
  private bool contactShadows;
  [SerializeField]
  private bool dof;
  [SerializeField]
  [Range(0.5f, 2f)]
  private float levelOfDetails;
  [SerializeField]
  private bool motionBlur;
  [SerializeField]
  [Range(5f, 75f)]
  private float shadowDistance;
  [SerializeField]
  private int shadowQuality;
  [SerializeField]
  private bool softParticles;
  [SerializeField]
  private bool ssao;
  [SerializeField]
  private bool ssr;
  [SerializeField]
  [Range(0.0f, 3f)]
  private int volumetricLighting;

  public bool AdditionalReflections => additionalReflections;

  public float AILodDistance => aiLodDistance;

  public bool AnisotropicFiltering => anisotropicFiltering;

  public bool Antialiasing => antialiasing;

  public bool AOFields => aoFields;

  public bool ContactShadows => contactShadows;

  public bool DOF => dof;

  public float LevelOfDetails => levelOfDetails;

  public bool MotionBlur => motionBlur;

  public float ShadowDistance => shadowDistance;

  public int ShadowQuality => shadowQuality;

  public bool SoftParticles => softParticles;

  public bool SSAO => ssao;

  public bool SSR => ssr;

  public int VolumetricLighting => volumetricLighting;

  public void Apply()
  {
    GraphicsGameSettings instance = InstanceByRequest<GraphicsGameSettings>.Instance;
    instance.AdditionalReflections.Value = additionalReflections;
    instance.AILodDistance.Value = aiLodDistance;
    instance.AnisotropicFiltering.Value = anisotropicFiltering;
    instance.Antialiasing.Value = antialiasing;
    instance.AOFields.Value = aoFields;
    instance.ContactShadows.Value = contactShadows;
    instance.DOF.Value = dof;
    instance.LevelOfDetails.Value = levelOfDetails;
    instance.MotionBlur.Value = motionBlur;
    instance.ShadowDistance.Value = shadowDistance;
    instance.ShadowQuality.Value = shadowQuality;
    instance.SoftParticles.Value = softParticles;
    instance.SSAO.Value = ssao;
    instance.SSR.Value = ssr;
    instance.VolumetricLighting.Value = volumetricLighting;
    instance.Apply();
  }

  public bool IsCurrent()
  {
    GraphicsGameSettings instance = InstanceByRequest<GraphicsGameSettings>.Instance;
    return instance.AdditionalReflections.Value == additionalReflections && instance.AILodDistance.Value == (double) aiLodDistance && instance.AnisotropicFiltering.Value == anisotropicFiltering && instance.Antialiasing.Value == antialiasing && instance.AOFields.Value == aoFields && instance.ContactShadows.Value == contactShadows && instance.DOF.Value == dof && instance.LevelOfDetails.Value == (double) levelOfDetails && instance.MotionBlur.Value == motionBlur && instance.ShadowDistance.Value == (double) shadowDistance && instance.ShadowQuality.Value == shadowQuality && instance.SoftParticles.Value == softParticles && instance.SSAO.Value == ssao && instance.SSR.Value == ssr && instance.VolumetricLighting.Value == volumetricLighting;
  }
}
