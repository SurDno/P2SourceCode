using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{
  [Category("Functions/Math Operators/Integers")]
  [Name("Invert")]
  [Description("Inverts the input ( value = value * -1 )")]
  public class Vector3Invert : PureFunctionNode<Vector3, Vector3>
  {
    public override Vector3 Invoke(Vector3 value) => value * -1f;
  }
}
