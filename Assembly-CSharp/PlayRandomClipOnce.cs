using UnityEngine;

public class PlayRandomClipOnce : MonoBehaviour
{
  [SerializeField]
  private AudioClip[] clips;

  private void Start()
  {
    AudioClip clip = this.clips[Random.Range(0, this.clips.Length)];
    if ((Object) clip == (Object) null)
      return;
    this.GetComponent<AudioSource>().PlayOneShot(clip);
  }
}
