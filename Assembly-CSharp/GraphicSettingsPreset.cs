using Engine.Source.Commons;
using Engine.Source.Settings;
using UnityEngine;

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

  public bool AdditionalReflections => this.additionalReflections;

  public float AILodDistance => this.aiLodDistance;

  public bool AnisotropicFiltering => this.anisotropicFiltering;

  public bool Antialiasing => this.antialiasing;

  public bool AOFields => this.aoFields;

  public bool ContactShadows => this.contactShadows;

  public bool DOF => this.dof;

  public float LevelOfDetails => this.levelOfDetails;

  public bool MotionBlur => this.motionBlur;

  public float ShadowDistance => this.shadowDistance;

  public int ShadowQuality => this.shadowQuality;

  public bool SoftParticles => this.softParticles;

  public bool SSAO => this.ssao;

  public bool SSR => this.ssr;

  public int VolumetricLighting => this.volumetricLighting;

  public void Apply()
  {
    GraphicsGameSettings instance = InstanceByRequest<GraphicsGameSettings>.Instance;
    instance.AdditionalReflections.Value = this.additionalReflections;
    instance.AILodDistance.Value = this.aiLodDistance;
    instance.AnisotropicFiltering.Value = this.anisotropicFiltering;
    instance.Antialiasing.Value = this.antialiasing;
    instance.AOFields.Value = this.aoFields;
    instance.ContactShadows.Value = this.contactShadows;
    instance.DOF.Value = this.dof;
    instance.LevelOfDetails.Value = this.levelOfDetails;
    instance.MotionBlur.Value = this.motionBlur;
    instance.ShadowDistance.Value = this.shadowDistance;
    instance.ShadowQuality.Value = this.shadowQuality;
    instance.SoftParticles.Value = this.softParticles;
    instance.SSAO.Value = this.ssao;
    instance.SSR.Value = this.ssr;
    instance.VolumetricLighting.Value = this.volumetricLighting;
    instance.Apply();
  }

  public bool IsCurrent()
  {
    GraphicsGameSettings instance = InstanceByRequest<GraphicsGameSettings>.Instance;
    return instance.AdditionalReflections.Value == this.additionalReflections && (double) instance.AILodDistance.Value == (double) this.aiLodDistance && instance.AnisotropicFiltering.Value == this.anisotropicFiltering && instance.Antialiasing.Value == this.antialiasing && instance.AOFields.Value == this.aoFields && instance.ContactShadows.Value == this.contactShadows && instance.DOF.Value == this.dof && (double) instance.LevelOfDetails.Value == (double) this.levelOfDetails && instance.MotionBlur.Value == this.motionBlur && (double) instance.ShadowDistance.Value == (double) this.shadowDistance && instance.ShadowQuality.Value == this.shadowQuality && instance.SoftParticles.Value == this.softParticles && instance.SSAO.Value == this.ssao && instance.SSR.Value == this.ssr && instance.VolumetricLighting.Value == this.volumetricLighting;
  }
}
