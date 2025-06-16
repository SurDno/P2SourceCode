// Decompiled with JetBrains decompiler
// Type: ClipperLib.MyIntersectNodeSort
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;

#nullable disable
namespace ClipperLib
{
  public class MyIntersectNodeSort : IComparer<IntersectNode>
  {
    public int Compare(IntersectNode node1, IntersectNode node2)
    {
      long num = node2.Pt.Y - node1.Pt.Y;
      if (num > 0L)
        return 1;
      return num < 0L ? -1 : 0;
    }
  }
}
