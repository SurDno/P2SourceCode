// Decompiled with JetBrains decompiler
// Type: FlockChildSound
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Source.Audio;
using UnityEngine;

#nullable disable
[RequireComponent(typeof (AudioSource))]
public class FlockChildSound : MonoBehaviour
{
  public AudioClip[] _idleSounds;
  public float _idleSoundRandomChance = 0.05f;
  public AudioClip[] _flightSounds;
  public float _flightSoundRandomChance = 0.05f;
  public AudioClip[] _scareSounds;
  public float _pitchMin = 0.85f;
  public float _pitchMax = 1f;
  public float _volumeMin = 0.6f;
  public float _volumeMax = 0.8f;
  private FlockChild _flockChild;
  private AudioSource _audio;
  private bool _hasLanded;

  public void Start()
  {
    this._flockChild = this.GetComponent<FlockChild>();
    this._audio = this.GetComponent<AudioSource>();
    this.InvokeRepeating("PlayRandomSound", Random.value + 1f, 1f);
    if (this._scareSounds.Length == 0)
      return;
    this.InvokeRepeating("ScareSound", 1f, 0.01f);
  }

  public void PlayRandomSound()
  {
    if (!this.gameObject.activeInHierarchy)
      return;
    if (!this._audio.isPlaying && this._flightSounds.Length != 0 && (double) this._flightSoundRandomChance > (double) Random.value && !this._flockChild.landing)
    {
      this._audio.clip = this._flightSounds[Random.Range(0, this._flightSounds.Length)];
      this._audio.pitch = Random.Range(this._pitchMin, this._pitchMax);
      this._audio.volume = Random.Range(this._volumeMin, this._volumeMax);
      this._audio.PlayAndCheck();
    }
    else if (!this._audio.isPlaying && this._idleSounds.Length != 0 && (double) this._idleSoundRandomChance > (double) Random.value && this._flockChild.landing)
    {
      this._audio.clip = this._idleSounds[Random.Range(0, this._idleSounds.Length)];
      this._audio.pitch = Random.Range(this._pitchMin, this._pitchMax);
      this._audio.volume = Random.Range(this._volumeMin, this._volumeMax);
      this._audio.PlayAndCheck();
      this._hasLanded = true;
    }
  }

  public void ScareSound()
  {
    if (!this.gameObject.activeInHierarchy || !this._hasLanded || this._flockChild.landing || (double) this._idleSoundRandomChance * 2.0 <= (double) Random.value)
      return;
    this._audio.clip = this._scareSounds[Random.Range(0, this._scareSounds.Length)];
    this._audio.volume = Random.Range(this._volumeMin, this._volumeMax);
    this._audio.PlayDelayed(Random.value * 0.2f);
    this._hasLanded = false;
  }
}
