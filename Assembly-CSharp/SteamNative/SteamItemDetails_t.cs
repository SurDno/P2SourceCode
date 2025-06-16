using System;
using System.Runtime.InteropServices;

namespace SteamNative;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct SteamItemDetails_t {
	public ulong ItemId;
	public int Definition;
	public ushort Quantity;
	public ushort Flags;

	public static SteamItemDetails_t FromPointer(IntPtr p) {
		return Platform.PackSmall
			? (PackSmall)Marshal.PtrToStructure(p, typeof(PackSmall))
			: (SteamItemDetails_t)Marshal.PtrToStructure(p, typeof(SteamItemDetails_t));
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct PackSmall {
		public ulong ItemId;
		public int Definition;
		public ushort Quantity;
		public ushort Flags;

		public static implicit operator SteamItemDetails_t(PackSmall d) {
			return new SteamItemDetails_t {
				ItemId = d.ItemId,
				Definition = d.Definition,
				Quantity = d.Quantity,
				Flags = d.Flags
			};
		}
	}
}