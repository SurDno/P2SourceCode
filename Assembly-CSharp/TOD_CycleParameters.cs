using System;
using UnityEngine;

[Serializable]
public class TOD_CycleParameters
{
  [Tooltip("Current hour of the day.")]
  public float Hour = 12f;
  [Tooltip("Current day of the month.")]
  public int Day = 15;
  [Tooltip("Current month of the year.")]
  public int Month = 6;
  [Tooltip("Current year.")]
  [TOD_Range(1f, 9999f)]
  public int Year = 2000;

  public DateTime DateTime
  {
    get
    {
      DateTime dateTime = new DateTime(0L, DateTimeKind.Utc).AddYears(Year - 1);
      dateTime = dateTime.AddMonths(Month - 1);
      dateTime = dateTime.AddDays(Day - 1);
      return dateTime.AddHours(Hour);
    }
    set
    {
      Year = value.Year;
      Month = value.Month;
      Day = value.Day;
      Hour = (float) (value.Hour + value.Minute / 60.0 + value.Second / 3600.0 + value.Millisecond / 3600000.0);
    }
  }

  public long Ticks
  {
    get => DateTime.Ticks;
    set => DateTime = new DateTime(value, DateTimeKind.Utc);
  }
}
