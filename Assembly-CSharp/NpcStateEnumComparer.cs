using System.Collections.Generic;

public class NpcStateEnumComparer : IEqualityComparer<NpcStateEnum>
{
  public static NpcStateEnumComparer Instance = new();

  public bool Equals(NpcStateEnum x, NpcStateEnum y) => x == y;

  public int GetHashCode(NpcStateEnum obj) => (int) obj;
}
