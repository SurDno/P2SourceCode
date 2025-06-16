using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{
  [Category("Functions/Math Operators/Integers")]
  [Name("%")]
  public class IntegerModulo : PureFunctionNode<int, int, int>
  {
    public override int Invoke(int value, int mod) => value % mod;
  }
}
