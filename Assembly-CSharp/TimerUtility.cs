// Decompiled with JetBrains decompiler
// Type: TimerUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.DateTime;
using System;

#nullable disable
public static class TimerUtility
{
  public static TimeSpan GetTimesOfDay2(this TimeSpan time)
  {
    return new TimeSpan(time.Hours, time.Minutes, time.Seconds);
  }

  public static TimesOfDay GetTimesOfDay(this TimeSpan time)
  {
    TimeSpan timeSpan1 = ScriptableObjectInstance<GameSettingsData>.Instance.Night.Value;
    if (time.Hours > timeSpan1.Hours || time.Hours == timeSpan1.Hours && time.Minutes >= timeSpan1.Minutes)
      return TimesOfDay.Night;
    TimeSpan timeSpan2 = ScriptableObjectInstance<GameSettingsData>.Instance.Evening.Value;
    if (time.Hours > timeSpan2.Hours || time.Hours == timeSpan2.Hours && time.Minutes >= timeSpan2.Minutes)
      return TimesOfDay.Evening;
    TimeSpan timeSpan3 = ScriptableObjectInstance<GameSettingsData>.Instance.Day.Value;
    if (time.Hours > timeSpan3.Hours || time.Hours == timeSpan3.Hours && time.Minutes >= timeSpan3.Minutes)
      return TimesOfDay.Day;
    TimeSpan timeSpan4 = ScriptableObjectInstance<GameSettingsData>.Instance.Morning.Value;
    return time.Hours > timeSpan4.Hours || time.Hours == timeSpan4.Hours && time.Minutes >= timeSpan4.Minutes ? TimesOfDay.Morning : TimesOfDay.Night;
  }

  public static bool IsDay(this TimesOfDay time)
  {
    return time == TimesOfDay.Morning || time == TimesOfDay.Day || time == TimesOfDay.Evening;
  }
}
