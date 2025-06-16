using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{
  [Category("Utilities/Constructors")]
  public class NewVector4 : PureFunctionNode<Vector4, float, float, float, float>
  {
    public override Vector4 Invoke(float x, float y, float z, float w) => new Vector4(x, y, z, w);
  }
}
