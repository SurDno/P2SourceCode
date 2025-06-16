// Decompiled with JetBrains decompiler
// Type: Engine.Source.Commons.Parameters.PriorityItemComprer`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;

#nullable disable
namespace Engine.Source.Commons.Parameters
{
  public class PriorityItemComprer<T> : IComparer<PriorityItem<T>>
  {
    public int Compare(PriorityItem<T> x, PriorityItem<T> y)
    {
      return x.Priority.CompareTo((object) y.Priority);
    }
  }
}
