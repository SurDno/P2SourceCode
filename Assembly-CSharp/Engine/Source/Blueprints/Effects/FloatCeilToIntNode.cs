using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints.Effects
{
  [Category("Engine")]
  public class FloatCeilToIntNode : PureFunctionNode<int, float>
  {
    public override int Invoke(float a) => Mathf.CeilToInt(a);
  }
}
