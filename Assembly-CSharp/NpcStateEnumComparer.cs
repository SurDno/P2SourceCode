// Decompiled with JetBrains decompiler
// Type: NpcStateEnumComparer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;

#nullable disable
public class NpcStateEnumComparer : IEqualityComparer<NpcStateEnum>
{
  public static NpcStateEnumComparer Instance = new NpcStateEnumComparer();

  public bool Equals(NpcStateEnum x, NpcStateEnum y) => x == y;

  public int GetHashCode(NpcStateEnum obj) => (int) obj;
}
