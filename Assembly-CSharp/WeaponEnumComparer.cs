// Decompiled with JetBrains decompiler
// Type: WeaponEnumComparer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;

#nullable disable
public class WeaponEnumComparer : IEqualityComparer<WeaponEnum>
{
  public static WeaponEnumComparer Instance = new WeaponEnumComparer();

  public bool Equals(WeaponEnum x, WeaponEnum y) => x == y;

  public int GetHashCode(WeaponEnum obj) => (int) obj;
}
