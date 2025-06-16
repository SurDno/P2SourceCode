// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.PointerInsideCheck
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.EventSystems;

#nullable disable
namespace Engine.Impl.UI.Controls
{
  public class PointerInsideCheck : 
    MonoBehaviour,
    IPointerEnterHandler,
    IEventSystemHandler,
    IPointerExitHandler
  {
    [SerializeField]
    private HideableView hideableView;
    [SerializeField]
    private EventView pointerEnterEventView;
    [SerializeField]
    private EventView pointerExitEventView;

    public void OnPointerEnter(PointerEventData eventData)
    {
      this.pointerEnterEventView?.Invoke();
      if (!((Object) this.hideableView != (Object) null))
        return;
      this.hideableView.Visible = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
      if ((Object) this.hideableView != (Object) null)
        this.hideableView.Visible = false;
      this.pointerExitEventView?.Invoke();
    }
  }
}
