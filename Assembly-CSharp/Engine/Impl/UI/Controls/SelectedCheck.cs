// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.SelectedCheck
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.EventSystems;

#nullable disable
namespace Engine.Impl.UI.Controls
{
  public class SelectedCheck : MonoBehaviour, ISelectHandler, IEventSystemHandler, IDeselectHandler
  {
    [SerializeField]
    private HideableView hideableView;

    public void OnDeselect(BaseEventData eventData)
    {
      if (!((Object) this.hideableView != (Object) null))
        return;
      this.hideableView.Visible = false;
    }

    public void OnSelect(BaseEventData eventData)
    {
      if (!((Object) this.hideableView != (Object) null))
        return;
      this.hideableView.Visible = true;
    }
  }
}
