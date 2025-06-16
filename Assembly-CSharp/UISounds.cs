[RequireComponent(typeof (AudioSource))]
public class UISounds : MonoBehaviourInstance<UISounds>
{
  [SerializeField]
  private AudioClip clickSound;
  private AudioSource audioSource;

  protected override void Awake()
  {
    base.Awake();
    audioSource = this.GetComponent<AudioSource>();
  }

  public void PlayClickSound() => PlaySound(clickSound);

  public void PlaySound(AudioClip sound)
  {
    if ((Object) sound == (Object) null)
      return;
    audioSource.PlayOneShot(sound);
  }
}
