using System.Collections;
using Engine.Source.Commons;
using Engine.Source.Settings.External;
using UnityEngine;

[RequireComponent(typeof (AudioSource))]
public class DisableSoundAtMaxDistance : MonoBehaviour
{
  [SerializeField]
  private AudioSource audioSource;

  private IEnumerator Start()
  {
    if (audioSource == null)
    {
      Debug.LogError("Null audio source, gameobject (need to fix by level designer), trying to get component: " + this.GetInfo(), gameObject);
      audioSource = GetComponent<AudioSource>();
      if (audioSource == null)
      {
        Debug.LogError("Can't get component: " + this.GetInfo(), gameObject);
        yield break;
      }
    }
    if (!ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.DisableAudio)
    {
      WaitForSeconds wait = new WaitForSeconds(ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.SoundUpdateDelay);
      while (true)
      {
        if (ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.DisableAudio)
        {
          yield return wait;
        }
        else
        {
          yield return wait;
          UpdateEnable();
        }
      }
    }
  }

  private void UpdateEnable()
  {
    bool flag = (transform.position - EngineApplication.PlayerPosition).sqrMagnitude < (double) (audioSource.maxDistance * audioSource.maxDistance);
    if (audioSource.enabled == flag)
      return;
    audioSource.enabled = flag;
  }
}
