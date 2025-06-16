// Decompiled with JetBrains decompiler
// Type: PlayRandomClipOnce
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class PlayRandomClipOnce : MonoBehaviour
{
  [SerializeField]
  private AudioClip[] clips;

  private void Start()
  {
    AudioClip clip = this.clips[Random.Range(0, this.clips.Length)];
    if ((Object) clip == (Object) null)
      return;
    this.GetComponent<AudioSource>().PlayOneShot(clip);
  }
}
