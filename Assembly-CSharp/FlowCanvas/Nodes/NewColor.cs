using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{
  [Category("Utilities/Constructors")]
  public class NewColor : PureFunctionNode<Color, float, float, float, float>
  {
    public override Color Invoke(float r, float g, float b, float a = 1f) => new Color(r, g, b, a);
  }
}
