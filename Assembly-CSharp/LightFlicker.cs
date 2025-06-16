// Decompiled with JetBrains decompiler
// Type: LightFlicker
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class LightFlicker : MonoBehaviour
{
  private float baseIntensity;
  public bool flicker = true;
  public float flickerIntensity = 0.5f;
  private Light lightComp;

  private void Awake()
  {
    this.lightComp = this.gameObject.GetComponent<Light>();
    this.baseIntensity = this.lightComp.intensity;
  }

  private void Update()
  {
    if (!this.flicker)
      return;
    this.lightComp.intensity = Mathf.Lerp(this.baseIntensity - this.flickerIntensity, this.baseIntensity, Mathf.PerlinNoise(Random.Range(0.0f, 1000f), Time.time));
  }
}
