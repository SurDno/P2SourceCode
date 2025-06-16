// Decompiled with JetBrains decompiler
// Type: UISounds
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[RequireComponent(typeof (AudioSource))]
public class UISounds : MonoBehaviourInstance<UISounds>
{
  [SerializeField]
  private AudioClip clickSound;
  private AudioSource audioSource;

  protected override void Awake()
  {
    base.Awake();
    this.audioSource = this.GetComponent<AudioSource>();
  }

  public void PlayClickSound() => this.PlaySound(this.clickSound);

  public void PlaySound(AudioClip sound)
  {
    if ((Object) sound == (Object) null)
      return;
    this.audioSource.PlayOneShot(sound);
  }
}
