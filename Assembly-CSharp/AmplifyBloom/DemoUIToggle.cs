// Decompiled with JetBrains decompiler
// Type: AmplifyBloom.DemoUIToggle
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine.UI;

#nullable disable
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
