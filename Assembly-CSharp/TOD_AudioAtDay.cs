// Decompiled with JetBrains decompiler
// Type: TOD_AudioAtDay
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[RequireComponent(typeof (AudioSource))]
public class TOD_AudioAtDay : MonoBehaviour
{
  public float fadeTime = 1f;
  private float lerpTime = 0.0f;
  private AudioSource audioComponent;
  private float audioVolume;

  protected void Start()
  {
    this.audioComponent = this.GetComponent<AudioSource>();
    this.audioVolume = this.audioComponent.volume;
    this.audioComponent.enabled = TOD_Sky.Instance.IsDay;
  }

  protected void Update()
  {
    this.lerpTime = Mathf.Clamp01(this.lerpTime + (TOD_Sky.Instance.IsDay ? 1f : -1f) * Time.deltaTime / this.fadeTime);
    this.audioComponent.volume = Mathf.Lerp(0.0f, this.audioVolume, this.lerpTime);
    this.audioComponent.enabled = (double) this.audioComponent.volume > 0.0;
  }
}
