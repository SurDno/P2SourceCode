using UnityEngine;

[RequireComponent(typeof (AudioSource))]
public class TOD_AudioAtTime : MonoBehaviour
{
  public AnimationCurve Volume = new() {
    keys = [
      new(0.0f, 0.0f),
      new(12f, 1f),
      new(24f, 0.0f)
    ]
  };
  private AudioSource audioComponent;

  protected void Start() => audioComponent = GetComponent<AudioSource>();

  protected void Update()
  {
    audioComponent.volume = Volume.Evaluate(TOD_Sky.Instance.Cycle.Hour);
    audioComponent.enabled = audioComponent.volume > 0.0;
  }
}
