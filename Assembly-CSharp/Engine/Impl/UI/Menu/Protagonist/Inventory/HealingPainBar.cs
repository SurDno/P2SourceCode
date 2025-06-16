using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Engine.Impl.UI.Menu.Protagonist.Inventory
{
  public class HealingPainBar : MonoBehaviour
  {
    [SerializeField]
    [FormerlySerializedAs("TopFillImage")]
    private Image topFillImage;
    [SerializeField]
    [FormerlySerializedAs("BottomFillImage")]
    private Image bottomFillImage;
    [SerializeField]
    private float animationTime = 0.5f;
    private float animationTimeLeft = 0.0f;
    private bool isAnimating = false;
    private float currentAnimationValue = 0.0f;
    private float targetAnimationValue = 0.0f;

    public void Set(float currentValue) => this.Set(currentValue, currentValue);

    public void Set(float currentValue, float targetValue)
    {
      currentValue = Mathf.Clamp01(currentValue);
      targetValue = Mathf.Clamp01(targetValue);
      if ((double) currentValue < (double) targetValue)
      {
        float num = currentValue;
        targetValue = currentValue;
        currentValue = num;
      }
      this.topFillImage.fillAmount = 1f - currentValue;
      this.bottomFillImage.fillAmount = targetValue;
    }

    public void SetAndAnimate(float currentValue, float oldValue)
    {
      this.isAnimating = true;
      this.animationTimeLeft = this.animationTime;
      this.currentAnimationValue = currentValue;
      this.targetAnimationValue = oldValue - currentValue;
    }

    private void Update()
    {
      if (!this.isAnimating)
        return;
      this.animationTimeLeft -= Time.deltaTime;
      if ((double) this.animationTimeLeft <= 0.0)
      {
        this.isAnimating = false;
        this.animationTimeLeft = 0.0f;
      }
      this.Set(this.currentAnimationValue + this.targetAnimationValue * (this.animationTimeLeft / this.animationTime), this.currentAnimationValue);
    }
  }
}
