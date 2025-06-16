using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{
  [Category("Functions/Math Operators/Floats")]
  [Name("<")]
  public class FloatLessThan : PureFunctionNode<bool, float, float>
  {
    public override bool Invoke(float a, float b) => (double) a < (double) b;
  }
}
