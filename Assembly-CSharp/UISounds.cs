using UnityEngine;

[RequireComponent(typeof (AudioSource))]
public class UISounds : MonoBehaviourInstance<UISounds>
{
  [SerializeField]
  private AudioClip clickSound;
  private AudioSource audioSource;

  protected override void Awake()
  {
    base.Awake();
    this.audioSource = this.GetComponent<AudioSource>();
  }

  public void PlayClickSound() => this.PlaySound(this.clickSound);

  public void PlaySound(AudioClip sound)
  {
    if ((Object) sound == (Object) null)
      return;
    this.audioSource.PlayOneShot(sound);
  }
}
