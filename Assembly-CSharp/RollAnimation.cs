using System;
using Engine.Impl.UI.Controls;

public class RollAnimation : MonoBehaviour
{
  [SerializeField]
  private ProgressView position;
  [SerializeField]
  private HideableView visible;
  [SerializeField]
  private HideableView failView;
  [SerializeField]
  private HideableView successView;
  [SerializeField]
  private float duration = 1f;
  [SerializeField]
  private Vector2 rateRange;
  private float rate;
  private float time;
  private float targetValue;
  private bool success;

  public event Action FinishEvent;

  public float Duration => duration;

  private void Update()
  {
    if (time <= 0.0)
      return;
    time -= Time.deltaTime / duration;
    if (time <= 0.0)
      Finish();
    else
      ShowPosition(targetValue - time * time * rate);
  }

  public void ShowPosition(float value)
  {
    position.FloatValue = NormalizedSine(Mathf.PingPong(value, 1f));
  }

  public void Set(float targetValue, bool success)
  {
    this.success = success;
    this.targetValue = NormalizedArcsine(targetValue);
    rate = UnityEngine.Random.Range(rateRange.x, rateRange.y);
    ShowPosition(targetValue - rate);
    visible.Visible = true;
    time = 1f;
  }

  public void Skip()
  {
    this.GetComponent<AudioSource>().Stop();
    Finish();
  }

  private void Finish()
  {
    time = 0.0f;
    if (success)
      successView.Visible = true;
    else
      failView.Visible = true;
    ShowPosition(targetValue);
    Action finishEvent = FinishEvent;
    if (finishEvent == null)
      return;
    finishEvent();
  }

  private float NormalizedSine(float value)
  {
    return (float) (0.5 - (double) Mathf.Cos(value * 3.14159274f) * 0.5);
  }

  private float NormalizedArcsine(float value)
  {
    return Mathf.Acos((float) ((0.5 - value) * 2.0)) / 3.14159274f;
  }
}
