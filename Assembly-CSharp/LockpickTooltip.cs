using Engine.Source.Components;
using UnityEngine;
using UnityEngine.UI;

public class LockpickTooltip : MonoBehaviour {
	[SerializeField] private GameObject tooltip;
	[SerializeField] private Image itemIcon;
	[SerializeField] private Text countText;

	public void SetActive(bool active) {
		gameObject.SetActive(active);
	}

	public void SetItem(StorableComponent storable) {
		itemIcon.sprite = storable?.Placeholder?.ImageInventorySlot.Value;
		itemIcon.gameObject.SetActive(itemIcon.sprite != null);
	}

	public void SetCount(int count) {
		countText.text = count.ToString();
	}
}