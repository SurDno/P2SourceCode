// Decompiled with JetBrains decompiler
// Type: SteamNative.ControllerMotionData_t
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Runtime.InteropServices;

#nullable disable
namespace SteamNative
{
  [StructLayout(LayoutKind.Sequential, Pack = 8)]
  internal struct ControllerMotionData_t
  {
    public float RotQuatX;
    public float RotQuatY;
    public float RotQuatZ;
    public float RotQuatW;
    public float PosAccelX;
    public float PosAccelY;
    public float PosAccelZ;
    public float RotVelX;
    public float RotVelY;
    public float RotVelZ;

    public static ControllerMotionData_t FromPointer(IntPtr p)
    {
      return Platform.PackSmall ? (ControllerMotionData_t) (ControllerMotionData_t.PackSmall) Marshal.PtrToStructure(p, typeof (ControllerMotionData_t.PackSmall)) : (ControllerMotionData_t) Marshal.PtrToStructure(p, typeof (ControllerMotionData_t));
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct PackSmall
    {
      public float RotQuatX;
      public float RotQuatY;
      public float RotQuatZ;
      public float RotQuatW;
      public float PosAccelX;
      public float PosAccelY;
      public float PosAccelZ;
      public float RotVelX;
      public float RotVelY;
      public float RotVelZ;

      public static implicit operator ControllerMotionData_t(ControllerMotionData_t.PackSmall d)
      {
        return new ControllerMotionData_t()
        {
          RotQuatX = d.RotQuatX,
          RotQuatY = d.RotQuatY,
          RotQuatZ = d.RotQuatZ,
          RotQuatW = d.RotQuatW,
          PosAccelX = d.PosAccelX,
          PosAccelY = d.PosAccelY,
          PosAccelZ = d.PosAccelZ,
          RotVelX = d.RotVelX,
          RotVelY = d.RotVelY,
          RotVelZ = d.RotVelZ
        };
      }
    }
  }
}
