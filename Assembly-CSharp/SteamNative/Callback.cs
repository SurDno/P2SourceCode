using System;
using System.Runtime.InteropServices;

namespace SteamNative;

[StructLayout(LayoutKind.Sequential)]
internal class Callback {
	public IntPtr vTablePtr;
	public byte CallbackFlags;
	public int CallbackId;

	internal enum Flags : byte {
		Registered = 1,
		GameServer = 2
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class VTable {
		public IntPtr ResultA;
		public IntPtr ResultB;
		public IntPtr GetSize;
	}

	internal class ThisCall {
		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		public delegate void Result(IntPtr thisptr, IntPtr pvParam);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		public delegate void ResultWithInfo(
			IntPtr thisptr,
			IntPtr pvParam,
			bool bIOFailure,
			SteamAPICall_t hSteamAPICall);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		public delegate int GetSize(IntPtr thisptr);
	}

	internal class StdCall {
		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void Result(IntPtr pvParam);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void ResultWithInfo(
			IntPtr pvParam,
			bool bIOFailure,
			SteamAPICall_t hSteamAPICall);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate int GetSize();
	}
}