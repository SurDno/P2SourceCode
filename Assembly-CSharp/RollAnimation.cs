// Decompiled with JetBrains decompiler
// Type: RollAnimation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Impl.UI.Controls;
using System;
using UnityEngine;

#nullable disable
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

  public float Duration => this.duration;

  private void Update()
  {
    if ((double) this.time <= 0.0)
      return;
    this.time -= Time.deltaTime / this.duration;
    if ((double) this.time <= 0.0)
      this.Finish();
    else
      this.ShowPosition(this.targetValue - this.time * this.time * this.rate);
  }

  public void ShowPosition(float value)
  {
    this.position.FloatValue = this.NormalizedSine(Mathf.PingPong(value, 1f));
  }

  public void Set(float targetValue, bool success)
  {
    this.success = success;
    this.targetValue = this.NormalizedArcsine(targetValue);
    this.rate = UnityEngine.Random.Range(this.rateRange.x, this.rateRange.y);
    this.ShowPosition(targetValue - this.rate);
    this.visible.Visible = true;
    this.time = 1f;
  }

  public void Skip()
  {
    this.GetComponent<AudioSource>().Stop();
    this.Finish();
  }

  private void Finish()
  {
    this.time = 0.0f;
    if (this.success)
      this.successView.Visible = true;
    else
      this.failView.Visible = true;
    this.ShowPosition(this.targetValue);
    Action finishEvent = this.FinishEvent;
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
    return Mathf.Acos((float) ((0.5 - (double) value) * 2.0)) / 3.14159274f;
  }
}
