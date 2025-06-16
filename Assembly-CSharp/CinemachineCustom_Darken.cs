using Cinemachine.PostFX;
using UnityEngine;
using UnityEngine.PostProcessing;

[ExecuteInEditMode]
public class CinemachineCustom_Darken : MonoBehaviour
{
  public float DarkenLevel;
  private CinemachinePostFX postFx;

  private void Awake()
  {
  }

  private void Update()
  {
    this.postFx = this.GetComponent<CinemachinePostFX>();
    if ((Object) this.postFx == (Object) null || (Object) this.postFx.m_Profile == (Object) null)
      return;
    ColorGradingModel colorGrading = this.postFx.m_Profile.colorGrading;
    ColorGradingModel.Settings settings = colorGrading.settings;
    settings.basic.postExposure = -10f * this.DarkenLevel;
    colorGrading.settings = settings;
  }
}
