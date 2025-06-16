using ParadoxNotion.Design;
using UnityEngine;

namespace FlowCanvas.Nodes
{
  [Category("Utilities/Constructors")]
  public class NewVector2 : PureFunctionNode<Vector2, float, float>
  {
    public override Vector2 Invoke(float x, float y) => new Vector2(x, y);
  }
}
