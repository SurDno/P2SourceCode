using System.Collections.Generic;
using System.Runtime.InteropServices;

public struct Byte2(byte x, byte y) {
  public byte X = x;
  public byte Y = y;

  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct XComparer : IComparer<Byte2>
  {
    public int Compare(Byte2 x, Byte2 y)
    {
      return x.X == y.X ? x.Y - y.Y : x.X - y.X;
    }
  }

  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct YComparer : IComparer<Byte2>
  {
    public int Compare(Byte2 x, Byte2 y)
    {
      return x.Y == y.Y ? x.X - y.X : x.Y - y.Y;
    }
  }
}
