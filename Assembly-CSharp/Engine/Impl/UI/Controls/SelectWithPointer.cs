// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.SelectWithPointer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.EventSystems;

#nullable disable
namespace Engine.Impl.UI.Controls
{
  public class SelectWithPointer : 
    MonoBehaviour,
    IPointerEnterHandler,
    IEventSystemHandler,
    IPointerExitHandler
  {
    public void OnPointerEnter(PointerEventData eventData)
    {
      EventSystem.current.SetSelectedGameObject(this.gameObject, (BaseEventData) eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
      EventSystem current = EventSystem.current;
      if (!((Object) current.currentSelectedGameObject == (Object) this.gameObject))
        return;
      current.SetSelectedGameObject((GameObject) null, (BaseEventData) eventData);
    }
  }
}
