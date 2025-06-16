using SRF;
using UnityEngine;
using UnityEngine.UI;

namespace SRDebugger.UI.Controls
{
  public class SRTabButton : SRMonoBehaviour
  {
    public Behaviour ActiveToggle;
    public Button Button;
    public Text TitleText;

    public bool IsActive
    {
      get => this.ActiveToggle.enabled;
      set => this.ActiveToggle.enabled = value;
    }
  }
}
