// Decompiled with JetBrains decompiler
// Type: TextTooltipViewInstance
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Impl.UI.Controls;
using UnityEngine;

#nullable disable
public class TextTooltipViewInstance : TextTooltipView
{
  [SerializeField]
  private StringView textView;
  [SerializeField]
  private ProgressView opacityView;
  [SerializeField]
  private float fadeInTime = 1f;
  [SerializeField]
  private float fadeOutTime = 1f;
  private TextTooltipViewInstance.State state = TextTooltipViewInstance.State.Disabled;
  private float phase;
  private string nextText;
  private Vector2 nextPosition;

  public override void Hide()
  {
    if (this.state == TextTooltipViewInstance.State.Disabled)
      return;
    this.nextText = (string) null;
    this.SetState(TextTooltipViewInstance.State.FadingOut);
  }

  public void Update()
  {
    if (this.state == TextTooltipViewInstance.State.FadingIn)
    {
      if ((double) this.fadeInTime == 0.0)
        this.phase = 1f;
      else
        this.phase += Time.deltaTime / this.fadeInTime;
      if ((double) this.phase >= 1.0)
      {
        this.phase = 1f;
        this.opacityView.Progress = 1f;
        this.SetState(TextTooltipViewInstance.State.Enabled);
      }
      else
        this.opacityView.Progress = this.phase;
    }
    else
    {
      if (this.state != TextTooltipViewInstance.State.FadingOut)
        return;
      if ((double) this.fadeOutTime == 0.0)
        this.phase = 0.0f;
      else
        this.phase -= Time.deltaTime / this.fadeOutTime;
      if ((double) this.phase <= 0.0)
      {
        this.phase = 0.0f;
        this.opacityView.Progress = 0.0f;
        if (this.nextText != null)
        {
          this.SetData(this.nextPosition, this.nextText);
          this.nextText = (string) null;
          this.SetState(TextTooltipViewInstance.State.FadingIn);
        }
        else
          this.SetState(TextTooltipViewInstance.State.Disabled);
      }
      else
        this.opacityView.Progress = this.phase;
    }
  }

  public override void Show(Vector2 screenPosition, string text)
  {
    if (text == null)
      this.Hide();
    else if (this.state == TextTooltipViewInstance.State.Disabled)
    {
      this.SetData(screenPosition, text);
      this.SetState(TextTooltipViewInstance.State.FadingIn);
    }
    else
    {
      this.nextPosition = screenPosition;
      this.nextText = text;
      this.SetState(TextTooltipViewInstance.State.FadingOut);
    }
  }

  private void SetState(TextTooltipViewInstance.State newState)
  {
    if (this.state == newState)
      return;
    this.state = newState;
    this.enabled = this.state == TextTooltipViewInstance.State.FadingIn || this.state == TextTooltipViewInstance.State.FadingOut;
  }

  private void SetData(Vector2 screenPosition, string text)
  {
    this.textView.StringValue = text;
    Vector2 vector2 = new Vector2((float) Screen.width * 0.5f, (float) Screen.height * 0.5f);
    RectTransform transform = (RectTransform) this.transform;
    transform.pivot = new Vector2((double) screenPosition.x < (double) vector2.x ? 0.0f : 1f, (double) screenPosition.y < (double) vector2.y ? 0.0f : 1f);
    transform.anchoredPosition = screenPosition;
  }

  private enum State
  {
    Disabled,
    FadingIn,
    Enabled,
    FadingOut,
  }
}
