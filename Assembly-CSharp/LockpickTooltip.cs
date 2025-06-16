using Engine.Source.Components;
using UnityEngine;
using UnityEngine.UI;

public class LockpickTooltip : MonoBehaviour
{
  [SerializeField]
  private GameObject tooltip;
  [SerializeField]
  private Image itemIcon;
  [SerializeField]
  private Text countText;

  public void SetActive(bool active) => this.gameObject.SetActive(active);

  public void SetItem(StorableComponent storable)
  {
    this.itemIcon.sprite = storable?.Placeholder?.ImageInventorySlot.Value;
    this.itemIcon.gameObject.SetActive((Object) this.itemIcon.sprite != (Object) null);
  }

  public void SetCount(int count) => this.countText.text = count.ToString();
}
