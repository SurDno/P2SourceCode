using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
  [StructLayout(LayoutKind.Sequential, Pack = 8)]
  internal struct HTML_StartRequest_t
  {
    public uint UnBrowserHandle;
    public string PchURL;
    public string PchTarget;
    public string PchPostData;
    [MarshalAs(UnmanagedType.I1)]
    public bool BIsRedirect;

    public static HTML_StartRequest_t FromPointer(IntPtr p)
    {
      return Platform.PackSmall ? (PackSmall) Marshal.PtrToStructure(p, typeof (PackSmall)) : (HTML_StartRequest_t) Marshal.PtrToStructure(p, typeof (HTML_StartRequest_t));
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct PackSmall
    {
      public uint UnBrowserHandle;
      public string PchURL;
      public string PchTarget;
      public string PchPostData;
      [MarshalAs(UnmanagedType.I1)]
      public bool BIsRedirect;

      public static implicit operator HTML_StartRequest_t(PackSmall d)
      {
        return new HTML_StartRequest_t {
          UnBrowserHandle = d.UnBrowserHandle,
          PchURL = d.PchURL,
          PchTarget = d.PchTarget,
          PchPostData = d.PchPostData,
          BIsRedirect = d.BIsRedirect
        };
      }
    }
  }
}
