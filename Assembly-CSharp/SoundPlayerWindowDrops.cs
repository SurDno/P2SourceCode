using System.Collections;
using System.Linq;
using Engine.Source.Audio;
using Engine.Source.Commons;
using Engine.Source.Settings.External;
using Rain;
using UnityEngine;

public class SoundPlayerWindowDrops : MonoBehaviour
{
  [SerializeField]
  private bool playInstantly;
  [SerializeField]
  private AudioClip[] clips = [];
  public AudioSource audioSource;
  public float rainMinimum;
  private float wait;

  private IEnumerator Start()
  {
    wait = ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.SoundUpdateDelay;
    clips = clips.Where(o => o != null).ToArray();
    if (clips.Length != 0)
    {
      while (true)
      {
        do
        {
          yield return new WaitForSeconds(wait);
          UpdateEnable();
        }
        while (!audioSource.enabled || audioSource.clip != null && audioSource.isPlaying);
        AudioClip clip = clips.Random();
        audioSource.clip = clip;
        audioSource.PlayAndCheck();
        float delay2 = playInstantly ? 0.0f : audioSource.clip.length;
        yield return new WaitForSeconds(delay2);
        clip = null;
      }
    }
  }

  private void UpdateEnable()
  {
    bool flag = (transform.position - EngineApplication.PlayerPosition).sqrMagnitude < (double) (audioSource.maxDistance * audioSource.maxDistance);
    if (flag)
      flag = RainManager.Instance.rainIntensity >= (double) rainMinimum;
    if (audioSource.gameObject.activeSelf == flag)
      return;
    audioSource.gameObject.SetActive(flag);
  }
}
