using Engine.Source.Audio;
using UnityEngine;
using UnityEngine.Audio;

public class RifleObject : MonoBehaviour
{
  public ParticleBurster ShotEffect;
  public AudioSource ShootAudio;
  public AudioSource ReloadAudio;
  private AudioMixerGroup mixer;
  private bool needShot;

  public void SetIndoor(bool indoor)
  {
    mixer = indoor ? ScriptableObjectInstance<GameSettingsData>.Instance.NpcWeaponIndoorMixer : ScriptableObjectInstance<GameSettingsData>.Instance.NpcWeaponOutdoorMixer;
    if (ShootAudio != null)
      ShootAudio.outputAudioMixerGroup = mixer;
    if (!(ReloadAudio != null))
      return;
    ReloadAudio.outputAudioMixerGroup = mixer;
  }

  public void Shoot() => needShot = true;

  private void LateUpdate()
  {
    if (!needShot)
      return;
    needShot = false;
    if (ShootAudio != null)
      ShootAudio.PlayAndCheck();
    if (ShotEffect != null)
      ShotEffect.Fire(mixer);
  }

  public void Reload()
  {
    if (!(ReloadAudio != null))
      return;
    ReloadAudio.PlayAndCheck();
  }
}
