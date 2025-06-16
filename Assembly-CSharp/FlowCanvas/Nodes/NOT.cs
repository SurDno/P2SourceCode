using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{
  [Category("Functions/Math Operators/Boolean")]
  public class NOT : PureFunctionNode<bool, bool>
  {
    public override bool Invoke(bool value) => !value;
  }
}
