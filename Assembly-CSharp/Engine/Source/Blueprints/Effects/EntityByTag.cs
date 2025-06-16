using Engine.Common;
using Engine.Source.Components;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints.Effects
{
  [Category("Engine")]
  public class EntityByTag : FlowControlNode
  {
    [Port("Tag")]
    private ValueInput<string> tagInput;

    [Port("Value")]
    private IEntity Value() => RegisterComponent.GetByTag(tagInput.value);
  }
}
