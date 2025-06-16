// Decompiled with JetBrains decompiler
// Type: AmplifyBloom.DemoUISlider
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine.UI;

#nullable disable
namespace AmplifyBloom
{
  public sealed class DemoUISlider : DemoUIElement
  {
    public bool SingleStep = false;
    private Slider m_slider;
    private bool m_lastStep = false;

    private void Start() => this.m_slider = this.GetComponent<Slider>();

    public override void DoAction(DemoUIElementAction action, params object[] vars)
    {
      if (!this.m_slider.IsInteractable() || action != DemoUIElementAction.Slide)
        return;
      float var = (float) vars[0];
      if (this.SingleStep)
      {
        if (this.m_lastStep)
          return;
        this.m_lastStep = true;
      }
      if (this.m_slider.wholeNumbers)
      {
        if ((double) var > 0.0)
          ++this.m_slider.value;
        else if ((double) var < 0.0)
          --this.m_slider.value;
      }
      else
        this.m_slider.value += var;
    }

    public override void Idle() => this.m_lastStep = false;
  }
}
