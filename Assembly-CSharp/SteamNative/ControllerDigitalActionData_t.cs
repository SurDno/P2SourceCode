using System;
using System.Runtime.InteropServices;

namespace SteamNative;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct ControllerDigitalActionData_t {
	[MarshalAs(UnmanagedType.I1)] public bool BState;
	[MarshalAs(UnmanagedType.I1)] public bool BActive;

	public static ControllerDigitalActionData_t FromPointer(IntPtr p) {
		return Platform.PackSmall
			? (PackSmall)Marshal.PtrToStructure(p, typeof(PackSmall))
			: (ControllerDigitalActionData_t)Marshal.PtrToStructure(p, typeof(ControllerDigitalActionData_t));
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct PackSmall {
		[MarshalAs(UnmanagedType.I1)] public bool BState;
		[MarshalAs(UnmanagedType.I1)] public bool BActive;

		public static implicit operator ControllerDigitalActionData_t(
			PackSmall d) {
			return new ControllerDigitalActionData_t {
				BState = d.BState,
				BActive = d.BActive
			};
		}
	}
}