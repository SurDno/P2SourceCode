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
      time = DateTime.UtcNow;
    }

    public static bool Check
    {
      get
      {
        DateTime utcNow = DateTime.UtcNow;
        if (!(time + delay < utcNow))
          return false;
        time = utcNow;
        return true;
      }
    }
  }
}
