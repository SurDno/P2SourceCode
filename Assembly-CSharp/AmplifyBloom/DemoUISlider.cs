using UnityEngine.UI;

namespace AmplifyBloom
{
  public sealed class DemoUISlider : DemoUIElement
  {
    public bool SingleStep;
    private Slider m_slider;
    private bool m_lastStep;

    private void Start() => m_slider = GetComponent<Slider>();

    public override void DoAction(DemoUIElementAction action, params object[] vars)
    {
      if (!m_slider.IsInteractable() || action != DemoUIElementAction.Slide)
        return;
      float var = (float) vars[0];
      if (SingleStep)
      {
        if (m_lastStep)
          return;
        m_lastStep = true;
      }
      if (m_slider.wholeNumbers)
      {
        if (var > 0.0)
          ++m_slider.value;
        else if (var < 0.0)
          --m_slider.value;
      }
      else
        m_slider.value += var;
    }

    public override void Idle() => m_lastStep = false;
  }
}
