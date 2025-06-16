using System;
using System.Runtime.InteropServices;

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
      return Platform.PackSmall ? (PackSmall) Marshal.PtrToStructure(p, typeof (PackSmall)) : (CallbackMsg_t) Marshal.PtrToStructure(p, typeof (CallbackMsg_t));
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct PackSmall
    {
      public int SteamUser;
      public int Callback;
      public IntPtr ParamPtr;
      public int ParamCount;

      public static implicit operator CallbackMsg_t(PackSmall d)
      {
        return new CallbackMsg_t {
          SteamUser = d.SteamUser,
          Callback = d.Callback,
          ParamPtr = d.ParamPtr,
          ParamCount = d.ParamCount
        };
      }
    }
  }
}
