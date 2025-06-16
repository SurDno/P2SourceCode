// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.ShuffleList`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Design;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace FlowCanvas.Nodes
{
  [Category("Functions/Lists")]
  public class ShuffleList<T> : CallableFunctionNode<IList<T>, IList<T>>
  {
    public override IList<T> Invoke(IList<T> list)
    {
      for (int index1 = list.Count - 1; index1 > 0; --index1)
      {
        int index2 = (int) Mathf.Floor(UnityEngine.Random.value * (float) (index1 + 1));
        T obj = list[index1];
        list[index1] = list[index2];
        list[index2] = obj;
      }
      return list;
    }
  }
}
