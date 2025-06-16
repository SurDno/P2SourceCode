using UnityEngine;

[CreateAssetMenu(menuName = "Data/Shadow Settings")]
public class ShadowSettings : ScriptableObject
{
  [SerializeField]
  private ShadowQuality shadows = ShadowQuality.All;
  [SerializeField]
  private ShadowResolution shadowResolution = ShadowResolution.Medium;
  [Space]
  [SerializeField]
  [Range(1f, 128f)]
  private int NgssTestSamplers = 3;
  [SerializeField]
  [Range(1f, 128f)]
  private int NgssFilterSamplers = 6;

  public void Apply()
  {
    QualitySettings.shadows = this.shadows;
    QualitySettings.shadowResolution = this.shadowResolution;
    NGSS_Directional instance = MonoBehaviourInstance<NGSS_Directional>.Instance;
    if (!((Object) instance != (Object) null))
      return;
    instance.NGSS_FILTER_SAMPLERS = this.NgssFilterSamplers;
    instance.NGSS_TEST_SAMPLERS = this.NgssTestSamplers;
  }
}
