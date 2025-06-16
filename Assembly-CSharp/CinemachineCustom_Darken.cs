// Decompiled with JetBrains decompiler
// Type: CinemachineCustom_Darken
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cinemachine.PostFX;
using UnityEngine;
using UnityEngine.PostProcessing;

#nullable disable
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
