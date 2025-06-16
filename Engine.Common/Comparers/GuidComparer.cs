using System;
using System.Collections.Generic;

namespace Engine.Common.Comparers
{
  public class GuidComparer : IEqualityComparer<Guid>
  {
    public static readonly GuidComparer Instance = new GuidComparer();

    public bool Equals(Guid x, Guid y) => x == y;

    public int GetHashCode(Guid obj) => obj.GetHashCode();
  }
}
