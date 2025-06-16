using UnityEngine;
using UnityEngine.UI;

namespace AmplifyBloom
{
  public class DemoUIElement : MonoBehaviour
  {
    private bool m_isSelected = false;
    private Text m_text;
    private Color m_selectedColor = new Color(1f, 1f, 1f);
    private Color m_unselectedColor;

    public void Init()
    {
      this.m_text = this.transform.GetComponentInChildren<Text>();
      this.m_unselectedColor = this.m_text.color;
    }

    public virtual void DoAction(DemoUIElementAction action, params object[] vars)
    {
    }

    public virtual void Idle()
    {
    }

    public bool Select
    {
      get => this.m_isSelected;
      set
      {
        this.m_isSelected = value;
        this.m_text.color = value ? this.m_selectedColor : this.m_unselectedColor;
      }
    }
  }
}
