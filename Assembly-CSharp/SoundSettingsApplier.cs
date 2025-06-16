// Decompiled with JetBrains decompiler
// Type: SoundSettingsApplier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Source.Commons;
using Engine.Source.Settings;
using System;
using UnityEngine;
using UnityEngine.Audio;

#nullable disable
public class SoundSettingsApplier : MonoBehaviour
{
  [SerializeField]
  private AudioMixer masterMixer;
  [SerializeField]
  private AudioMixer[] effectsMixers;
  [SerializeField]
  private AudioMixer[] musicMixers;
  [SerializeField]
  private AudioMixer[] voiceMixers;

  private void Start()
  {
    this.Apply();
    InstanceByRequest<SoundGameSettings>.Instance.OnApply += new Action(this.Apply);
  }

  private void OnDestroy()
  {
    InstanceByRequest<SoundGameSettings>.Instance.OnApply -= new Action(this.Apply);
  }

  private void Apply()
  {
    this.Apply(this.masterMixer, InstanceByRequest<SoundGameSettings>.Instance.MasterVolume.Value);
    this.Apply(this.effectsMixers, InstanceByRequest<SoundGameSettings>.Instance.EffectsVolume.Value);
    this.Apply(this.musicMixers, InstanceByRequest<SoundGameSettings>.Instance.MusicVolume.Value);
    this.Apply(this.voiceMixers, InstanceByRequest<SoundGameSettings>.Instance.VoiceVolume.Value);
  }

  private void Apply(AudioMixer mixer, float value)
  {
    mixer.SetFloat("Master+Base{0}::Volume", this.ValueToVolume(value));
  }

  private void Apply(AudioMixer[] mixers, float value)
  {
    float volume = this.ValueToVolume(value);
    foreach (AudioMixer mixer in mixers)
      mixer.SetFloat("Master+Base{0}::Volume", volume);
  }

  private float ValueToVolume(float value)
  {
    value = Mathf.Clamp(value, 0.0001f, 1f);
    return Mathf.Log10(value) * 20f;
  }
}
