using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{
  [Category("Functions/Math Operators/Floats")]
  [Name("Invert")]
  [Description("Inverts the input ( value = value * -1 )")]
  public class FloatInvert : PureFunctionNode<float, float>
  {
    public override float Invoke(float value) => value * -1f;
  }
}
