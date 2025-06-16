using System.Collections;
using System.Collections.Generic;
using Engine.Common.DateTime;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Settings.External;

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
    if ((Object) audioSource == (Object) null)
    {
      Debug.LogError((object) ("Null audio source, gameobject (need to fix by level designer), trying to get component: " + this.GetInfo()), (Object) this.gameObject);
      audioSource = this.GetComponent<AudioSource>();
      if ((Object) audioSource == (Object) null)
      {
        Debug.LogError((object) ("Can't get component: " + this.GetInfo()), (Object) this.gameObject);
        yield break;
      }
    }
    if ((Object) audioSource.clip != (Object) null)
      Debug.LogError((object) ("clip != null, gameobject : " + this.gameObject.name), (Object) this.gameObject);
    else if (!ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.DisableAudio)
    {
      clips.Cleanup<AudioClip>();
      if (clips.Count != 0)
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
                    UpdateEnable();
                  }
                  else
                    goto label_9;
                }
                while (!audioSource.enabled);
                TimesOfDay timeOfDay = ServiceLocator.GetService<ITimeService>().SolarTime.GetTimesOfDay();
                inTimeOfDay = TimesOfDayUtility.HasValue(timesOfDay, timeOfDay);
              }
              while (!inTimeOfDay);
              if (!((Object) audioSource.clip != (Object) null))
                goto label_19;
            }
            while (audioSource.isPlaying);
            if (audioSource.clip.loadState != AudioDataLoadState.Unloaded)
            {
              if (audioSource.clip.loadState == AudioDataLoadState.Failed)
                goto label_17;
            }
            else
              goto label_15;
          }
          while (audioSource.clip.loadState == AudioDataLoadState.Loading);
          goto label_20;
label_9:
          yield return (object) wait;
          continue;
label_15:
          audioSource.clip.LoadAudioData();
          continue;
label_17:
          audioSource.clip = (AudioClip) null;
          continue;
label_19:
          AudioClip clip = clips.Random();
          audioSource.clip = clip;
          continue;
label_20:
          audioSource.PlayAndCheck();
          Debug.Log((object) ObjectInfoUtility.GetStream().Append("[Sounds]").Append(" Play sound, name : ").Append(audioSource.clip.name).Append(" , context : ").GetFullName(this.gameObject));
          float delay2 = (playInstantly ? 0.0f : audioSource.clip.length) + Random.Range(delay.Min, delay.Max);
          yield return (object) new WaitForSeconds(delay2);
        }
      }
    }
  }

  private void UpdateEnable()
  {
    bool flag = (double) (this.transform.position - EngineApplication.PlayerPosition).sqrMagnitude < (double) (audioSource.maxDistance * audioSource.maxDistance);
    if (audioSource.enabled == flag)
      return;
    audioSource.enabled = flag;
  }
}
