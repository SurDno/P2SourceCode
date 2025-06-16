using UnityEngine;
using UnityEngine.Serialization;

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
    private float currentValue = 0.0f;
    private float tailValue = 0.0f;
    private float velocity = 0.0f;

    public Sprite Image
    {
      get => this.image;
      set => this.image = value;
    }

    public bool IsReversed
    {
      get => this.isReversed;
      set => this.isReversed = value;
    }

    public override float Progress
    {
      get => this.CurrentValue;
      set => this.CurrentValue = value;
    }

    public float CurrentValue
    {
      get => this.currentValue;
      set
      {
        if ((double) this.currentValue == (double) value)
          return;
        this.currentValue = value;
        if (this.gameObject.activeInHierarchy && this.enabled)
          return;
        this.TailValue = value;
      }
    }

    public float TailValue
    {
      get => this.tailValue;
      set
      {
        if ((double) this.tailValue == (double) value)
          return;
        this.tailValue = value;
      }
    }

    private Color FillColor => this.IsReversed ? this.negativeFillColor : this.positiveFillColor;

    private Color BaseColor => this.IsReversed ? this.negativeBaseColor : this.positiveBaseColor;

    private void Start()
    {
      this.rightFill = this.CreateGradient();
      RectTransform transform = (RectTransform) this.rightFill.transform;
      transform.offsetMin = new Vector2(0.0f, -2f);
      transform.offsetMax = new Vector2(0.0f, -2f);
      this.rightFill.color = this.BaseColor;
      this.leftFill = this.CreateGradient();
      this.leftFill.color = this.FillColor;
      this.delta = this.CreateGradient();
      Color markerColor = this.markerColor with { a = 0.0f };
      this.markerLeft = this.CreateGradient();
      this.markerLeft.StartColor = markerColor;
      this.markerLeft.EndColor = this.markerColor;
      this.markerRight = this.CreateGradient();
      this.markerRight.StartColor = this.markerColor;
      this.markerRight.EndColor = markerColor;
      this.Redraw();
    }

    private Gradient CreateGradient()
    {
      GameObject gameObject = new GameObject("Gradient", new System.Type[3]
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
      component2.Sprite = this.Image;
      component2.material = this.material;
      return component2;
    }

    private float BlinkPhase()
    {
      float f = Time.unscaledTime * 2f;
      return (float) (0.25 + (double) Mathf.Abs((f - Mathf.Round(f)) * 2f) * 0.75);
    }

    private void Redraw()
    {
      float num1 = this.rightLimit - this.leftLimit;
      float num2 = this.leftLimit + num1 * this.currentValue;
      float num3;
      if ((double) this.tailValue == (double) this.currentValue)
      {
        num3 = num2;
        this.delta.gameObject.SetActive(false);
      }
      else if ((double) this.tailValue < (double) this.currentValue)
      {
        num3 = this.leftLimit + num1 * this.tailValue;
        float num4 = num2;
        if (this.IsReversed)
          this.delta.color = Color.Lerp(this.FillColor, this.negativeBlinkColor, this.BlinkPhase());
        else
          this.delta.color = Color.Lerp(this.BaseColor, this.positiveBlinkColor, this.BlinkPhase());
        this.delta.StartPosition = num3;
        this.delta.EndPosition = num4;
        this.delta.gameObject.SetActive(true);
      }
      else
      {
        num3 = num2;
        float num5 = this.leftLimit + num1 * this.tailValue;
        if (this.IsReversed)
          this.delta.color = Color.Lerp(this.positiveBaseColor, this.positiveBlinkColor, this.BlinkPhase());
        else
          this.delta.color = Color.Lerp(this.positiveFillColor, this.negativeBlinkColor, this.BlinkPhase());
        this.delta.StartPosition = num3;
        this.delta.EndPosition = num5;
        this.delta.gameObject.SetActive(true);
      }
      this.leftFill.EndPosition = num3;
      this.leftFill.color = this.FillColor;
      this.rightFill.color = this.BaseColor;
      this.markerLeft.StartPosition = num2 - this.markerWidth;
      this.markerLeft.EndPosition = num2;
      this.markerRight.StartPosition = num2;
      this.markerRight.EndPosition = num2 + this.markerWidth;
    }

    private void Update()
    {
      if ((double) this.tailValue < (double) this.currentValue)
      {
        if ((double) this.velocity < 0.0)
          this.velocity = 0.0f;
        this.velocity += Time.deltaTime * this.tailAcceleration;
        this.tailValue += this.velocity * Time.deltaTime;
        if ((double) this.tailValue > (double) this.currentValue)
          this.tailValue = this.currentValue;
      }
      else if ((double) this.tailValue > (double) this.currentValue)
      {
        if ((double) this.velocity > 0.0)
          this.velocity = 0.0f;
        this.velocity -= Time.deltaTime * this.tailAcceleration;
        this.tailValue += this.velocity * Time.deltaTime;
        if ((double) this.tailValue < (double) this.currentValue)
          this.tailValue = this.currentValue;
      }
      if ((double) this.tailValue == (double) this.currentValue)
        this.velocity = 0.0f;
      this.Redraw();
    }

    public override void SkipAnimation() => this.TailValue = this.CurrentValue;
  }
}
