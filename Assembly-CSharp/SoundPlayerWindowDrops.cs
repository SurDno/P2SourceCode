using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Engine.Source.Commons;
using Engine.Source.Settings.External;
using Rain;

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
    wait = ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.SoundUpdateDelay;
    clips = ((IEnumerable<AudioClip>) clips).Where((Func<AudioClip, bool>) (o => (UnityEngine.Object) o != (UnityEngine.Object) null)).ToArray();
    if (clips.Length != 0)
    {
      while (true)
      {
        do
        {
          yield return (object) new WaitForSeconds(wait);
          UpdateEnable();
        }
        while (!audioSource.enabled || (UnityEngine.Object) audioSource.clip != (UnityEngine.Object) null && audioSource.isPlaying);
        AudioClip clip = ((IEnumerable<AudioClip>) clips).Random();
        audioSource.clip = clip;
        audioSource.PlayAndCheck();
        float delay2 = playInstantly ? 0.0f : audioSource.clip.length;
        yield return (object) new WaitForSeconds(delay2);
        clip = (AudioClip) null;
      }
    }
  }

  private void UpdateEnable()
  {
    bool flag = (double) (this.transform.position - EngineApplication.PlayerPosition).sqrMagnitude < (double) (audioSource.maxDistance * audioSource.maxDistance);
    if (flag)
      flag = RainManager.Instance.rainIntensity >= (double) rainMinimum;
    if (audioSource.gameObject.activeSelf == flag)
      return;
    audioSource.gameObject.SetActive(flag);
  }
}
