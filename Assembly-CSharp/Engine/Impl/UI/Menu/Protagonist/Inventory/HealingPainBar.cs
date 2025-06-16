using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Engine.Impl.UI.Menu.Protagonist.Inventory;

public class HealingPainBar : MonoBehaviour {
	[SerializeField] [FormerlySerializedAs("TopFillImage")]
	private Image topFillImage;

	[SerializeField] [FormerlySerializedAs("BottomFillImage")]
	private Image bottomFillImage;

	[SerializeField] private float animationTime = 0.5f;
	private float animationTimeLeft;
	private bool isAnimating;
	private float currentAnimationValue;
	private float targetAnimationValue;

	public void Set(float currentValue) {
		Set(currentValue, currentValue);
	}

	public void Set(float currentValue, float targetValue) {
		currentValue = Mathf.Clamp01(currentValue);
		targetValue = Mathf.Clamp01(targetValue);
		if (currentValue < (double)targetValue) {
			var num = currentValue;
			targetValue = currentValue;
			currentValue = num;
		}

		topFillImage.fillAmount = 1f - currentValue;
		bottomFillImage.fillAmount = targetValue;
	}

	public void SetAndAnimate(float currentValue, float oldValue) {
		isAnimating = true;
		animationTimeLeft = animationTime;
		currentAnimationValue = currentValue;
		targetAnimationValue = oldValue - currentValue;
	}

	private void Update() {
		if (!isAnimating)
			return;
		animationTimeLeft -= Time.deltaTime;
		if (animationTimeLeft <= 0.0) {
			isAnimating = false;
			animationTimeLeft = 0.0f;
		}

		Set(currentAnimationValue + targetAnimationValue * (animationTimeLeft / animationTime), currentAnimationValue);
	}
}