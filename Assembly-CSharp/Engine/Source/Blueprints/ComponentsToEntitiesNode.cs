using Engine.Common;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using System.Collections.Generic;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class ComponentsToEntitiesNode : FlowControlNode
  {
    [Port("Components")]
    private ValueInput<IEnumerable<IComponent>> componentsInput;

    [Port("Entities")]
    private IEnumerable<IEntity> Entities()
    {
      IEnumerable<IComponent> components = this.componentsInput.value;
      if (components != null)
      {
        foreach (IComponent component in components)
          yield return component.Owner;
      }
    }
  }
}
