// Decompiled with JetBrains decompiler
// Type: SteamNative.CallbackHandle
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Facepunch.Steamworks;
using System;
using System.Runtime.InteropServices;

#nullable disable
namespace SteamNative
{
  internal class CallbackHandle : IDisposable
  {
    internal BaseSteamworks steamworks;
    internal SteamAPICall_t CallResultHandle;
    internal bool CallResult;
    internal GCHandle FuncA;
    internal GCHandle FuncB;
    internal GCHandle FuncC;
    internal IntPtr vTablePtr;
    internal GCHandle PinnedCallback;

    public void Dispose()
    {
      if (this.CallResult)
        this.UnregisterCallResult();
      else
        this.UnregisterCallback();
      if (this.FuncA.IsAllocated)
        this.FuncA.Free();
      if (this.FuncB.IsAllocated)
        this.FuncB.Free();
      if (this.FuncC.IsAllocated)
        this.FuncC.Free();
      if (this.PinnedCallback.IsAllocated)
        this.PinnedCallback.Free();
      if (!(this.vTablePtr != IntPtr.Zero))
        return;
      Marshal.FreeHGlobal(this.vTablePtr);
      this.vTablePtr = IntPtr.Zero;
    }

    private void UnregisterCallback()
    {
      if (!this.PinnedCallback.IsAllocated)
        return;
      this.steamworks.native.api.SteamAPI_UnregisterCallback(this.PinnedCallback.AddrOfPinnedObject());
    }

    private void UnregisterCallResult()
    {
      if ((ulong) this.CallResultHandle == 0UL || !this.PinnedCallback.IsAllocated)
        return;
      this.steamworks.native.api.SteamAPI_UnregisterCallResult(this.PinnedCallback.AddrOfPinnedObject(), this.CallResultHandle);
    }
  }
}
