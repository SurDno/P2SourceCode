using UnityEngine;

[CreateAssetMenu(menuName = "Data/Sound Collection")]
public class SoundCollection : ScriptableObject
{
  [SerializeField]
  private AudioClip[] clips = [];
  private int prevRandomClip = -1;

  public AudioClip GetClip()
  {
    int length = clips.Length;
    AudioClip clip;
    switch (length)
    {
      case 0:
        return null;
      case 1:
        clip = clips[0];
        break;
      default:
        if (prevRandomClip == -1)
        {
          prevRandomClip = Random.Range(0, length);
          clip = clips[prevRandomClip];
          break;
        }
        int index = Random.Range(0, length - 1);
        if (index >= prevRandomClip)
          ++index;
        clip = clips[index];
        prevRandomClip = index;
        break;
    }
    return clip;
  }
}
