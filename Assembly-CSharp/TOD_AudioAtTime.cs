// Decompiled with JetBrains decompiler
// Type: TOD_AudioAtTime
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[RequireComponent(typeof (AudioSource))]
public class TOD_AudioAtTime : MonoBehaviour
{
  public AnimationCurve Volume = new AnimationCurve()
  {
    keys = new Keyframe[3]
    {
      new Keyframe(0.0f, 0.0f),
      new Keyframe(12f, 1f),
      new Keyframe(24f, 0.0f)
    }
  };
  private AudioSource audioComponent;

  protected void Start() => this.audioComponent = this.GetComponent<AudioSource>();

  protected void Update()
  {
    this.audioComponent.volume = this.Volume.Evaluate(TOD_Sky.Instance.Cycle.Hour);
    this.audioComponent.enabled = (double) this.audioComponent.volume > 0.0;
  }
}
