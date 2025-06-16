using System.Collections.Generic;
using System.Runtime.InteropServices;

public struct Byte2
{
  public byte X;
  public byte Y;

  public Byte2(byte x, byte y)
  {
    X = x;
    Y = y;
  }

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
