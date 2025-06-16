using System.Collections.Generic;

public class NpcStateEnumComparer : IEqualityComparer<NpcStateEnum> {
	public static NpcStateEnumComparer Instance = new();

	public bool Equals(NpcStateEnum x, NpcStateEnum y) {
		return x == y;
	}

	public int GetHashCode(NpcStateEnum obj) {
		return (int)obj;
	}
}