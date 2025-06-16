// Decompiled with JetBrains decompiler
// Type: TextTooltip
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.EventSystems;

#nullable disable
public class TextTooltip : 
  MonoBehaviour,
  IPointerEnterHandler,
  IEventSystemHandler,
  IPointerExitHandler
{
  [SerializeField]
  private string text = string.Empty;
  private TextTooltipView view;

  public string Text
  {
    get => this.text;
    set => this.text = value;
  }

  public void OnPointerEnter(PointerEventData eventData)
  {
    this.view = TextTooltipView.Current;
    this.view?.Show(eventData.position, this.text);
  }

  public void OnPointerExit(PointerEventData eventData)
  {
    this.view?.Hide();
    this.view = (TextTooltipView) null;
  }
}
