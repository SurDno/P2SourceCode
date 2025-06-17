using UnityEngine;
using UnityEngine.UI;

namespace AmplifyBloom
{
  public class DemoUIElement : MonoBehaviour
  {
    private bool m_isSelected;
    private Text m_text;
    private Color m_selectedColor = new(1f, 1f, 1f);
    private Color m_unselectedColor;

    public void Init()
    {
      m_text = transform.GetComponentInChildren<Text>();
      m_unselectedColor = m_text.color;
    }

    public virtual void DoAction(DemoUIElementAction action, params object[] vars)
    {
    }

    public virtual void Idle()
    {
    }

    public bool Select
    {
      get => m_isSelected;
      set
      {
        m_isSelected = value;
        m_text.color = value ? m_selectedColor : m_unselectedColor;
      }
    }
  }
}
