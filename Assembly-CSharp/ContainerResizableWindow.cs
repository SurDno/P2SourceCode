using Engine.Impl.UI.Menu.Protagonist.Inventory.Container;
using System.Collections.Generic;
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
    if (this.borders.Count == 0)
      return;
    Sprite sprite = active ? this.activeBorder : this.inactiveBorder;
    foreach (Image border in this.borders)
    {
      if (!((Object) border.sprite == (Object) sprite))
        border.sprite = sprite;
    }
  }

  public RectTransform GetContainerRect() => this.containerRect;

  public void Resize(List<InventoryContainerUI> containers)
  {
    this.innerContainer.localPosition = (Vector3) Vector2.zero;
    Vector2 totalSize = this.CalculateTotalSize(containers);
    Vector2 vector2 = new Vector2(Mathf.Max(totalSize.x, this.widthMin), Mathf.Max(totalSize.y, this.heightMin));
    this.containerRect.sizeDelta = vector2;
    this.innerContainer.sizeDelta = vector2;
    this.innerContainer.localPosition = (Vector3) Vector2.zero;
    this.innerContainer.localPosition = (Vector3) new Vector2((this.containerRect.rect.center - (this.CalculateTotalCenter(containers) - (Vector2) this.transform.InverseTransformPoint((Vector3) (Vector2) this.containerRect.position))).x, 0.0f);
  }

  private Vector2 CalculateTotalSize(List<InventoryContainerUI> containers)
  {
    Rect bounds = this.CalculateBounds(containers);
    return new Vector2(Mathf.Abs(bounds.width), Mathf.Abs(bounds.height));
  }

  private Vector2 CalculateTotalCenter(List<InventoryContainerUI> containers)
  {
    return new Vector2(this.CalculateBounds(containers).center.x, 0.0f);
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
      Vector2 vector2_1 = (Vector2) this.transform.InverseTransformPoint((Vector3) (Vector2) container.Content.Transform.position);
      Vector2 vector2_2 = position + vector2_1;
      float x = vector2_2.x;
      float? nullable5;
      int num1;
      if (nullable1.HasValue)
      {
        nullable5 = nullable1;
        float num2 = x;
        num1 = (double) nullable5.GetValueOrDefault() > (double) num2 & nullable5.HasValue ? 1 : 0;
      }
      else
        num1 = 1;
      if (num1 != 0)
        nullable1 = new float?(x);
      float num3 = vector2_2.x + size.x;
      int num4;
      if (nullable2.HasValue)
      {
        nullable5 = nullable2;
        float num5 = num3;
        num4 = (double) nullable5.GetValueOrDefault() < (double) num5 & nullable5.HasValue ? 1 : 0;
      }
      else
        num4 = 1;
      if (num4 != 0)
        nullable2 = new float?(num3);
      float y = vector2_2.y;
      int num6;
      if (nullable3.HasValue)
      {
        nullable5 = nullable3;
        float num7 = y;
        num6 = (double) nullable5.GetValueOrDefault() > (double) num7 & nullable5.HasValue ? 1 : 0;
      }
      else
        num6 = 1;
      if (num6 != 0)
        nullable3 = new float?(y);
      float num8 = vector2_2.y + size.y;
      int num9;
      if (nullable4.HasValue)
      {
        nullable5 = nullable4;
        float num10 = num8;
        num9 = (double) nullable5.GetValueOrDefault() < (double) num10 & nullable5.HasValue ? 1 : 0;
      }
      else
        num9 = 1;
      if (num9 != 0)
        nullable4 = new float?(num8);
    }
    if (!nullable1.HasValue)
      nullable1 = new float?(0.0f);
    if (!nullable2.HasValue)
      nullable2 = new float?(0.0f);
    if (!nullable3.HasValue)
      nullable3 = new float?(0.0f);
    if (!nullable4.HasValue)
      nullable4 = new float?(0.0f);
    return new Rect(new Vector2(nullable1.Value, nullable3.Value), new Vector2(nullable2.Value - nullable1.Value, nullable4.Value - nullable3.Value));
  }
}
