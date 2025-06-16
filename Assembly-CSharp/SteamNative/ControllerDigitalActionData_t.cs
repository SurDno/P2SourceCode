// Decompiled with JetBrains decompiler
// Type: SteamNative.ControllerDigitalActionData_t
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Runtime.InteropServices;

#nullable disable
namespace SteamNative
{
  [StructLayout(LayoutKind.Sequential, Pack = 8)]
  internal struct ControllerDigitalActionData_t
  {
    [MarshalAs(UnmanagedType.I1)]
    public bool BState;
    [MarshalAs(UnmanagedType.I1)]
    public bool BActive;

    public static ControllerDigitalActionData_t FromPointer(IntPtr p)
    {
      return Platform.PackSmall ? (ControllerDigitalActionData_t) (ControllerDigitalActionData_t.PackSmall) Marshal.PtrToStructure(p, typeof (ControllerDigitalActionData_t.PackSmall)) : (ControllerDigitalActionData_t) Marshal.PtrToStructure(p, typeof (ControllerDigitalActionData_t));
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct PackSmall
    {
      [MarshalAs(UnmanagedType.I1)]
      public bool BState;
      [MarshalAs(UnmanagedType.I1)]
      public bool BActive;

      public static implicit operator ControllerDigitalActionData_t(
        ControllerDigitalActionData_t.PackSmall d)
      {
        return new ControllerDigitalActionData_t()
        {
          BState = d.BState,
          BActive = d.BActive
        };
      }
    }
  }
}
