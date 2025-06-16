// Decompiled with JetBrains decompiler
// Type: TOD_LightAtDay
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[RequireComponent(typeof (Light))]
public class TOD_LightAtDay : MonoBehaviour
{
  public float fadeTime = 1f;
  private float lerpTime = 0.0f;
  private Light lightComponent;
  private float lightIntensity;

  protected void Start()
  {
    this.lightComponent = this.GetComponent<Light>();
    this.lightIntensity = this.lightComponent.intensity;
    this.lightComponent.enabled = TOD_Sky.Instance.IsDay;
  }

  protected void Update()
  {
    this.lerpTime = Mathf.Clamp01(this.lerpTime + (TOD_Sky.Instance.IsDay ? 1f : -1f) * Time.deltaTime / this.fadeTime);
    this.lightComponent.intensity = Mathf.Lerp(0.0f, this.lightIntensity, this.lerpTime);
    this.lightComponent.enabled = (double) this.lightComponent.intensity > 0.0;
  }
}
