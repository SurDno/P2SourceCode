using UnityEngine;
using UnityEngine.EventSystems;

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
