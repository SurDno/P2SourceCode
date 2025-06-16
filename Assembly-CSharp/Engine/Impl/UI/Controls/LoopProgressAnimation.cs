using UnityEngine;

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
      get => this.rate;
      set => this.rate = value;
    }

    private void ApplyPhase()
    {
      if ((Object) this.progressView == (Object) null)
        return;
      this.progressView.FloatValue = this.phase;
    }

    public override void SkipAnimation()
    {
    }

    private void OnEnable()
    {
      this.phase = this.randomStart ? Random.value : 0.0f;
      this.ApplyPhase();
    }

    private void Update()
    {
      this.phase += Time.deltaTime * this.rate;
      this.phase %= 1f;
      this.ApplyPhase();
    }
  }
}
