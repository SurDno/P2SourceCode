// Decompiled with JetBrains decompiler
// Type: HUDStamina
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class HUDStamina : MonoBehaviour
{
  [SerializeField]
  private CanvasGroup canvasGroup;
  [SerializeField]
  private Engine.Impl.UI.Controls.Gradient leftBar;
  [SerializeField]
  private Engine.Impl.UI.Controls.Gradient middleBar;
  [SerializeField]
  private Engine.Impl.UI.Controls.Gradient rightBar;
  [SerializeField]
  private CanvasGroup marker;
  [SerializeField]
  private CanvasGroup markerLabel;
  [SerializeField]
  private float markerVisibilityThreshold;
  [SerializeField]
  private Vector2 mainFade = Vector2.one;
  [Header("Alert")]
  [SerializeField]
  private Color blinkColor = Color.red;
  [SerializeField]
  private float blinkRate = 1f;
  private Color baseColor;
  private float lastMaxValue;
  private bool labelFadingIn;
  private float blinkPhase;

  public float Value { get; set; }

  public float MaxValue { get; set; }

  public bool Alert { get; set; }

  private void Start() => this.baseColor = this.rightBar.color;

  private void Update()
  {
    bool flag1 = (double) this.Value < (double) this.MaxValue;
    this.canvasGroup.alpha = Mathf.MoveTowards(this.canvasGroup.alpha, flag1 ? 1f : 0.0f, Time.unscaledDeltaTime / (flag1 ? this.mainFade.x : this.mainFade.y));
    if ((double) this.canvasGroup.alpha > 0.0)
    {
      this.canvasGroup.gameObject.SetActive(true);
      float x = 1f - this.MaxValue;
      this.leftBar.EndPosition = x;
      this.middleBar.StartPosition = x;
      Vector2 vector2 = new Vector2(x, 0.5f);
      RectTransform transform = (RectTransform) this.marker.transform;
      transform.anchorMin = vector2;
      transform.anchorMax = vector2;
      this.middleBar.EndPosition = x + this.Value;
      bool flag2 = (double) this.MaxValue < (double) this.markerVisibilityThreshold;
      this.marker.alpha = Mathf.MoveTowards(this.marker.alpha, flag2 ? 1f : 0.0f, Time.unscaledDeltaTime / (flag2 ? this.mainFade.x : this.mainFade.y));
      bool flag3 = (double) this.lastMaxValue != (double) this.MaxValue;
      this.lastMaxValue = this.MaxValue;
      if (flag3)
        this.labelFadingIn = true;
      this.markerLabel.alpha = Mathf.MoveTowards(this.markerLabel.alpha, this.labelFadingIn ? 1f : 0.0f, Time.unscaledDeltaTime / (this.labelFadingIn ? this.mainFade.x : this.mainFade.y));
      if ((double) this.markerLabel.alpha == 1.0)
        this.labelFadingIn = false;
      if (this.Alert)
        this.SetBlinkPhase(this.blinkPhase + Time.deltaTime * this.blinkRate);
      else if ((double) this.blinkPhase > 1.0)
      {
        this.SetBlinkPhase(Mathf.MoveTowards(this.blinkPhase, 2f, Time.deltaTime * this.blinkRate));
      }
      else
      {
        if ((double) this.blinkPhase <= 0.0)
          return;
        this.SetBlinkPhase(Mathf.MoveTowards(this.blinkPhase, 0.0f, Time.deltaTime * this.blinkRate));
      }
    }
    else
    {
      this.marker.alpha = 0.0f;
      this.markerLabel.alpha = 0.0f;
      this.labelFadingIn = false;
      this.canvasGroup.gameObject.SetActive(false);
    }
  }

  private void SetBlinkPhase(float value)
  {
    if ((double) value >= 2.0)
      value %= 2f;
    if ((double) this.blinkPhase == (double) value)
      return;
    this.blinkPhase = value;
    this.rightBar.color = Color.Lerp(this.blinkColor, this.baseColor, Mathf.Abs(1f - value));
  }
}
