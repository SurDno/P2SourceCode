// Decompiled with JetBrains decompiler
// Type: SamopalObject
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Source.Audio;
using UnityEngine;
using UnityEngine.Audio;

#nullable disable
public class SamopalObject : MonoBehaviour
{
  public GameObject AimPoint;
  public ParticleBurster ShotEffect;
  public AudioSource FireBeginAudio;
  public AudioSource FireLoopAudio;
  public AudioSource FireShootAudio;
  private AudioMixerGroup mixer;
  private bool needShot = false;

  public void SetIndoor(bool indoor)
  {
    this.mixer = indoor ? ScriptableObjectInstance<GameSettingsData>.Instance.NpcWeaponIndoorMixer : ScriptableObjectInstance<GameSettingsData>.Instance.NpcWeaponOutdoorMixer;
    this.FireBeginAudio.outputAudioMixerGroup = this.mixer;
    this.FireLoopAudio.outputAudioMixerGroup = this.mixer;
    this.FireShootAudio.outputAudioMixerGroup = this.mixer;
  }

  private void Start()
  {
    if ((Object) this.FireBeginAudio != (Object) null)
      this.FireBeginAudio.PlayAndCheck();
    if (!((Object) this.FireLoopAudio != (Object) null))
      return;
    this.FireLoopAudio.PlayAndCheck();
  }

  private void LateUpdate()
  {
    if (!this.needShot)
      return;
    this.ShotEffect.Fire(this.mixer);
    this.needShot = false;
  }

  public void Shoot()
  {
    if ((Object) this.FireLoopAudio != (Object) null)
      this.FireLoopAudio.PlayAndCheck();
    if ((Object) this.FireShootAudio != (Object) null)
      this.FireShootAudio.PlayAndCheck();
    this.needShot = true;
  }

  public void Stop()
  {
    if (!((Object) this.FireLoopAudio != (Object) null))
      return;
    this.FireLoopAudio.PlayAndCheck();
  }
}
