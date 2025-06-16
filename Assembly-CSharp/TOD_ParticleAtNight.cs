// Decompiled with JetBrains decompiler
// Type: TOD_ParticleAtNight
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[RequireComponent(typeof (ParticleSystem))]
public class TOD_ParticleAtNight : MonoBehaviour
{
  public float fadeTime = 1f;
  private float lerpTime = 0.0f;
  private ParticleSystem particleComponent;
  private float particleEmission;

  protected void Start()
  {
    this.particleComponent = this.GetComponent<ParticleSystem>();
    this.particleEmission = this.particleComponent.emissionRate;
    this.particleComponent.emissionRate = TOD_Sky.Instance.IsNight ? this.particleEmission : 0.0f;
  }

  protected void Update()
  {
    this.lerpTime = Mathf.Clamp01(this.lerpTime + (TOD_Sky.Instance.IsNight ? 1f : -1f) * Time.deltaTime / this.fadeTime);
    this.particleComponent.emissionRate = Mathf.Lerp(0.0f, this.particleEmission, this.lerpTime);
  }
}
