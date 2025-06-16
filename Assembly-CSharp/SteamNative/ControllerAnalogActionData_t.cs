using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
  [StructLayout(LayoutKind.Sequential, Pack = 8)]
  internal struct ControllerAnalogActionData_t
  {
    public ControllerSourceMode EMode;
    public float X;
    public float Y;
    [MarshalAs(UnmanagedType.I1)]
    public bool BActive;

    public static ControllerAnalogActionData_t FromPointer(IntPtr p)
    {
      return Platform.PackSmall ? (PackSmall) Marshal.PtrToStructure(p, typeof (PackSmall)) : (ControllerAnalogActionData_t) Marshal.PtrToStructure(p, typeof (ControllerAnalogActionData_t));
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct PackSmall
    {
      public ControllerSourceMode EMode;
      public float X;
      public float Y;
      [MarshalAs(UnmanagedType.I1)]
      public bool BActive;

      public static implicit operator ControllerAnalogActionData_t(
        PackSmall d)
      {
        return new ControllerAnalogActionData_t {
          EMode = d.EMode,
          X = d.X,
          Y = d.Y,
          BActive = d.BActive
        };
      }
    }
  }
}
