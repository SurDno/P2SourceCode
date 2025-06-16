// Decompiled with JetBrains decompiler
// Type: TOD_Time
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
public class TOD_Time : MonoBehaviour
{
  [Tooltip("Length of one day in minutes.")]
  [TOD_Min(0.0f)]
  public float DayLengthInMinutes = 30f;
  [Tooltip("Progress time at runtime.")]
  public bool ProgressTime = true;
  [Tooltip("Set the date to the current device date on start.")]
  public bool UseDeviceDate = false;
  [Tooltip("Set the time to the current device time on start.")]
  public bool UseDeviceTime = false;
  [Tooltip("Apply the time curve when progressing time.")]
  public bool UseTimeCurve = false;
  [Tooltip("Time progression curve.")]
  public AnimationCurve TimeCurve = AnimationCurve.Linear(0.0f, 0.0f, 24f, 24f);
  private TOD_Sky sky;
  private AnimationCurve timeCurve;
  private AnimationCurve timeCurveInverse;

  public event Action OnMinute;

  public event Action OnHour;

  public event Action OnDay;

  public event Action OnMonth;

  public event Action OnYear;

  public event Action OnSunrise;

  public event Action OnSunset;

  public void RefreshTimeCurve()
  {
    this.TimeCurve.preWrapMode = WrapMode.Once;
    this.TimeCurve.postWrapMode = WrapMode.Once;
    this.ApproximateCurve(this.TimeCurve, out this.timeCurve, out this.timeCurveInverse);
    this.timeCurve.preWrapMode = WrapMode.Loop;
    this.timeCurve.postWrapMode = WrapMode.Loop;
    this.timeCurveInverse.preWrapMode = WrapMode.Loop;
    this.timeCurveInverse.postWrapMode = WrapMode.Loop;
  }

  public float ApplyTimeCurve(float deltaTime)
  {
    float time = this.timeCurveInverse.Evaluate(this.sky.Cycle.Hour) + deltaTime;
    deltaTime = this.timeCurve.Evaluate(time) - this.sky.Cycle.Hour;
    if ((double) time >= 24.0)
      deltaTime += (float) ((int) time / 24 * 24);
    else if ((double) time < 0.0)
      deltaTime += (float) (((int) time / 24 - 1) * 24);
    return deltaTime;
  }

  public void AddHours(float hours, bool adjust = true)
  {
    if (this.UseTimeCurve & adjust)
      hours = this.ApplyTimeCurve(hours);
    DateTime dateTime1 = this.sky.Cycle.DateTime;
    DateTime dateTime2 = dateTime1.AddHours((double) hours);
    this.sky.Cycle.DateTime = dateTime2;
    if (dateTime2.Year > dateTime1.Year)
    {
      Action onYear = this.OnYear;
      if (onYear != null)
        onYear();
      Action onMonth = this.OnMonth;
      if (onMonth != null)
        onMonth();
      Action onDay = this.OnDay;
      if (onDay != null)
        onDay();
      Action onHour = this.OnHour;
      if (onHour != null)
        onHour();
      Action onMinute = this.OnMinute;
      if (onMinute != null)
        onMinute();
    }
    else if (dateTime2.Month > dateTime1.Month)
    {
      Action onMonth = this.OnMonth;
      if (onMonth != null)
        onMonth();
      Action onDay = this.OnDay;
      if (onDay != null)
        onDay();
      Action onHour = this.OnHour;
      if (onHour != null)
        onHour();
      Action onMinute = this.OnMinute;
      if (onMinute != null)
        onMinute();
    }
    else if (dateTime2.Day > dateTime1.Day)
    {
      Action onDay = this.OnDay;
      if (onDay != null)
        onDay();
      Action onHour = this.OnHour;
      if (onHour != null)
        onHour();
      Action onMinute = this.OnMinute;
      if (onMinute != null)
        onMinute();
    }
    else if (dateTime2.Hour > dateTime1.Hour)
    {
      Action onHour = this.OnHour;
      if (onHour != null)
        onHour();
      Action onMinute = this.OnMinute;
      if (onMinute != null)
        onMinute();
    }
    else if (dateTime2.Minute > dateTime1.Minute)
    {
      Action onMinute = this.OnMinute;
      if (onMinute != null)
        onMinute();
    }
    double totalHours1 = dateTime1.TimeOfDay.TotalHours;
    double totalHours2 = dateTime2.TimeOfDay.TotalHours;
    if (totalHours1 < (double) this.sky.SunriseTime && totalHours2 >= (double) this.sky.SunriseTime)
    {
      Action onSunrise = this.OnSunrise;
      if (onSunrise != null)
        onSunrise();
    }
    if (totalHours1 >= (double) this.sky.SunsetTime || totalHours2 < (double) this.sky.SunsetTime)
      return;
    Action onSunset = this.OnSunset;
    if (onSunset != null)
      onSunset();
  }

  public void AddSeconds(float seconds, bool adjust = true) => this.AddHours(seconds / 3600f);

  private void CalculateLinearTangents(Keyframe[] keys)
  {
    for (int index = 0; index < keys.Length; ++index)
    {
      Keyframe key1 = keys[index];
      if (index > 0)
      {
        Keyframe key2 = keys[index - 1];
        key1.inTangent = (float) (((double) key1.value - (double) key2.value) / ((double) key1.time - (double) key2.time));
      }
      if (index < keys.Length - 1)
      {
        Keyframe key3 = keys[index + 1];
        key1.outTangent = (float) (((double) key3.value - (double) key1.value) / ((double) key3.time - (double) key1.time));
      }
      keys[index] = key1;
    }
  }

  private void ApproximateCurve(
    AnimationCurve source,
    out AnimationCurve approxCurve,
    out AnimationCurve approxInverse)
  {
    Keyframe[] keys1 = new Keyframe[25];
    Keyframe[] keys2 = new Keyframe[25];
    float time1 = -0.01f;
    for (int time2 = 0; time2 < 25; ++time2)
    {
      time1 = Mathf.Max(time1 + 0.01f, source.Evaluate((float) time2));
      keys1[time2] = new Keyframe((float) time2, time1);
      keys2[time2] = new Keyframe(time1, (float) time2);
    }
    this.CalculateLinearTangents(keys1);
    this.CalculateLinearTangents(keys2);
    approxCurve = new AnimationCurve(keys1);
    approxInverse = new AnimationCurve(keys2);
  }

  protected void Awake()
  {
    this.sky = this.GetComponent<TOD_Sky>();
    if (this.UseDeviceDate)
    {
      this.sky.Cycle.Year = DateTime.Now.Year;
      this.sky.Cycle.Month = DateTime.Now.Month;
      this.sky.Cycle.Day = DateTime.Now.Day;
    }
    if (this.UseDeviceTime)
      this.sky.Cycle.Hour = (float) DateTime.Now.TimeOfDay.TotalHours;
    this.RefreshTimeCurve();
  }

  protected void FixedUpdate()
  {
    if (!this.ProgressTime || (double) this.DayLengthInMinutes <= 0.0)
      return;
    this.AddSeconds(Time.deltaTime * (1440f / this.DayLengthInMinutes));
  }
}
