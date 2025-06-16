using UnityEngine;

[RequireComponent(typeof (AudioSource))]
public class TOD_AudioAtDay : MonoBehaviour
{
  public float fadeTime = 1f;
  private float lerpTime = 0.0f;
  private AudioSource audioComponent;
  private float audioVolume;

  protected void Start()
  {
    this.audioComponent = this.GetComponent<AudioSource>();
    this.audioVolume = this.audioComponent.volume;
    this.audioComponent.enabled = TOD_Sky.Instance.IsDay;
  }

  protected void Update()
  {
    this.lerpTime = Mathf.Clamp01(this.lerpTime + (TOD_Sky.Instance.IsDay ? 1f : -1f) * Time.deltaTime / this.fadeTime);
    this.audioComponent.volume = Mathf.Lerp(0.0f, this.audioVolume, this.lerpTime);
    this.audioComponent.enabled = (double) this.audioComponent.volume > 0.0;
  }
}
