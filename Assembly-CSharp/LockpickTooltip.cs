using Engine.Source.Components;

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
    itemIcon.sprite = storable?.Placeholder?.ImageInventorySlot.Value;
    itemIcon.gameObject.SetActive((Object) itemIcon.sprite != (Object) null);
  }

  public void SetCount(int count) => countText.text = count.ToString();
}
