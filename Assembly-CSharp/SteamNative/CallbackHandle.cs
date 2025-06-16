using System;
using System.Runtime.InteropServices;
using Facepunch.Steamworks;

namespace SteamNative;

internal class CallbackHandle : IDisposable {
	internal BaseSteamworks steamworks;
	internal SteamAPICall_t CallResultHandle;
	internal bool CallResult;
	internal GCHandle FuncA;
	internal GCHandle FuncB;
	internal GCHandle FuncC;
	internal IntPtr vTablePtr;
	internal GCHandle PinnedCallback;

	public void Dispose() {
		if (CallResult)
			UnregisterCallResult();
		else
			UnregisterCallback();
		if (FuncA.IsAllocated)
			FuncA.Free();
		if (FuncB.IsAllocated)
			FuncB.Free();
		if (FuncC.IsAllocated)
			FuncC.Free();
		if (PinnedCallback.IsAllocated)
			PinnedCallback.Free();
		if (!(vTablePtr != IntPtr.Zero))
			return;
		Marshal.FreeHGlobal(vTablePtr);
		vTablePtr = IntPtr.Zero;
	}

	private void UnregisterCallback() {
		if (!PinnedCallback.IsAllocated)
			return;
		steamworks.native.api.SteamAPI_UnregisterCallback(PinnedCallback.AddrOfPinnedObject());
	}

	private void UnregisterCallResult() {
		if (CallResultHandle == 0UL || !PinnedCallback.IsAllocated)
			return;
		steamworks.native.api.SteamAPI_UnregisterCallResult(PinnedCallback.AddrOfPinnedObject(), CallResultHandle);
	}
}