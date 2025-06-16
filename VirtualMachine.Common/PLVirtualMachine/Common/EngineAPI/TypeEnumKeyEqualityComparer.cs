using System.Collections.Generic;

namespace PLVirtualMachine.Common.EngineAPI;

public class TypeEnumKeyEqualityComparer : IEqualityComparer<TypeEnumKey> {
	public static readonly TypeEnumKeyEqualityComparer Instance = new();

	public bool Equals(TypeEnumKey x, TypeEnumKey y) {
		return x.Type == y.Type && x.Int == y.Int;
	}

	public int GetHashCode(TypeEnumKey obj) {
		return obj.Type.GetHashCode() ^ obj.Int.GetHashCode();
	}
}