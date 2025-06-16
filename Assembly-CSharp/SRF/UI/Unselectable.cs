// Decompiled with JetBrains decompiler
// Type: SRF.UI.Unselectable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.EventSystems;

#nullable disable
namespace SRF.UI
{
  [AddComponentMenu("SRF/UI/Unselectable")]
  public sealed class Unselectable : SRMonoBehaviour, ISelectHandler, IEventSystemHandler
  {
    private bool _suspectedSelected;

    public void OnSelect(BaseEventData eventData) => this._suspectedSelected = true;

    private void Update()
    {
      if (!this._suspectedSelected)
        return;
      if ((Object) EventSystem.current.currentSelectedGameObject == (Object) this.CachedGameObject)
        EventSystem.current.SetSelectedGameObject((GameObject) null);
      this._suspectedSelected = false;
    }
  }
}
