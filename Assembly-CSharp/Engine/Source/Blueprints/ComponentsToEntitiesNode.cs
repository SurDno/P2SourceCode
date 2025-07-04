﻿using System.Collections.Generic;
using Engine.Common;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

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
      IEnumerable<IComponent> components = componentsInput.value;
      if (components != null)
      {
        foreach (IComponent component in components)
          yield return component.Owner;
      }
    }
  }
}
