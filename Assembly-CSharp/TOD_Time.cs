using System;
using UnityEngine;

public class TOD_Time : MonoBehaviour
{
  [Tooltip("Length of one day in minutes.")]
  [TOD_Min(0.0f)]
  public float DayLengthInMinutes = 30f;
  [Tooltip("Progress time at runtime.")]
  public bool ProgressTime = true;
  [Tooltip("Set the date to the current device date on start.")]
  public bool UseDeviceDate;
  [Tooltip("Set the time to the current device time on start.")]
  public bool UseDeviceTime;
  [Tooltip("Apply the time curve when progressing time.")]
  public bool UseTimeCurve;
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
    TimeCurve.preWrapMode = WrapMode.Once;
    TimeCurve.postWrapMode = WrapMode.Once;
    ApproximateCurve(TimeCurve, out timeCurve, out timeCurveInverse);
    timeCurve.preWrapMode = WrapMode.Loop;
    timeCurve.postWrapMode = WrapMode.Loop;
    timeCurveInverse.preWrapMode = WrapMode.Loop;
    timeCurveInverse.postWrapMode = WrapMode.Loop;
  }

  public float ApplyTimeCurve(float deltaTime)
  {
    float time = timeCurveInverse.Evaluate(sky.Cycle.Hour) + deltaTime;
    deltaTime = timeCurve.Evaluate(time) - sky.Cycle.Hour;
    if (time >= 24.0)
      deltaTime += (int) time / 24 * 24;
    else if (time < 0.0)
      deltaTime += ((int) time / 24 - 1) * 24;
    return deltaTime;
  }

  public void AddHours(float hours, bool adjust = true)
  {
    if (UseTimeCurve & adjust)
      hours = ApplyTimeCurve(hours);
    DateTime dateTime1 = sky.Cycle.DateTime;
    DateTime dateTime2 = dateTime1.AddHours(hours);
    sky.Cycle.DateTime = dateTime2;
    if (dateTime2.Year > dateTime1.Year)
    {
      Action onYear = OnYear;
      if (onYear != null)
        onYear();
      Action onMonth = OnMonth;
      if (onMonth != null)
        onMonth();
      Action onDay = OnDay;
      if (onDay != null)
        onDay();
      Action onHour = OnHour;
      if (onHour != null)
        onHour();
      Action onMinute = OnMinute;
      if (onMinute != null)
        onMinute();
    }
    else if (dateTime2.Month > dateTime1.Month)
    {
      Action onMonth = OnMonth;
      if (onMonth != null)
        onMonth();
      Action onDay = OnDay;
      if (onDay != null)
        onDay();
      Action onHour = OnHour;
      if (onHour != null)
        onHour();
      Action onMinute = OnMinute;
      if (onMinute != null)
        onMinute();
    }
    else if (dateTime2.Day > dateTime1.Day)
    {
      Action onDay = OnDay;
      if (onDay != null)
        onDay();
      Action onHour = OnHour;
      if (onHour != null)
        onHour();
      Action onMinute = OnMinute;
      if (onMinute != null)
        onMinute();
    }
    else if (dateTime2.Hour > dateTime1.Hour)
    {
      Action onHour = OnHour;
      if (onHour != null)
        onHour();
      Action onMinute = OnMinute;
      if (onMinute != null)
        onMinute();
    }
    else if (dateTime2.Minute > dateTime1.Minute)
    {
      Action onMinute = OnMinute;
      if (onMinute != null)
        onMinute();
    }
    double totalHours1 = dateTime1.TimeOfDay.TotalHours;
    double totalHours2 = dateTime2.TimeOfDay.TotalHours;
    if (totalHours1 < sky.SunriseTime && totalHours2 >= sky.SunriseTime)
    {
      Action onSunrise = OnSunrise;
      if (onSunrise != null)
        onSunrise();
    }
    if (totalHours1 >= sky.SunsetTime || totalHours2 < sky.SunsetTime)
      return;
    Action onSunset = OnSunset;
    if (onSunset != null)
      onSunset();
  }

  public void AddSeconds(float seconds, bool adjust = true) => AddHours(seconds / 3600f);

  private void CalculateLinearTangents(Keyframe[] keys)
  {
    for (int index = 0; index < keys.Length; ++index)
    {
      Keyframe key1 = keys[index];
      if (index > 0)
      {
        Keyframe key2 = keys[index - 1];
        key1.inTangent = (float) ((key1.value - (double) key2.value) / (key1.time - (double) key2.time));
      }
      if (index < keys.Length - 1)
      {
        Keyframe key3 = keys[index + 1];
        key1.outTangent = (float) ((key3.value - (double) key1.value) / (key3.time - (double) key1.time));
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
      time1 = Mathf.Max(time1 + 0.01f, source.Evaluate(time2));
      keys1[time2] = new Keyframe(time2, time1);
      keys2[time2] = new Keyframe(time1, time2);
    }
    CalculateLinearTangents(keys1);
    CalculateLinearTangents(keys2);
    approxCurve = new AnimationCurve(keys1);
    approxInverse = new AnimationCurve(keys2);
  }

  protected void Awake()
  {
    sky = GetComponent<TOD_Sky>();
    if (UseDeviceDate)
    {
      sky.Cycle.Year = DateTime.Now.Year;
      sky.Cycle.Month = DateTime.Now.Month;
      sky.Cycle.Day = DateTime.Now.Day;
    }
    if (UseDeviceTime)
      sky.Cycle.Hour = (float) DateTime.Now.TimeOfDay.TotalHours;
    RefreshTimeCurve();
  }

  protected void FixedUpdate()
  {
    if (!ProgressTime || DayLengthInMinutes <= 0.0)
      return;
    AddSeconds(Time.deltaTime * (1440f / DayLengthInMinutes));
  }
}
