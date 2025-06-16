using ParadoxNotion.Design;
using System.Collections.Generic;
using System.Linq;

namespace FlowCanvas.Nodes
{
  [Category("Utilities/Converters")]
  public class ToArray<T> : PureFunctionNode<T[], IList<T>>
  {
    public override T[] Invoke(IList<T> list) => list.ToArray<T>();
  }
}
