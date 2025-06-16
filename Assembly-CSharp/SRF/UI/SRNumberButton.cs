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
      Apply();
      _isDown = true;
      _downTime = Time.realtimeSinceStartup;
      _delayTime = _downTime + 0.4f;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
      base.OnPointerUp(eventData);
      _isDown = false;
    }

    protected virtual void Update()
    {
      if (!_isDown || _delayTime > (double) Time.realtimeSinceStartup)
        return;
      Apply();
      float num1 = 0.2f;
      int num2 = Mathf.RoundToInt((float) (((double) Time.realtimeSinceStartup - _downTime) / 3.0));
      for (int index = 0; index < num2; ++index)
        num1 *= 0.5f;
      _delayTime = Time.realtimeSinceStartup + num1;
    }

    private void Apply()
    {
      double num = double.Parse(TargetField.text) + Amount;
      if (num > TargetField.MaxValue)
        num = TargetField.MaxValue;
      if (num < TargetField.MinValue)
        num = TargetField.MinValue;
      TargetField.text = num.ToString();
      TargetField.onEndEdit.Invoke(TargetField.text);
    }
  }
}
