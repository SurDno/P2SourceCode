using ParadoxNotion.Design;
using System.Collections.Generic;

namespace FlowCanvas.Nodes
{
  [Category("Functions/Lists")]
  public class RemoveListItemAt<T> : CallableFunctionNode<IList<T>, List<T>, int>
  {
    public override IList<T> Invoke(List<T> list, int index)
    {
      list.RemoveAt(index);
      return (IList<T>) list;
    }
  }
}
