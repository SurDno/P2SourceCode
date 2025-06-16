// Decompiled with JetBrains decompiler
// Type: RifleObject
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Source.Audio;
using UnityEngine;
using UnityEngine.Audio;

#nullable disable
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
