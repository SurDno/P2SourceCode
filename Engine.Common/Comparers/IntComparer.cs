using System.Collections.Generic;

namespace Engine.Common.Comparers
{
  public class IntComparer : IEqualityComparer<int>
  {
    public static readonly IntComparer Instance = new();

    public bool Equals(int x, int y) => x == y;

    public int GetHashCode(int obj) => obj;
  }
}
