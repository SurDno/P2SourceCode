using System;
using System.Runtime.InteropServices;

namespace SteamNative;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct HTML_CloseBrowser_t {
	public uint UnBrowserHandle;

	public static HTML_CloseBrowser_t FromPointer(IntPtr p) {
		return Platform.PackSmall
			? (PackSmall)Marshal.PtrToStructure(p, typeof(PackSmall))
			: (HTML_CloseBrowser_t)Marshal.PtrToStructure(p, typeof(HTML_CloseBrowser_t));
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct PackSmall {
		public uint UnBrowserHandle;

		public static implicit operator HTML_CloseBrowser_t(PackSmall d) {
			return new HTML_CloseBrowser_t {
				UnBrowserHandle = d.UnBrowserHandle
			};
		}
	}
}