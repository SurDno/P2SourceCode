using ParadoxNotion.Design;
using UnityEngine;

namespace FlowCanvas.Nodes
{
  [Category("Functions/Math Operators/Vector3")]
  [Name("-")]
  public class Vector3Subtract : PureFunctionNode<Vector3, Vector3, Vector3>
  {
    public override Vector3 Invoke(Vector3 a, Vector3 b) => a - b;
  }
}
