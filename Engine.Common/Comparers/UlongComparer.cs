using System.Collections.Generic;

namespace Engine.Common.Comparers
{
  public class UlongComparer : IEqualityComparer<ulong>
  {
    public static readonly UlongComparer Instance = new();

    public bool Equals(ulong x, ulong y) => (long) x == (long) y;

    public int GetHashCode(ulong obj) => obj.GetHashCode();
  }
}
