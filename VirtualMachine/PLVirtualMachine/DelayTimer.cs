using System;

namespace PLVirtualMachine
{
  public static class DelayTimer
  {
    private static TimeSpan delay;
    private static DateTime time;

    public static void Begin(TimeSpan delay)
    {
      DelayTimer.delay = delay;
      DelayTimer.time = DateTime.UtcNow;
    }

    public static bool Check
    {
      get
      {
        DateTime utcNow = DateTime.UtcNow;
        if (!(DelayTimer.time + DelayTimer.delay < utcNow))
          return false;
        DelayTimer.time = utcNow;
        return true;
      }
    }
  }
}
