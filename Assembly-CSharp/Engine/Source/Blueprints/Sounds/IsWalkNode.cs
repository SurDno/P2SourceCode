﻿using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Components;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints.Sounds
{
  [Category("Sounds")]
  public class IsWalkNode : FlowControlNode
  {
    [FromLocator]
    private ISimulation simulation;

    [Port("Value")]
    private bool Value()
    {
      IEntity player = simulation.Player;
      if (player != null)
      {
        ControllerComponent component = player.GetComponent<ControllerComponent>();
        if (component != null)
          return component.IsWalk.Value;
      }
      return false;
    }
  }
}
