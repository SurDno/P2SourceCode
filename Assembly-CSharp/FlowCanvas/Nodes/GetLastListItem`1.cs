using ParadoxNotion.Design;
using System.Collections.Generic;
using System.Linq;

namespace FlowCanvas.Nodes
{
  [Category("Functions/Lists")]
  public class GetLastListItem<T> : PureFunctionNode<T, IList<T>>
  {
    public override T Invoke(IList<T> list) => list.LastOrDefault<T>();
  }
}
