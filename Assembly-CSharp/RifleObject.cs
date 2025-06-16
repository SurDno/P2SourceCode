using Engine.Source.Audio;
using UnityEngine;
using UnityEngine.Audio;

public class RifleObject : MonoBehaviour
{
  public ParticleBurster ShotEffect;
  public AudioSource ShootAudio;
  public AudioSource ReloadAudio;
  private AudioMixerGroup mixer;
  private bool needShot = false;

  public void SetIndoor(bool indoor)
  {
    this.mixer = indoor ? ScriptableObjectInstance<GameSettingsData>.Instance.NpcWeaponIndoorMixer : ScriptableObjectInstance<GameSettingsData>.Instance.NpcWeaponOutdoorMixer;
    if ((Object) this.ShootAudio != (Object) null)
      this.ShootAudio.outputAudioMixerGroup = this.mixer;
    if (!((Object) this.ReloadAudio != (Object) null))
      return;
    this.ReloadAudio.outputAudioMixerGroup = this.mixer;
  }

  public void Shoot() => this.needShot = true;

  private void LateUpdate()
  {
    if (!this.needShot)
      return;
    this.needShot = false;
    if ((Object) this.ShootAudio != (Object) null)
      this.ShootAudio.PlayAndCheck();
    if ((Object) this.ShotEffect != (Object) null)
      this.ShotEffect.Fire(this.mixer);
  }

  public void Reload()
  {
    if (!((Object) this.ReloadAudio != (Object) null))
      return;
    this.ReloadAudio.PlayAndCheck();
  }
}
