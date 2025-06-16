using System.Collections;
using System.Linq;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{
  [Category("Functions/Utility")]
  public class GetChildTransforms : PureFunctionNode<Transform[], Transform>
  {
    public override Transform[] Invoke(Transform parent)
    {
      return ((IEnumerable) parent.transform).Cast<Transform>().ToArray();
    }
  }
}
