using System.Collections.Generic;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{
  [Category("Functions/Lists")]
  public class GetRandomListItem<T> : PureFunctionNode<T, IList<T>>
  {
    public override T Invoke(IList<T> list)
    {
      return list.Count > 0 ? list[UnityEngine.Random.Range(0, list.Count)] : default (T);
    }
  }
}
