using ParadoxNotion.Design;
using UnityEngine;

namespace FlowCanvas.Nodes
{
  [Category("Functions/Math Operators/Vector3")]
  [Name("*")]
  public class Vector3Multiply : PureFunctionNode<Vector3, Vector3, float>
  {
    public override Vector3 Invoke(Vector3 a, float b) => a * b;
  }
}
