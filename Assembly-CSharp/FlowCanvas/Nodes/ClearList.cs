using ParadoxNotion.Design;
using System.Collections;

namespace FlowCanvas.Nodes
{
  [Category("Functions/Lists")]
  public class ClearList : CallableFunctionNode<IList, IList>
  {
    public override IList Invoke(IList list)
    {
      list.Clear();
      return list;
    }
  }
}
