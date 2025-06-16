// Decompiled with JetBrains decompiler
// Type: ShadowSettings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
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
