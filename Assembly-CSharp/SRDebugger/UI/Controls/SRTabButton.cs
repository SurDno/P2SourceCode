// Decompiled with JetBrains decompiler
// Type: SRDebugger.UI.Controls.SRTabButton
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using SRF;
using UnityEngine;
using UnityEngine.UI;

#nullable disable
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
