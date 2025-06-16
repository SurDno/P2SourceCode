// Decompiled with JetBrains decompiler
// Type: Engine.Common.Comparers.UlongComparer
// Assembly: Engine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3568A167-18A7-4983-8BC2-C25824901591
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Engine.Common.dll

using System.Collections.Generic;

#nullable disable
namespace Engine.Common.Comparers
{
  public class UlongComparer : IEqualityComparer<ulong>
  {
    public static readonly UlongComparer Instance = new UlongComparer();

    public bool Equals(ulong x, ulong y) => (long) x == (long) y;

    public int GetHashCode(ulong obj) => obj.GetHashCode();
  }
}
