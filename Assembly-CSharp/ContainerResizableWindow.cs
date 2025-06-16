using System.Collections.Generic;
using Engine.Impl.UI.Menu.Protagonist.Inventory.Container;
using UnityEngine;
using UnityEngine.UI;

public class ContainerResizableWindow : MonoBehaviour
{
  [SerializeField]
  private float widthMin;
  [SerializeField]
  private float heightMin;
  [SerializeField]
  private RectTransform containerRect;
  [SerializeField]
  private RectTransform backgroundRect;
  [SerializeField]
  private RectTransform innerContainer;
  [SerializeField]
  private List<Image> borders;
  [SerializeField]
  private Sprite activeBorder;
  [SerializeField]
  private Sprite inactiveBorder;

  public void SetActive(bool active)
  {
    if (borders.Count == 0)
      return;
    Sprite sprite = active ? activeBorder : inactiveBorder;
    foreach (Image border in borders)
    {
      if (!(border.sprite == sprite))
        border.sprite = sprite;
    }
  }

  public RectTransform GetContainerRect() => containerRect;

  public void Resize(List<InventoryContainerUI> containers)
  {
    innerContainer.localPosition = Vector2.zero;
    Vector2 totalSize = CalculateTotalSize(containers);
    Vector2 vector2 = new Vector2(Mathf.Max(totalSize.x, widthMin), Mathf.Max(totalSize.y, heightMin));
    containerRect.sizeDelta = vector2;
    innerContainer.sizeDelta = vector2;
    innerContainer.localPosition = Vector2.zero;
    innerContainer.localPosition = new Vector2((containerRect.rect.center - (CalculateTotalCenter(containers) - (Vector2) transform.InverseTransformPoint((Vector2) containerRect.position))).x, 0.0f);
  }

  private Vector2 CalculateTotalSize(List<InventoryContainerUI> containers)
  {
    Rect bounds = CalculateBounds(containers);
    return new Vector2(Mathf.Abs(bounds.width), Mathf.Abs(bounds.height));
  }

  private Vector2 CalculateTotalCenter(List<InventoryContainerUI> containers)
  {
    return new Vector2(CalculateBounds(containers).center.x, 0.0f);
  }

  private Rect CalculateBounds(List<InventoryContainerUI> containers)
  {
    float? nullable1 = new float?();
    float? nullable2 = new float?();
    float? nullable3 = new float?();
    float? nullable4 = new float?();
    foreach (InventoryContainerUI container in containers)
    {
      Rect rect = container.Transform.rect;
      Vector2 position = rect.position;
      rect = container.Transform.rect;
      Vector2 size = rect.size;
      Vector2 vector2_1 = transform.InverseTransformPoint((Vector2) container.Content.Transform.position);
      Vector2 vector2_2 = position + vector2_1;
      float x = vector2_2.x;
      float? nullable5;
      int num1;
      if (nullable1.HasValue)
      {
        nullable5 = nullable1;
        float num2 = x;
        num1 = nullable5.GetValueOrDefault() > (double) num2 & nullable5.HasValue ? 1 : 0;
      }
      else
        num1 = 1;
      if (num1 != 0)
        nullable1 = x;
      float num3 = vector2_2.x + size.x;
      int num4;
      if (nullable2.HasValue)
      {
        nullable5 = nullable2;
        float num5 = num3;
        num4 = nullable5.GetValueOrDefault() < (double) num5 & nullable5.HasValue ? 1 : 0;
      }
      else
        num4 = 1;
      if (num4 != 0)
        nullable2 = num3;
      float y = vector2_2.y;
      int num6;
      if (nullable3.HasValue)
      {
        nullable5 = nullable3;
        float num7 = y;
        num6 = nullable5.GetValueOrDefault() > (double) num7 & nullable5.HasValue ? 1 : 0;
      }
      else
        num6 = 1;
      if (num6 != 0)
        nullable3 = y;
      float num8 = vector2_2.y + size.y;
      int num9;
      if (nullable4.HasValue)
      {
        nullable5 = nullable4;
        float num10 = num8;
        num9 = nullable5.GetValueOrDefault() < (double) num10 & nullable5.HasValue ? 1 : 0;
      }
      else
        num9 = 1;
      if (num9 != 0)
        nullable4 = num8;
    }
    if (!nullable1.HasValue)
      nullable1 = 0.0f;
    if (!nullable2.HasValue)
      nullable2 = 0.0f;
    if (!nullable3.HasValue)
      nullable3 = 0.0f;
    if (!nullable4.HasValue)
      nullable4 = 0.0f;
    return new Rect(new Vector2(nullable1.Value, nullable3.Value), new Vector2(nullable2.Value - nullable1.Value, nullable4.Value - nullable3.Value));
  }
}
