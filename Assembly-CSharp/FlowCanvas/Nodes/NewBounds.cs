using ParadoxNotion.Design;
using UnityEngine;

namespace FlowCanvas.Nodes
{
  [Category("Utilities/Constructors")]
  public class NewBounds : PureFunctionNode<Bounds, Vector3, Vector3>
  {
    public override Bounds Invoke(Vector3 center, Vector3 size) => new(center, size);
  }
}
