﻿using Engine.Common;
using Engine.Common.Components.Parameters;
using Engine.Common.Services;
using Engine.Source.Components;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints.Sounds
{
  [Category("Sounds")]
  public class ParameterNode : FlowControlNode
  {
    [FromLocator]
    private ISimulation simulation;
    [Port("Parameter")]
    private ValueInput<ParameterNameEnum> parameterInput;
    [Port("DefaultValue")]
    private ValueInput<float> defaultValue;

    [Port("Value")]
    private float Value()
    {
      IEntity player = simulation.Player;
      if (player == null)
        return defaultValue.value;
      ParametersComponent component = player.GetComponent<ParametersComponent>();
      if (component == null)
        return defaultValue.value;
      IParameter<float> byName1 = component.GetByName<float>(parameterInput.value);
      IParameter<bool> byName2 = component.GetByName<bool>(parameterInput.value);
      if (byName1 == null && byName2 == null)
        return defaultValue.value;
      if (byName1 != null)
        return byName1.Value;
      return byName2 != null ? (byName2.Value ? 1f : 0.0f) : defaultValue.value;
    }
  }
}
