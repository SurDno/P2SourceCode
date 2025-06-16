// Decompiled with JetBrains decompiler
// Type: AmplifyBloom.DemoUIElement
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

#nullable disable
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
