using Cinemachine.PostFX;
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
    postFx = this.GetComponent<CinemachinePostFX>();
    if ((Object) postFx == (Object) null || (Object) postFx.m_Profile == (Object) null)
      return;
    ColorGradingModel colorGrading = postFx.m_Profile.colorGrading;
    ColorGradingModel.Settings settings = colorGrading.settings;
    settings.basic.postExposure = -10f * DarkenLevel;
    colorGrading.settings = settings;
  }
}
