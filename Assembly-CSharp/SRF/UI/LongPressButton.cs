// Decompiled with JetBrains decompiler
// Type: SRF.UI.LongPressButton
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#nullable disable
namespace SRF.UI
{
  [AddComponentMenu("SRF/UI/Long Press Button")]
  public class LongPressButton : Button
  {
    private bool _handled;
    [SerializeField]
    private Button.ButtonClickedEvent _onLongPress = new Button.ButtonClickedEvent();
    private bool _pressed;
    private float _pressedTime;
    public float LongPressDuration = 0.9f;

    public Button.ButtonClickedEvent onLongPress
    {
      get => this._onLongPress;
      set => this._onLongPress = value;
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
      base.OnPointerExit(eventData);
      this._pressed = false;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
      base.OnPointerDown(eventData);
      if (eventData.button != 0)
        return;
      this._pressed = true;
      this._handled = false;
      this._pressedTime = Time.realtimeSinceStartup;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
      if (!this._handled)
        base.OnPointerUp(eventData);
      this._pressed = false;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
      if (this._handled)
        return;
      base.OnPointerClick(eventData);
    }

    private void Update()
    {
      if (!this._pressed || (double) Time.realtimeSinceStartup - (double) this._pressedTime < (double) this.LongPressDuration)
        return;
      this._pressed = false;
      this._handled = true;
      this.onLongPress.Invoke();
    }
  }
}
