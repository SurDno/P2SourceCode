// Decompiled with JetBrains decompiler
// Type: UnityHeapCrawler.ReferenceEqualityComparer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using JetBrains.Annotations;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable disable
namespace UnityHeapCrawler
{
  public class ReferenceEqualityComparer : IEqualityComparer<object>
  {
    [NotNull]
    public static ReferenceEqualityComparer Instance = new ReferenceEqualityComparer();

    public bool Equals(object x, object y) => x == y;

    public int GetHashCode(object o) => RuntimeHelpers.GetHashCode(o);
  }
}
