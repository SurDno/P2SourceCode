using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{
  [Category("Functions/Math Operators/Boolean")]
  public class XOR : PureFunctionNode<bool, bool, bool>
  {
    public override bool Invoke(bool a, bool b) => a | b && a != b;
  }
}
