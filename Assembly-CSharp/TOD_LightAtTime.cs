// Decompiled with JetBrains decompiler
// Type: TOD_LightAtTime
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[RequireComponent(typeof (Light))]
public class TOD_LightAtTime : MonoBehaviour
{
  public AnimationCurve Intensity = new AnimationCurve()
  {
    keys = new Keyframe[3]
    {
      new Keyframe(0.0f, 0.0f),
      new Keyframe(12f, 1f),
      new Keyframe(24f, 0.0f)
    }
  };
  private Light lightComponent;

  protected void Start() => this.lightComponent = this.GetComponent<Light>();

  protected void Update()
  {
    this.lightComponent.intensity = this.Intensity.Evaluate(TOD_Sky.Instance.Cycle.Hour);
    this.lightComponent.enabled = (double) this.lightComponent.intensity > 0.0;
  }
}
