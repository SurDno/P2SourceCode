// Decompiled with JetBrains decompiler
// Type: TOD_ParticleAtTime
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[RequireComponent(typeof (ParticleSystem))]
public class TOD_ParticleAtTime : MonoBehaviour
{
  public AnimationCurve Emission = new AnimationCurve()
  {
    keys = new Keyframe[3]
    {
      new Keyframe(0.0f, 0.0f),
      new Keyframe(12f, 1f),
      new Keyframe(24f, 0.0f)
    }
  };
  private ParticleSystem particleComponent;

  protected void Start() => this.particleComponent = this.GetComponent<ParticleSystem>();

  protected void Update()
  {
    this.particleComponent.emissionRate = this.Emission.Evaluate(TOD_Sky.Instance.Cycle.Hour);
  }
}
