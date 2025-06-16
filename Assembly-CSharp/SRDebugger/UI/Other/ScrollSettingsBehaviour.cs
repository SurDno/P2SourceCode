// Decompiled with JetBrains decompiler
// Type: SRDebugger.UI.Other.ScrollSettingsBehaviour
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

#nullable disable
namespace SRDebugger.UI.Other
{
  [RequireComponent(typeof (ScrollRect))]
  public class ScrollSettingsBehaviour : MonoBehaviour
  {
    public const float ScrollSensitivity = 40f;

    private void Awake()
    {
      ScrollRect component = this.GetComponent<ScrollRect>();
      component.scrollSensitivity = 40f;
      component.movementType = ScrollRect.MovementType.Clamped;
      component.inertia = false;
    }
  }
}
