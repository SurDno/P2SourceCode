using System.Collections.Generic;
using System.Linq;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{
  [Category("Functions/Lists")]
  public class GetFirstListItem<T> : PureFunctionNode<T, IList<T>>
  {
    public override T Invoke(IList<T> list) => list.FirstOrDefault();
  }
}
