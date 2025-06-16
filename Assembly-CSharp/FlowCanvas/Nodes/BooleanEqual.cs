using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{
  [Category("Functions/Math Operators/Boolean")]
  [Name("==")]
  public class BooleanEqual : PureFunctionNode<bool, bool, bool>
  {
    public override bool Invoke(bool a, bool b) => a == b;
  }
}
