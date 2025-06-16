using Engine.Impl.UI.Menu.Protagonist.Inventory;
using Engine.Source.Components;
using Engine.Source.UI.Menu.Protagonist.Inventory;
using InputServices;
using UnityEngine;

public class StorableUITrade : StorableUI {
	[SerializeField] private GameObject _selectedImage;
	private int selectedCount;

	private void Start() {
		_selectedImage.SetActive(false);
	}

	protected override void Update() {
		if (internalStorable.Max > 1) {
			textCount.text = selectedCount == 0
				? internalStorable.Count.ToString()
				: selectedCount + "/" + internalStorable.Count;
			if (textCount.gameObject != null)
				textCount.gameObject.SetActive(true);
		} else {
			textCount.text = null;
			if (textCount.gameObject != null)
				textCount.gameObject.SetActive(false);
		}

		selectedImage.gameObject.SetActive(isSelected);
		var color = enabledBackgroundColor;
		if (!isEnabled)
			color = disabledBackgroundColor;
		ImageBackground.color = color;
	}

	protected override void CalculatePosition() {
		if (internalStorable == null || internalStorable.IsDisposed)
			return;
		base.CalculatePosition();
		var vector2 = !cellStyle.IsSlot
			? InventoryUtility.CalculateInnerSize(((StorableComponent)internalStorable).Placeholder.Grid, cellStyle)
			: InventoryUtility.CalculateInnerSize(gridSlot, cellStyle);
		selectedImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
			vector2.x + cellStyle.BackgroundImageOffset.x * 2f);
		selectedImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
			vector2.y + cellStyle.BackgroundImageOffset.x * 2f);
	}

	public int GetSelectedCount() {
		return selectedCount;
	}

	public void SetSelectedCount(int count, bool isInit = false) {
		selectedCount = count;
		if (InputService.Instance.JoystickUsed) {
			_selectedImage.SetActive(count > 0);
			if (isInit)
				isSelected = false;
		} else {
			isSelected = count > 0;
			_selectedImage.SetActive(false);
		}

		Update();
	}
}