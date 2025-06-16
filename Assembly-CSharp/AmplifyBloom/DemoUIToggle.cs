using UnityEngine.UI;

namespace AmplifyBloom
{
  public sealed class DemoUIToggle : DemoUIElement
  {
    private Toggle m_toggle;

    private void Start() => this.m_toggle = this.GetComponent<Toggle>();

    public override void DoAction(DemoUIElementAction action, params object[] vars)
    {
      if (!this.m_toggle.IsInteractable() || action != DemoUIElementAction.Press)
        return;
      this.m_toggle.isOn = !this.m_toggle.isOn;
    }
  }
}
