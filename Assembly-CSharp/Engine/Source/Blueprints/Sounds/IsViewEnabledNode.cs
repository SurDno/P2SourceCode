using Engine.Source.Commons;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints.Sounds
{
  [Category("Sounds")]
  public class IsViewEnabledNode : FlowControlNode
  {
    [Port("Value")]
    private bool Value() => InstanceByRequest<EngineApplication>.Instance.ViewEnabled;
  }
}
