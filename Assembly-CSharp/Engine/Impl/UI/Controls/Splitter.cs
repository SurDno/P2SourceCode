using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Engine.Impl.UI.Controls
{
  [DisallowMultipleComponent]
  [RequireComponent(typeof (CanvasRenderer))]
  public class Splitter : 
    Graphic,
    IBeginDragHandler,
    IEventSystemHandler,
    IDragHandler,
    IEndDragHandler,
    IPointerEnterHandler,
    IPointerExitHandler
  {
    public static Texture2D CursorNormal = (Texture2D) null;
    public static Texture2D CursorHorizontalResize = (Texture2D) null;
    [SerializeField]
    [FormerlySerializedAs("_Direction")]
    protected Direction direction = Direction.None;
    private bool isDragging;
    [SerializeField]
    [FormerlySerializedAs("Target")]
    private RectTransform target = (RectTransform) null;

    public Direction Direction
    {
      get => this.direction;
      set => this.direction = value;
    }

    public void OnBeginDrag(PointerEventData EventData) => this.isDragging = true;

    public void OnDrag(PointerEventData data)
    {
      switch (this.direction)
      {
        case Direction.LeftToRight:
          this.target.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, this.target.rect.width - data.delta.x);
          break;
        case Direction.RightToLeft:
          this.target.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, this.target.rect.width + data.delta.x);
          break;
        case Direction.TopToBottom:
          this.target.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, this.target.rect.height - data.delta.y);
          break;
        case Direction.BottomToTop:
          this.target.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, this.target.rect.height + data.delta.y);
          break;
      }
    }

    public void OnEndDrag(PointerEventData data) => this.isDragging = false;

    public void OnPointerEnter(PointerEventData data)
    {
      if (this.isDragging)
        ;
    }

    public void OnPointerExit(PointerEventData data)
    {
      if (this.isDragging)
        ;
    }
  }
}
