using System;

public class ParticleBurster : MonoBehaviour
{
  public Burst[] Bursts;
  public AudioSource AudioSource;

  [ContextMenu("Test Fire")]
  public void Fire(AudioMixerGroup mixer = null)
  {
    for (int index = 0; index < Bursts.Length; ++index)
      Bursts[index].System.Emit(Bursts[index].Count);
    if (!((UnityEngine.Object) AudioSource != (UnityEngine.Object) null) || !((UnityEngine.Object) AudioSource.clip != (UnityEngine.Object) null))
      return;
    if ((UnityEngine.Object) mixer != (UnityEngine.Object) null)
      AudioSource.outputAudioMixerGroup = mixer;
    AudioSource.PlayOneShot(AudioSource.clip);
  }

  [Serializable]
  public struct Burst
  {
    public ParticleSystem System;
    public int Count;
  }
}
