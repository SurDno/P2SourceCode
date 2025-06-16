// Decompiled with JetBrains decompiler
// Type: SoundPlayer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.DateTime;
using Engine.Common.Services;
using Engine.Source.Audio;
using Engine.Source.Commons;
using Engine.Source.Settings.External;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
[RequireComponent(typeof (AudioSource))]
public class SoundPlayer : MonoBehaviour
{
  [SerializeField]
  private bool playInstantly;
  [EnumFlag(typeof (TimesOfDay))]
  [SerializeField]
  private TimesOfDay timesOfDay;
  [SerializeField]
  public List<AudioClip> clips;
  [SerializeField]
  private MinMaxPair delay;
  [SerializeField]
  private AudioSource audioSource;

  private IEnumerator Start()
  {
    if ((Object) this.audioSource == (Object) null)
    {
      Debug.LogError((object) ("Null audio source, gameobject (need to fix by level designer), trying to get component: " + this.GetInfo()), (Object) this.gameObject);
      this.audioSource = this.GetComponent<AudioSource>();
      if ((Object) this.audioSource == (Object) null)
      {
        Debug.LogError((object) ("Can't get component: " + this.GetInfo()), (Object) this.gameObject);
        yield break;
      }
    }
    if ((Object) this.audioSource.clip != (Object) null)
      Debug.LogError((object) ("clip != null, gameobject : " + this.gameObject.name), (Object) this.gameObject);
    else if (!ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.DisableAudio)
    {
      this.clips.Cleanup<AudioClip>();
      if (this.clips.Count != 0)
      {
        WaitForSeconds wait = new WaitForSeconds(ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.SoundUpdateDelay);
        while (true)
        {
          do
          {
            do
            {
              bool inTimeOfDay;
              do
              {
                do
                {
                  if (!ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.DisableAudio)
                  {
                    yield return (object) wait;
                    this.UpdateEnable();
                  }
                  else
                    goto label_9;
                }
                while (!this.audioSource.enabled);
                TimesOfDay timeOfDay = ServiceLocator.GetService<ITimeService>().SolarTime.GetTimesOfDay();
                inTimeOfDay = TimesOfDayUtility.HasValue(this.timesOfDay, timeOfDay);
              }
              while (!inTimeOfDay);
              if (!((Object) this.audioSource.clip != (Object) null))
                goto label_19;
            }
            while (this.audioSource.isPlaying);
            if (this.audioSource.clip.loadState != AudioDataLoadState.Unloaded)
            {
              if (this.audioSource.clip.loadState == AudioDataLoadState.Failed)
                goto label_17;
            }
            else
              goto label_15;
          }
          while (this.audioSource.clip.loadState == AudioDataLoadState.Loading);
          goto label_20;
label_9:
          yield return (object) wait;
          continue;
label_15:
          this.audioSource.clip.LoadAudioData();
          continue;
label_17:
          this.audioSource.clip = (AudioClip) null;
          continue;
label_19:
          AudioClip clip = this.clips.Random<AudioClip>();
          this.audioSource.clip = clip;
          continue;
label_20:
          this.audioSource.PlayAndCheck();
          Debug.Log((object) ObjectInfoUtility.GetStream().Append("[Sounds]").Append(" Play sound, name : ").Append(this.audioSource.clip.name).Append(" , context : ").GetFullName(this.gameObject));
          float delay2 = (this.playInstantly ? 0.0f : this.audioSource.clip.length) + Random.Range(this.delay.Min, this.delay.Max);
          yield return (object) new WaitForSeconds(delay2);
        }
      }
    }
  }

  private void UpdateEnable()
  {
    bool flag = (double) (this.transform.position - EngineApplication.PlayerPosition).sqrMagnitude < (double) (this.audioSource.maxDistance * this.audioSource.maxDistance);
    if (this.audioSource.enabled == flag)
      return;
    this.audioSource.enabled = flag;
  }
}
