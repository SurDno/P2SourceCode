// Decompiled with JetBrains decompiler
// Type: SteamNative.ControllerAnalogActionData_t
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Runtime.InteropServices;

#nullable disable
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
      return Platform.PackSmall ? (ControllerAnalogActionData_t) (ControllerAnalogActionData_t.PackSmall) Marshal.PtrToStructure(p, typeof (ControllerAnalogActionData_t.PackSmall)) : (ControllerAnalogActionData_t) Marshal.PtrToStructure(p, typeof (ControllerAnalogActionData_t));
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
        ControllerAnalogActionData_t.PackSmall d)
      {
        return new ControllerAnalogActionData_t()
        {
          EMode = d.EMode,
          X = d.X,
          Y = d.Y,
          BActive = d.BActive
        };
      }
    }
  }
}
