using ParadoxNotion.Design;
using System.Collections;

namespace FlowCanvas.Nodes
{
  [Category("Functions/Lists")]
  public class GetListItemIndex : PureFunctionNode<int, IList, object>
  {
    public override int Invoke(IList list, object item) => list.IndexOf(item);
  }
}
