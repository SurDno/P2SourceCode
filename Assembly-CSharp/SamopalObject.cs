public class SamopalObject : MonoBehaviour
{
  public GameObject AimPoint;
  public ParticleBurster ShotEffect;
  public AudioSource FireBeginAudio;
  public AudioSource FireLoopAudio;
  public AudioSource FireShootAudio;
  private AudioMixerGroup mixer;
  private bool needShot;

  public void SetIndoor(bool indoor)
  {
    mixer = indoor ? ScriptableObjectInstance<GameSettingsData>.Instance.NpcWeaponIndoorMixer : ScriptableObjectInstance<GameSettingsData>.Instance.NpcWeaponOutdoorMixer;
    FireBeginAudio.outputAudioMixerGroup = mixer;
    FireLoopAudio.outputAudioMixerGroup = mixer;
    FireShootAudio.outputAudioMixerGroup = mixer;
  }

  private void Start()
  {
    if ((Object) FireBeginAudio != (Object) null)
      FireBeginAudio.PlayAndCheck();
    if (!((Object) FireLoopAudio != (Object) null))
      return;
    FireLoopAudio.PlayAndCheck();
  }

  private void LateUpdate()
  {
    if (!needShot)
      return;
    ShotEffect.Fire(mixer);
    needShot = false;
  }

  public void Shoot()
  {
    if ((Object) FireLoopAudio != (Object) null)
      FireLoopAudio.PlayAndCheck();
    if ((Object) FireShootAudio != (Object) null)
      FireShootAudio.PlayAndCheck();
    needShot = true;
  }

  public void Stop()
  {
    if (!((Object) FireLoopAudio != (Object) null))
      return;
    FireLoopAudio.PlayAndCheck();
  }
}
