using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
  [StructLayout(LayoutKind.Sequential, Pack = 8)]
  internal struct HTML_NeedsPaint_t
  {
    public uint UnBrowserHandle;
    public string PBGRA;
    public uint UnWide;
    public uint UnTall;
    public uint UnUpdateX;
    public uint UnUpdateY;
    public uint UnUpdateWide;
    public uint UnUpdateTall;
    public uint UnScrollX;
    public uint UnScrollY;
    public float FlPageScale;
    public uint UnPageSerial;

    public static HTML_NeedsPaint_t FromPointer(IntPtr p)
    {
      return Platform.PackSmall ? (HTML_NeedsPaint_t) (HTML_NeedsPaint_t.PackSmall) Marshal.PtrToStructure(p, typeof (HTML_NeedsPaint_t.PackSmall)) : (HTML_NeedsPaint_t) Marshal.PtrToStructure(p, typeof (HTML_NeedsPaint_t));
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct PackSmall
    {
      public uint UnBrowserHandle;
      public string PBGRA;
      public uint UnWide;
      public uint UnTall;
      public uint UnUpdateX;
      public uint UnUpdateY;
      public uint UnUpdateWide;
      public uint UnUpdateTall;
      public uint UnScrollX;
      public uint UnScrollY;
      public float FlPageScale;
      public uint UnPageSerial;

      public static implicit operator HTML_NeedsPaint_t(HTML_NeedsPaint_t.PackSmall d)
      {
        return new HTML_NeedsPaint_t()
        {
          UnBrowserHandle = d.UnBrowserHandle,
          PBGRA = d.PBGRA,
          UnWide = d.UnWide,
          UnTall = d.UnTall,
          UnUpdateX = d.UnUpdateX,
          UnUpdateY = d.UnUpdateY,
          UnUpdateWide = d.UnUpdateWide,
          UnUpdateTall = d.UnUpdateTall,
          UnScrollX = d.UnScrollX,
          UnScrollY = d.UnScrollY,
          FlPageScale = d.FlPageScale,
          UnPageSerial = d.UnPageSerial
        };
      }
    }
  }
}
