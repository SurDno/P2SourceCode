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
    if ((Object) ShootAudio != (Object) null)
      ShootAudio.outputAudioMixerGroup = mixer;
    if (!((Object) ReloadAudio != (Object) null))
      return;
    ReloadAudio.outputAudioMixerGroup = mixer;
  }

  public void Shoot() => needShot = true;

  private void LateUpdate()
  {
    if (!needShot)
      return;
    needShot = false;
    if ((Object) ShootAudio != (Object) null)
      ShootAudio.PlayAndCheck();
    if ((Object) ShotEffect != (Object) null)
      ShotEffect.Fire(mixer);
  }

  public void Reload()
  {
    if (!((Object) ReloadAudio != (Object) null))
      return;
    ReloadAudio.PlayAndCheck();
  }
}
