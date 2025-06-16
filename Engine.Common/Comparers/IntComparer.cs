// Decompiled with JetBrains decompiler
// Type: Engine.Common.Comparers.IntComparer
// Assembly: Engine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3568A167-18A7-4983-8BC2-C25824901591
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Engine.Common.dll

using System.Collections.Generic;

#nullable disable
namespace Engine.Common.Comparers
{
  public class IntComparer : IEqualityComparer<int>
  {
    public static readonly IntComparer Instance = new IntComparer();

    public bool Equals(int x, int y) => x == y;

    public int GetHashCode(int obj) => obj;
  }
}
