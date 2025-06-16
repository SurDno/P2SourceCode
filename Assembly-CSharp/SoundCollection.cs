using UnityEngine;

[CreateAssetMenu(menuName = "Data/Sound Collection")]
public class SoundCollection : ScriptableObject
{
  [SerializeField]
  private AudioClip[] clips = new AudioClip[0];
  private int prevRandomClip = -1;

  public AudioClip GetClip()
  {
    int length = this.clips.Length;
    AudioClip clip;
    switch (length)
    {
      case 0:
        return (AudioClip) null;
      case 1:
        clip = this.clips[0];
        break;
      default:
        if (this.prevRandomClip == -1)
        {
          this.prevRandomClip = Random.Range(0, length);
          clip = this.clips[this.prevRandomClip];
          break;
        }
        int index = Random.Range(0, length - 1);
        if (index >= this.prevRandomClip)
          ++index;
        clip = this.clips[index];
        this.prevRandomClip = index;
        break;
    }
    return clip;
  }
}
