public class PlayRandomClipOnce : MonoBehaviour
{
  [SerializeField]
  private AudioClip[] clips;

  private void Start()
  {
    AudioClip clip = clips[Random.Range(0, clips.Length)];
    if ((Object) clip == (Object) null)
      return;
    this.GetComponent<AudioSource>().PlayOneShot(clip);
  }
}
