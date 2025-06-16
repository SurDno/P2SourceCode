using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{
  [Category("Functions/Math Operators/Floats")]
  [Name("-")]
  public class FloatSubtract : PureFunctionNode<float, float, float>
  {
    public override float Invoke(float a, float b) => a - b;
  }
}
