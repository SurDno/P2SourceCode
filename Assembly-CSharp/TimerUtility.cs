using System;
using Engine.Common.DateTime;

public static class TimerUtility {
	public static TimeSpan GetTimesOfDay2(this TimeSpan time) {
		return new TimeSpan(time.Hours, time.Minutes, time.Seconds);
	}

	public static TimesOfDay GetTimesOfDay(this TimeSpan time) {
		var timeSpan1 = ScriptableObjectInstance<GameSettingsData>.Instance.Night.Value;
		if (time.Hours > timeSpan1.Hours || (time.Hours == timeSpan1.Hours && time.Minutes >= timeSpan1.Minutes))
			return TimesOfDay.Night;
		var timeSpan2 = ScriptableObjectInstance<GameSettingsData>.Instance.Evening.Value;
		if (time.Hours > timeSpan2.Hours || (time.Hours == timeSpan2.Hours && time.Minutes >= timeSpan2.Minutes))
			return TimesOfDay.Evening;
		var timeSpan3 = ScriptableObjectInstance<GameSettingsData>.Instance.Day.Value;
		if (time.Hours > timeSpan3.Hours || (time.Hours == timeSpan3.Hours && time.Minutes >= timeSpan3.Minutes))
			return TimesOfDay.Day;
		var timeSpan4 = ScriptableObjectInstance<GameSettingsData>.Instance.Morning.Value;
		return time.Hours > timeSpan4.Hours || (time.Hours == timeSpan4.Hours && time.Minutes >= timeSpan4.Minutes)
			? TimesOfDay.Morning
			: TimesOfDay.Night;
	}

	public static bool IsDay(this TimesOfDay time) {
		return time == TimesOfDay.Morning || time == TimesOfDay.Day || time == TimesOfDay.Evening;
	}
}