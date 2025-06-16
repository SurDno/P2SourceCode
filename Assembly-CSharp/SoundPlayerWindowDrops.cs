// Decompiled with JetBrains decompiler
// Type: SoundPlayerWindowDrops
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Source.Audio;
using Engine.Source.Commons;
using Engine.Source.Settings.External;
using Rain;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable disable
public class SoundPlayerWindowDrops : MonoBehaviour
{
  [SerializeField]
  private bool playInstantly;
  [SerializeField]
  private AudioClip[] clips = new AudioClip[0];
  public AudioSource audioSource;
  public float rainMinimum;
  private float wait;

  private IEnumerator Start()
  {
    this.wait = ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.SoundUpdateDelay;
    this.clips = ((IEnumerable<AudioClip>) this.clips).Where<AudioClip>((Func<AudioClip, bool>) (o => (UnityEngine.Object) o != (UnityEngine.Object) null)).ToArray<AudioClip>();
    if (this.clips.Length != 0)
    {
      while (true)
      {
        do
        {
          yield return (object) new WaitForSeconds(this.wait);
          this.UpdateEnable();
        }
        while (!this.audioSource.enabled || (UnityEngine.Object) this.audioSource.clip != (UnityEngine.Object) null && this.audioSource.isPlaying);
        AudioClip clip = ((IEnumerable<AudioClip>) this.clips).Random<AudioClip>();
        this.audioSource.clip = clip;
        this.audioSource.PlayAndCheck();
        float delay2 = this.playInstantly ? 0.0f : this.audioSource.clip.length;
        yield return (object) new WaitForSeconds(delay2);
        clip = (AudioClip) null;
      }
    }
  }

  private void UpdateEnable()
  {
    bool flag = (double) (this.transform.position - EngineApplication.PlayerPosition).sqrMagnitude < (double) (this.audioSource.maxDistance * this.audioSource.maxDistance);
    if (flag)
      flag = (double) RainManager.Instance.rainIntensity >= (double) this.rainMinimum;
    if (this.audioSource.gameObject.activeSelf == flag)
      return;
    this.audioSource.gameObject.SetActive(flag);
  }
}
