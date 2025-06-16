// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.DelayTimer
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using System;

#nullable disable
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
