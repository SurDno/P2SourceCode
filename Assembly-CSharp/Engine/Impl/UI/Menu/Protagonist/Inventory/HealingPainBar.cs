// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Menu.Protagonist.Inventory.HealingPainBar
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

#nullable disable
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
