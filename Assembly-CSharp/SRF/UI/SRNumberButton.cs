using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SRF.UI
{
  [AddComponentMenu("SRF/UI/SRNumberButton")]
  public class SRNumberButton : 
    Button,
    IPointerClickHandler,
    IEventSystemHandler,
    IPointerDownHandler,
    IPointerUpHandler
  {
    private const float ExtraThreshold = 3f;
    public const float Delay = 0.4f;
    private float _delayTime;
    private float _downTime;
    private bool _isDown;
    public double Amount = 1.0;
    public SRNumberSpinner TargetField;

    public override void OnPointerDown(PointerEventData eventData)
    {
      base.OnPointerDown(eventData);
      if (!this.interactable)
        return;
      this.Apply();
      this._isDown = true;
      this._downTime = Time.realtimeSinceStartup;
      this._delayTime = this._downTime + 0.4f;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
      base.OnPointerUp(eventData);
      this._isDown = false;
    }

    protected virtual void Update()
    {
      if (!this._isDown || (double) this._delayTime > (double) Time.realtimeSinceStartup)
        return;
      this.Apply();
      float num1 = 0.2f;
      int num2 = Mathf.RoundToInt((float) (((double) Time.realtimeSinceStartup - (double) this._downTime) / 3.0));
      for (int index = 0; index < num2; ++index)
        num1 *= 0.5f;
      this._delayTime = Time.realtimeSinceStartup + num1;
    }

    private void Apply()
    {
      double num = double.Parse(this.TargetField.text) + this.Amount;
      if (num > this.TargetField.MaxValue)
        num = this.TargetField.MaxValue;
      if (num < this.TargetField.MinValue)
        num = this.TargetField.MinValue;
      this.TargetField.text = num.ToString();
      this.TargetField.onEndEdit.Invoke(this.TargetField.text);
    }
  }
}
