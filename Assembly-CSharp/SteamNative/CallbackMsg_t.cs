// Decompiled with JetBrains decompiler
// Type: SteamNative.CallbackMsg_t
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Runtime.InteropServices;

#nullable disable
namespace SteamNative
{
  [StructLayout(LayoutKind.Sequential, Pack = 8)]
  internal struct CallbackMsg_t
  {
    public int SteamUser;
    public int Callback;
    public IntPtr ParamPtr;
    public int ParamCount;

    public static CallbackMsg_t FromPointer(IntPtr p)
    {
      return Platform.PackSmall ? (CallbackMsg_t) (CallbackMsg_t.PackSmall) Marshal.PtrToStructure(p, typeof (CallbackMsg_t.PackSmall)) : (CallbackMsg_t) Marshal.PtrToStructure(p, typeof (CallbackMsg_t));
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct PackSmall
    {
      public int SteamUser;
      public int Callback;
      public IntPtr ParamPtr;
      public int ParamCount;

      public static implicit operator CallbackMsg_t(CallbackMsg_t.PackSmall d)
      {
        return new CallbackMsg_t()
        {
          SteamUser = d.SteamUser,
          Callback = d.Callback,
          ParamPtr = d.ParamPtr,
          ParamCount = d.ParamCount
        };
      }
    }
  }
}
