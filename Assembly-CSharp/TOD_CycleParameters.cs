// Decompiled with JetBrains decompiler
// Type: TOD_CycleParameters
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
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
      DateTime dateTime = new DateTime(0L, DateTimeKind.Utc).AddYears(this.Year - 1);
      dateTime = dateTime.AddMonths(this.Month - 1);
      dateTime = dateTime.AddDays((double) (this.Day - 1));
      return dateTime.AddHours((double) this.Hour);
    }
    set
    {
      this.Year = value.Year;
      this.Month = value.Month;
      this.Day = value.Day;
      this.Hour = (float) ((double) value.Hour + (double) value.Minute / 60.0 + (double) value.Second / 3600.0 + (double) value.Millisecond / 3600000.0);
    }
  }

  public long Ticks
  {
    get => this.DateTime.Ticks;
    set => this.DateTime = new DateTime(value, DateTimeKind.Utc);
  }
}
