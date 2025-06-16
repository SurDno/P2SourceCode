namespace Engine.Impl.UI.Controls
{
  public class LoopProgressAnimation : FloatView
  {
    [SerializeField]
    private FloatView progressView;
    [SerializeField]
    private float rate;
    [SerializeField]
    private bool randomStart;
    private float phase;

    public override float FloatValue
    {
      get => rate;
      set => rate = value;
    }

    private void ApplyPhase()
    {
      if ((Object) progressView == (Object) null)
        return;
      progressView.FloatValue = phase;
    }

    public override void SkipAnimation()
    {
    }

    private void OnEnable()
    {
      phase = randomStart ? Random.value : 0.0f;
      ApplyPhase();
    }

    private void Update()
    {
      phase += Time.deltaTime * rate;
      phase %= 1f;
      ApplyPhase();
    }
  }
}
