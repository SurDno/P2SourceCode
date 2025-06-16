using System;
using UnityEngine;
using UnityEngine.Audio;

public class ParticleBurster : MonoBehaviour
{
  public ParticleBurster.Burst[] Bursts;
  public AudioSource AudioSource;

  [ContextMenu("Test Fire")]
  public void Fire(AudioMixerGroup mixer = null)
  {
    for (int index = 0; index < this.Bursts.Length; ++index)
      this.Bursts[index].System.Emit(this.Bursts[index].Count);
    if (!((UnityEngine.Object) this.AudioSource != (UnityEngine.Object) null) || !((UnityEngine.Object) this.AudioSource.clip != (UnityEngine.Object) null))
      return;
    if ((UnityEngine.Object) mixer != (UnityEngine.Object) null)
      this.AudioSource.outputAudioMixerGroup = mixer;
    this.AudioSource.PlayOneShot(this.AudioSource.clip);
  }

  [Serializable]
  public struct Burst
  {
    public ParticleSystem System;
    public int Count;
  }
}
