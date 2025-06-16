[RequireComponent(typeof (AudioSource))]
public class TOD_AudioAtTime : MonoBehaviour
{
  public AnimationCurve Volume = new AnimationCurve {
    keys = new Keyframe[3]
    {
      new Keyframe(0.0f, 0.0f),
      new Keyframe(12f, 1f),
      new Keyframe(24f, 0.0f)
    }
  };
  private AudioSource audioComponent;

  protected void Start() => audioComponent = this.GetComponent<AudioSource>();

  protected void Update()
  {
    audioComponent.volume = Volume.Evaluate(TOD_Sky.Instance.Cycle.Hour);
    audioComponent.enabled = (double) audioComponent.volume > 0.0;
  }
}
