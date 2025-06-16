using System;

namespace Engine.Source.Services.Times
{
  public static class TimerUtility
  {
    public static string ToLongTimeString(this TimeSpan time)
    {
      return string.Format("{0:d}:{1:d2}:{2:d2}:{3:d2}", (object) time.Days, (object) time.Hours, (object) time.Minutes, (object) time.Seconds);
    }

    public static string ToShortTimeString(this TimeSpan time)
    {
      return string.Format("{0:d2}:{1:d2}", (object) time.Hours, (object) time.Minutes);
    }
  }
}
