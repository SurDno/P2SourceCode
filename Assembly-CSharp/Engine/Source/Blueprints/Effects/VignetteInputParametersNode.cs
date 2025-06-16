using Engine.Common.Services;
using Engine.Source.Services;
using Engine.Source.VisualEffects;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using System.Collections.Generic;
using UnityEngine;

namespace Engine.Source.Blueprints.Effects
{
  [Category("Effects")]
  public class VignetteInputParametersNode : FlowControlNode
  {
    [Port("Name")]
    private ValueInput<string> nameInput;
    [FromLocator]
    private EffectsService effects;
    private List<IParameter<IntensityParameter<Color>>> result = new List<IParameter<IntensityParameter<Color>>>();

    [Port("Value")]
    private IList<IParameter<IntensityParameter<Color>>> Value()
    {
      this.effects.GetParameters<IntensityParameter<Color>>(this.nameInput.value, this.result);
      return (IList<IParameter<IntensityParameter<Color>>>) this.result;
    }
  }
}
