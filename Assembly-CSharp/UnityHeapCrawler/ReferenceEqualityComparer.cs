using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace UnityHeapCrawler
{
  public class ReferenceEqualityComparer : IEqualityComparer<object>
  {
    [NotNull]
    public static ReferenceEqualityComparer Instance = new ReferenceEqualityComparer();

    public bool Equals(object x, object y) => x == y;

    public int GetHashCode(object o) => RuntimeHelpers.GetHashCode(o);
  }
}
