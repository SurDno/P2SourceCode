using System;

[CreateAssetMenu(menuName = "Data/Item Sound Group")]
public class ItemSoundGroup : ScriptableObject
{
  [SerializeField]
  private ClipSource put;
  [SerializeField]
  private ClipSource take;
  [SerializeField]
  private ClipSource use;
  [SerializeField]
  private ClipSource pourOut;

  public AudioClip GetPutClip() => put.GetClip();

  public AudioClip GetTakeClip() => take.GetClip();

  public AudioClip GetUseClip() => use.GetClip();

  public AudioClip GetPourOutClip() => pourOut.GetClip();

  [Serializable]
  private struct ClipSource
  {
    [SerializeField]
    private AudioClip clip;
    [SerializeField]
    private SoundCollection collection;

    public AudioClip GetClip()
    {
      if ((UnityEngine.Object) clip != (UnityEngine.Object) null)
        return clip;
      return (UnityEngine.Object) collection != (UnityEngine.Object) null ? collection.GetClip() : (AudioClip) null;
    }
  }
}
