using UnityEngine;
using UnityEngine.UI;

namespace Engine.Impl.UI.Controls;

public class CompareMeterLine : MonoBehaviour {
	[SerializeField] private Transform Arrow;
	[SerializeField] private RectTransform Bar;
	[SerializeField] private RectTransform leftBorder;
	[SerializeField] private RectTransform rightBorder;
	[SerializeField] private GameObject priceItem;
	[SerializeField] private Image priceCoin;
	[SerializeField] private Sprite coinSprite;
	[SerializeField] private Sprite handSprite;
	[SerializeField] private Text priceText;
	[SerializeField] private Color priceColorEnabled;
	[SerializeField] private Color priceColorDisabled;
	[SerializeField] private GameObject leftArrow;
	[SerializeField] private GameObject rightArrow;
	private float currentValue = float.NaN;
	private float factor;
	private float oldFactor = 1f;
	private float targetValue = float.NaN;
	private bool barterMode;
	private int storedCoins;
	private int marketCoins;

	public float TargetValue {
		get => targetValue;
		set {
			targetValue = value;
			Calculate();
		}
	}

	public float CurrentValue {
		get => currentValue;
		set {
			currentValue = value;
			Calculate();
		}
	}

	public int StoredCoins {
		get => storedCoins;
		set => storedCoins = value;
	}

	public int MarketCoins {
		get => marketCoins;
		set => marketCoins = value;
	}

	public void Reset() {
		currentValue = 0.0f;
		targetValue = 0.0f;
		ShowResult(0.0f);
		ShowPrices();
	}

	protected void Start() {
		currentValue = 0.0f;
		targetValue = 0.0f;
		Calculate();
	}

	public void BarterMode(bool b) {
		barterMode = b;
		priceCoin.sprite = b ? handSprite : coinSprite;
		priceCoin.SetNativeSize();
	}

	protected void Calculate() {
		if (Mathf.Approximately(currentValue, targetValue))
			factor = 1f;
		else if (currentValue < (double)targetValue)
			factor = currentValue / targetValue;
		else {
			factor = targetValue / currentValue;
			factor = (float)(1.0 - factor + 1.0);
		}

		--factor;
		if (factor != (double)oldFactor && !float.IsNaN(factor)) {
			oldFactor = factor;
			ShowResult(factor);
		}

		ShowPrices();
	}

	private void ShowPrices() {
		priceItem.SetActive(currentValue != 0.0 || targetValue != 0.0);
		leftArrow.SetActive(currentValue < (double)targetValue);
		rightArrow.SetActive(currentValue > (double)targetValue);
		if (barterMode)
			priceText.text = " " + Mathf.Abs(currentValue - targetValue) + " ";
		else if (currentValue - (double)targetValue > marketCoins)
			priceText.text = " " + marketCoins + " / " + Mathf.Abs(currentValue - targetValue) + " ";
		else
			priceText.text = " " + Mathf.Abs(currentValue - targetValue) + " ";
		priceText.color = storedCoins >= targetValue - (double)currentValue ? priceColorEnabled : priceColorDisabled;
	}

	private void ShowResult(float result) {
		var vector2_1 = new Vector2(0.0f, Arrow.localPosition.y);
		var vector2_2 = new Vector2(0.0f, Arrow.localPosition.y);
		var a = new Vector2(0.0f, Arrow.localPosition.y);
		var vector2_3 = new Vector2(leftBorder.localPosition.x, Arrow.localPosition.y);
		var vector2_4 = new Vector2(rightBorder.localPosition.x, Arrow.localPosition.y);
		Vector2 current1;
		Vector2 current2;
		Vector2 vector2_5;
		if (result < 0.0) {
			current1 = Vector2.zero;
			var f = Mathf.Abs(result) * Vector2.Distance(a, vector2_3);
			current2 = Vector2.MoveTowards(current1, vector2_3, Mathf.Abs(f));
			vector2_5 = current2;
		} else {
			current2 = Vector2.zero;
			var f = Mathf.Abs(result) * Vector2.Distance(a, vector2_4);
			current1 = Vector2.MoveTowards(current2, vector2_4, Mathf.Abs(f));
			vector2_5 = current1;
		}

		Arrow.localPosition = vector2_5;
		Bar.localPosition = new Vector2(current2.x, Bar.localPosition.y);
		Bar.sizeDelta = new Vector2((current1 - current2).x, Bar.rect.height);
	}
}