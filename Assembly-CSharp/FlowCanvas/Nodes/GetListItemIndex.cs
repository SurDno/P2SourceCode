using System.Collections;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{
  [Category("Functions/Lists")]
  public class GetListItemIndex : PureFunctionNode<int, IList, object>
  {
    public override int Invoke(IList list, object item) => list.IndexOf(item);
  }
}
