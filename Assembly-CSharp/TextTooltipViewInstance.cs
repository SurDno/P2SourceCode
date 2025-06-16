using Engine.Impl.UI.Controls;
using UnityEngine;

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
  private State state = State.Disabled;
  private float phase;
  private string nextText;
  private Vector2 nextPosition;

  public override void Hide()
  {
    if (state == State.Disabled)
      return;
    nextText = null;
    SetState(State.FadingOut);
  }

  public void Update()
  {
    if (state == State.FadingIn)
    {
      if (fadeInTime == 0.0)
        phase = 1f;
      else
        phase += Time.deltaTime / fadeInTime;
      if (phase >= 1.0)
      {
        phase = 1f;
        opacityView.Progress = 1f;
        SetState(State.Enabled);
      }
      else
        opacityView.Progress = phase;
    }
    else
    {
      if (state != State.FadingOut)
        return;
      if (fadeOutTime == 0.0)
        phase = 0.0f;
      else
        phase -= Time.deltaTime / fadeOutTime;
      if (phase <= 0.0)
      {
        phase = 0.0f;
        opacityView.Progress = 0.0f;
        if (nextText != null)
        {
          SetData(nextPosition, nextText);
          nextText = null;
          SetState(State.FadingIn);
        }
        else
          SetState(State.Disabled);
      }
      else
        opacityView.Progress = phase;
    }
  }

  public override void Show(Vector2 screenPosition, string text)
  {
    if (text == null)
      Hide();
    else if (state == State.Disabled)
    {
      SetData(screenPosition, text);
      SetState(State.FadingIn);
    }
    else
    {
      nextPosition = screenPosition;
      nextText = text;
      SetState(State.FadingOut);
    }
  }

  private void SetState(State newState)
  {
    if (state == newState)
      return;
    state = newState;
    enabled = state == State.FadingIn || state == State.FadingOut;
  }

  private void SetData(Vector2 screenPosition, string text)
  {
    textView.StringValue = text;
    Vector2 vector2 = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
    RectTransform transform = (RectTransform) this.transform;
    transform.pivot = new Vector2(screenPosition.x < (double) vector2.x ? 0.0f : 1f, screenPosition.y < (double) vector2.y ? 0.0f : 1f);
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
