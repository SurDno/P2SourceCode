using Engine.Behaviours.Localization;
using Engine.Common.Commons;
using Engine.Common.Services;
using Engine.Common.Types;
using Engine.Impl.Services;
using Engine.Impl.UI.Controls;
using Engine.Source.Components.BoundCharacters;
using UnityEngine;
using UnityEngine.UI;

public class MapItemInfoWindow : MonoBehaviour {
	[SerializeField] private Localizer title;
	[SerializeField] private Text text;
	[Space] [SerializeField] private GameObject icons;
	[SerializeField] private GameObject craftIcon;
	[SerializeField] private GameObject saveIcon;
	[SerializeField] private GameObject sleepIcon;
	[SerializeField] private GameObject storeIcon;
	[SerializeField] private GameObject tradeIcon;
	[Space] [SerializeField] private GameObject portrait;
	[SerializeField] private Image portraitImage;
	[SerializeField] private StringView portraitText;
	[SerializeField] private Sprite fallbackPortraitSprite;
	[SerializeField] private Color dangerColor;
	[SerializeField] private Color diseasedColor;
	[SerializeField] private Color deadColor;
	[SerializeField] private GameObject medicated;
	private MapItemView targetView;

	public void Show(MapItemView itemView) {
		if (itemView == targetView)
			return;
		if (targetView == null)
			gameObject.SetActive(true);
		else
			targetView.SetHightlight(false);
		targetView = itemView;
		targetView.SetHightlight(true);
		var mapItem = targetView.Item;
		if (mapItem.Resource == null)
			return;
		GetComponent<LayoutElement>();
		var service = ServiceLocator.GetService<LocalizationService>();
		if (mapItem.Title != LocalizedText.Empty) {
			title.Signature = service.GetText(mapItem.Title);
			title.gameObject.SetActive(true);
		} else
			title.gameObject.SetActive(false);

		if (mapItem.Text != LocalizedText.Empty) {
			text.text = service.GetText(mapItem.Text);
			text.gameObject.SetActive(true);
		} else
			text.gameObject.SetActive(false);

		var component = mapItem.BoundCharacter?.GetComponent<BoundCharacterComponent>();
		if (component != null) {
			var boundHealthStateEnum = BoundCharacterUtility.PerceivedHealth(component);
			portraitImage.sprite = BoundCharacterUtility.StateSprite(component, boundHealthStateEnum) ??
			                       fallbackPortraitSprite;
			var str = BoundCharacterUtility.StateText(component, boundHealthStateEnum);
			if (str != null) {
				switch (boundHealthStateEnum) {
					case BoundHealthStateEnum.Danger:
						str = "<color=" + dangerColor.ToRGBHex() + ">" + str + "</color>";
						break;
					case BoundHealthStateEnum.Diseased:
						str = "<color=" + diseasedColor.ToRGBHex() + ">" + str + "</color>";
						break;
					case BoundHealthStateEnum.Dead:
						str = "<color=" + deadColor.ToRGBHex() + ">" + str + "</color>";
						break;
				}

				portraitText.StringValue = str;
				portraitText.gameObject.SetActive(true);
			} else
				portraitText.gameObject.SetActive(false);

			medicated.SetActive(BoundCharacterUtility.MedicineAttempted(component));
			portrait.SetActive(true);
		} else
			portrait.SetActive(false);

		craftIcon.SetActive(mapItem.CraftIcon.Value);
		saveIcon.SetActive(mapItem.SavePointIcon.Value);
		sleepIcon.SetActive(mapItem.SleepIcon.Value);
		storeIcon.SetActive(mapItem.StorageIcon.Value);
		tradeIcon.SetActive(mapItem.MerchantIcon.Value);
		icons.SetActive(craftIcon.activeSelf || saveIcon.activeSelf || sleepIcon.activeSelf || storeIcon.activeSelf ||
		                tradeIcon.activeSelf);
		UpdatePosition();
	}

	public void Hide(MapItemView mapItemView) {
		if (targetView != mapItemView)
			return;
		gameObject.SetActive(false);
		targetView.SetHightlight(false);
		targetView = null;
		portraitImage.sprite = null;
	}

	private void UpdatePosition() {
		if (targetView == null)
			return;
		var transform1 = (RectTransform)transform;
		var transform2 = (RectTransform)GetComponentInParent<Canvas>().transform;
		var vector2 = new Vector2(transform2.sizeDelta.x, transform2.sizeDelta.y);
		var worldPosition = targetView.WorldPosition;
		worldPosition.x = Mathf.Round(worldPosition.x);
		worldPosition.y = Mathf.Round(worldPosition.y);
		worldPosition.x /= transform2.localScale.x;
		worldPosition.y /= transform2.localScale.y;
		transform1.pivot = new Vector2(worldPosition.x > vector2.x * 0.699999988079071 ? 1f : 0.0f,
			worldPosition.y > vector2.y * 0.30000001192092896 ? 1f : 0.0f);
		transform1.anchoredPosition = worldPosition;
	}

	private void LateUpdate() {
		UpdatePosition();
	}
}