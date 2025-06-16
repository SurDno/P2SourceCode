using ParadoxNotion.Design;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace FlowCanvas.Nodes
{
  [Category("Functions/Utility")]
  public class GetChildTransforms : PureFunctionNode<Transform[], Transform>
  {
    public override Transform[] Invoke(Transform parent)
    {
      return ((IEnumerable) parent.transform).Cast<Transform>().ToArray<Transform>();
    }
  }
}
