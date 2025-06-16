using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{
  [Category("Functions/Math Operators/Integers")]
  [Name("!=")]
  public class IntegerNotEqual : PureFunctionNode<bool, int, int>
  {
    public override bool Invoke(int a, int b) => a != b;
  }
}
