using System.Collections.Generic;

public class WeaponEnumComparer : IEqualityComparer<WeaponEnum>
{
  public static WeaponEnumComparer Instance = new();

  public bool Equals(WeaponEnum x, WeaponEnum y) => x == y;

  public int GetHashCode(WeaponEnum obj) => (int) obj;
}
