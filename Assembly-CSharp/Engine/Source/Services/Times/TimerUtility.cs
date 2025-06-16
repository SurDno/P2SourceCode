using System;

namespace Engine.Source.Services.Times;

public static class TimerUtility {
	public static string ToLongTimeString(this TimeSpan time) {
		return string.Format("{0:d}:{1:d2}:{2:d2}:{3:d2}", time.Days, time.Hours, time.Minutes, time.Seconds);
	}

	public static string ToShortTimeString(this TimeSpan time) {
		return string.Format("{0:d2}:{1:d2}", time.Hours, time.Minutes);
	}
}