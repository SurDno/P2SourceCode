using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{
  [Category("Functions/Math Operators/Any")]
  [Name("!=")]
  public class AnyNotEqual : PureFunctionNode<bool, object, object>
  {
    public override bool Invoke(object a, object b) => !object.Equals(a, b);
  }
}
