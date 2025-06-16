using UnityEngine;

public class PlayRandomClipOnce : MonoBehaviour
{
  [SerializeField]
  private AudioClip[] clips;

  private void Start()
  {
    AudioClip clip = clips[Random.Range(0, clips.Length)];
    if (clip == null)
      return;
    GetComponent<AudioSource>().PlayOneShot(clip);
  }
}
