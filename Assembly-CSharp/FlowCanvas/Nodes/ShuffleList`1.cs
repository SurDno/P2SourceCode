using ParadoxNotion.Design;
using System.Collections.Generic;
using UnityEngine;

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
