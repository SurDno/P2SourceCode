using ParadoxNotion.Design;
using System.Collections.Generic;

namespace FlowCanvas.Nodes
{
  [Category("Functions/Lists")]
  public class InsertListItem<T> : CallableFunctionNode<IList<T>, List<T>, int, T>
  {
    public override IList<T> Invoke(List<T> list, int index, T item)
    {
      list.Insert(index, item);
      return (IList<T>) list;
    }
  }
}
