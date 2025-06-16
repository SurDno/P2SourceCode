using System.Collections.Generic;

namespace PLVirtualMachine.Common.EngineAPI
{
  public class TypeEnumKeyEqualityComparer : IEqualityComparer<TypeEnumKey>
  {
    public static readonly TypeEnumKeyEqualityComparer Instance = new TypeEnumKeyEqualityComparer();

    public bool Equals(TypeEnumKey x, TypeEnumKey y) => x.Type == y.Type && x.Int == y.Int;

    public int GetHashCode(TypeEnumKey obj) => obj.Type.GetHashCode() ^ obj.Int.GetHashCode();
  }
}
