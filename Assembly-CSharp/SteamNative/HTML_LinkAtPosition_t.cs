using System;
using System.Runtime.InteropServices;
using Facepunch.Steamworks;

namespace SteamNative;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct HTML_LinkAtPosition_t {
	public const int CallbackId = 4513;
	public uint UnBrowserHandle;
	public uint X;
	public uint Y;
	public string PchURL;
	[MarshalAs(UnmanagedType.I1)] public bool BInput;
	[MarshalAs(UnmanagedType.I1)] public bool BLiveLink;

	public static HTML_LinkAtPosition_t FromPointer(IntPtr p) {
		return Platform.PackSmall
			? (PackSmall)Marshal.PtrToStructure(p, typeof(PackSmall))
			: (HTML_LinkAtPosition_t)Marshal.PtrToStructure(p, typeof(HTML_LinkAtPosition_t));
	}

	public static void RegisterCallback(
		BaseSteamworks steamworks,
		Action<HTML_LinkAtPosition_t, bool> CallbackFunction) {
		var handle = new CallbackHandle();
		handle.steamworks = steamworks;
		if (Config.UseThisCall) {
			Callback.ThisCall.Result d1 = (_, p) => CallbackFunction(FromPointer(p), false);
			Callback.ThisCall.ResultWithInfo d2 = (_, p, bIOFailure, hSteamAPICall) =>
				CallbackFunction(FromPointer(p), bIOFailure);
			Callback.ThisCall.GetSize d3 = _ => Marshal.SizeOf(typeof(HTML_LinkAtPosition_t));
			if (Platform.PackSmall)
				d3 = _ => Marshal.SizeOf(typeof(PackSmall));
			handle.FuncA = GCHandle.Alloc(d1);
			handle.FuncB = GCHandle.Alloc(d2);
			handle.FuncC = GCHandle.Alloc(d3);
			handle.vTablePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Callback.VTable)));
			var structure = new Callback.VTable {
				ResultA = Marshal.GetFunctionPointerForDelegate(d1),
				ResultB = Marshal.GetFunctionPointerForDelegate(d2),
				GetSize = Marshal.GetFunctionPointerForDelegate(d3)
			};
			if (Platform.IsWindows) {
				structure.ResultA = Marshal.GetFunctionPointerForDelegate(d2);
				structure.ResultB = Marshal.GetFunctionPointerForDelegate(d1);
			}

			Marshal.StructureToPtr(structure, handle.vTablePtr, false);
		} else {
			Callback.StdCall.Result d4 = p => CallbackFunction(FromPointer(p), false);
			Callback.StdCall.ResultWithInfo d5 = (p, bIOFailure, hSteamAPICall) =>
				CallbackFunction(FromPointer(p), bIOFailure);
			Callback.StdCall.GetSize d6 = () => Marshal.SizeOf(typeof(HTML_LinkAtPosition_t));
			if (Platform.PackSmall)
				d6 = () => Marshal.SizeOf(typeof(PackSmall));
			handle.FuncA = GCHandle.Alloc(d4);
			handle.FuncB = GCHandle.Alloc(d5);
			handle.FuncC = GCHandle.Alloc(d6);
			handle.vTablePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Callback.VTable)));
			var structure = new Callback.VTable {
				ResultA = Marshal.GetFunctionPointerForDelegate(d4),
				ResultB = Marshal.GetFunctionPointerForDelegate(d5),
				GetSize = Marshal.GetFunctionPointerForDelegate(d6)
			};
			if (Platform.IsWindows) {
				structure.ResultA = Marshal.GetFunctionPointerForDelegate(d5);
				structure.ResultB = Marshal.GetFunctionPointerForDelegate(d4);
			}

			Marshal.StructureToPtr(structure, handle.vTablePtr, false);
		}

		handle.PinnedCallback = GCHandle.Alloc(new Callback {
			vTablePtr = handle.vTablePtr,
			CallbackFlags = steamworks.IsGameServer ? (byte)2 : (byte)0,
			CallbackId = 4513
		}, GCHandleType.Pinned);
		steamworks.native.api.SteamAPI_RegisterCallback(handle.PinnedCallback.AddrOfPinnedObject(), 4513);
		steamworks.RegisterCallbackHandle(handle);
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct PackSmall {
		public uint UnBrowserHandle;
		public uint X;
		public uint Y;
		public string PchURL;
		[MarshalAs(UnmanagedType.I1)] public bool BInput;
		[MarshalAs(UnmanagedType.I1)] public bool BLiveLink;

		public static implicit operator HTML_LinkAtPosition_t(PackSmall d) {
			return new HTML_LinkAtPosition_t {
				UnBrowserHandle = d.UnBrowserHandle,
				X = d.X,
				Y = d.Y,
				PchURL = d.PchURL,
				BInput = d.BInput,
				BLiveLink = d.BLiveLink
			};
		}
	}
}