using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Item Sound Group")]
public class ItemSoundGroup : ScriptableObject
{
  [SerializeField]
  private ItemSoundGroup.ClipSource put;
  [SerializeField]
  private ItemSoundGroup.ClipSource take;
  [SerializeField]
  private ItemSoundGroup.ClipSource use;
  [SerializeField]
  private ItemSoundGroup.ClipSource pourOut;

  public AudioClip GetPutClip() => this.put.GetClip();

  public AudioClip GetTakeClip() => this.take.GetClip();

  public AudioClip GetUseClip() => this.use.GetClip();

  public AudioClip GetPourOutClip() => this.pourOut.GetClip();

  [Serializable]
  private struct ClipSource
  {
    [SerializeField]
    private AudioClip clip;
    [SerializeField]
    private SoundCollection collection;

    public AudioClip GetClip()
    {
      if ((UnityEngine.Object) this.clip != (UnityEngine.Object) null)
        return this.clip;
      return (UnityEngine.Object) this.collection != (UnityEngine.Object) null ? this.collection.GetClip() : (AudioClip) null;
    }
  }
}
