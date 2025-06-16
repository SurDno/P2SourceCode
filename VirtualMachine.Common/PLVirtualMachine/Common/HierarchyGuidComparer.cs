using System.Collections.Generic;

namespace PLVirtualMachine.Common
{
  public class HierarchyGuidComparer : IEqualityComparer<HierarchyGuid>
  {
    public static readonly HierarchyGuidComparer Instance = new HierarchyGuidComparer();

    public bool Equals(HierarchyGuid x, HierarchyGuid y) => x == y;

    public int GetHashCode(HierarchyGuid obj) => obj.GetHashCode();
  }
}
