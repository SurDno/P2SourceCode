using System;

namespace Engine.Impl.UI.Controls
{
  public class StatBar : ProgressViewBase
  {
    [SerializeField]
    [FormerlySerializedAs("leftFillColor")]
    private Color positiveFillColor;
    [SerializeField]
    [FormerlySerializedAs("reversedLeftFillColor")]
    private Color negativeFillColor;
    [SerializeField]
    [FormerlySerializedAs("reversedRightFillColor")]
    private Color negativeBaseColor;
    [SerializeField]
    [FormerlySerializedAs("rightFillColor")]
    private Color positiveBaseColor;
    [SerializeField]
    [FormerlySerializedAs("leftBlinkColor")]
    private Color negativeBlinkColor;
    [SerializeField]
    [FormerlySerializedAs("rightBlinkColor")]
    private Color positiveBlinkColor;
    [SerializeField]
    [FormerlySerializedAs("markerColor")]
    private Color markerColor;
    [SerializeField]
    [FormerlySerializedAs("leftLimit")]
    private float leftLimit = 0.05f;
    [SerializeField]
    [FormerlySerializedAs("rightLimit")]
    private float rightLimit = 0.95f;
    [SerializeField]
    [FormerlySerializedAs("markerWidth")]
    private float markerWidth = 0.05f;
    [SerializeField]
    [FormerlySerializedAs("tailAcceleration")]
    private float tailAcceleration = 0.5f;
    [Space]
    [SerializeField]
    [FormerlySerializedAs("Material")]
    private Material material;
    [SerializeField]
    [FormerlySerializedAs("Image")]
    private Sprite image;
    [SerializeField]
    [FormerlySerializedAs("IsReversed")]
    private bool isReversed;
    private Gradient leftFill;
    private Gradient delta;
    private Gradient rightFill;
    private Gradient markerLeft;
    private Gradient markerRight;
    [SerializeField]
    [Range(0.0f, 1f)]
    private float currentValue;
    private float tailValue;
    private float velocity;

    public Sprite Image
    {
      get => image;
      set => image = value;
    }

    public bool IsReversed
    {
      get => isReversed;
      set => isReversed = value;
    }

    public override float Progress
    {
      get => CurrentValue;
      set => CurrentValue = value;
    }

    public float CurrentValue
    {
      get => currentValue;
      set
      {
        if (currentValue == (double) value)
          return;
        currentValue = value;
        if (this.gameObject.activeInHierarchy && this.enabled)
          return;
        TailValue = value;
      }
    }

    public float TailValue
    {
      get => tailValue;
      set
      {
        if (tailValue == (double) value)
          return;
        tailValue = value;
      }
    }

    private Color FillColor => IsReversed ? negativeFillColor : positiveFillColor;

    private Color BaseColor => IsReversed ? negativeBaseColor : positiveBaseColor;

    private void Start()
    {
      rightFill = CreateGradient();
      RectTransform transform = (RectTransform) rightFill.transform;
      transform.offsetMin = new Vector2(0.0f, -2f);
      transform.offsetMax = new Vector2(0.0f, -2f);
      rightFill.color = BaseColor;
      leftFill = CreateGradient();
      leftFill.color = FillColor;
      delta = CreateGradient();
      Color markerColor = this.markerColor with { a = 0.0f };
      markerLeft = CreateGradient();
      markerLeft.StartColor = markerColor;
      markerLeft.EndColor = this.markerColor;
      markerRight = CreateGradient();
      markerRight.StartColor = this.markerColor;
      markerRight.EndColor = markerColor;
      Redraw();
    }

    private Gradient CreateGradient()
    {
      GameObject gameObject = new GameObject("Gradient", new Type[3]
      {
        typeof (RectTransform),
        typeof (CanvasRenderer),
        typeof (Gradient)
      });
      RectTransform component1 = gameObject.GetComponent<RectTransform>();
      component1.SetParent(this.transform, false);
      component1.anchorMin = Vector2.zero;
      component1.anchorMax = Vector2.one;
      component1.offsetMin = Vector2.zero;
      component1.offsetMax = Vector2.zero;
      Gradient component2 = gameObject.GetComponent<Gradient>();
      component2.Sprite = Image;
      component2.material = material;
      return component2;
    }

    private float BlinkPhase()
    {
      float f = Time.unscaledTime * 2f;
      return (float) (0.25 + (double) Mathf.Abs((f - Mathf.Round(f)) * 2f) * 0.75);
    }

    private void Redraw()
    {
      float num1 = rightLimit - leftLimit;
      float num2 = leftLimit + num1 * currentValue;
      float num3;
      if (tailValue == (double) currentValue)
      {
        num3 = num2;
        delta.gameObject.SetActive(false);
      }
      else if (tailValue < (double) currentValue)
      {
        num3 = leftLimit + num1 * tailValue;
        float num4 = num2;
        if (IsReversed)
          delta.color = Color.Lerp(FillColor, negativeBlinkColor, BlinkPhase());
        else
          delta.color = Color.Lerp(BaseColor, positiveBlinkColor, BlinkPhase());
        delta.StartPosition = num3;
        delta.EndPosition = num4;
        delta.gameObject.SetActive(true);
      }
      else
      {
        num3 = num2;
        float num5 = leftLimit + num1 * tailValue;
        if (IsReversed)
          delta.color = Color.Lerp(positiveBaseColor, positiveBlinkColor, BlinkPhase());
        else
          delta.color = Color.Lerp(positiveFillColor, negativeBlinkColor, BlinkPhase());
        delta.StartPosition = num3;
        delta.EndPosition = num5;
        delta.gameObject.SetActive(true);
      }
      leftFill.EndPosition = num3;
      leftFill.color = FillColor;
      rightFill.color = BaseColor;
      markerLeft.StartPosition = num2 - markerWidth;
      markerLeft.EndPosition = num2;
      markerRight.StartPosition = num2;
      markerRight.EndPosition = num2 + markerWidth;
    }

    private void Update()
    {
      if (tailValue < (double) currentValue)
      {
        if (velocity < 0.0)
          velocity = 0.0f;
        velocity += Time.deltaTime * tailAcceleration;
        tailValue += velocity * Time.deltaTime;
        if (tailValue > (double) currentValue)
          tailValue = currentValue;
      }
      else if (tailValue > (double) currentValue)
      {
        if (velocity > 0.0)
          velocity = 0.0f;
        velocity -= Time.deltaTime * tailAcceleration;
        tailValue += velocity * Time.deltaTime;
        if (tailValue < (double) currentValue)
          tailValue = currentValue;
      }
      if (tailValue == (double) currentValue)
        velocity = 0.0f;
      Redraw();
    }

    public override void SkipAnimation() => TailValue = CurrentValue;
  }
}
