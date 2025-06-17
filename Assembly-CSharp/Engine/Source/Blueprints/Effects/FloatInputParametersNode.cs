using System.Collections.Generic;
using Engine.Common.Services;
using Engine.Source.Services;
using Engine.Source.VisualEffects;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints.Effects
{
  [Category("Effects")]
  public class FloatInputParametersNode : FlowControlNode
  {
    [Port("Name")]
    private ValueInput<string> nameInput;
    [FromLocator]
    private EffectsService effects;
    private List<IParameter<float>> result = [];

    [Port("Value")]
    private IList<IParameter<float>> Value()
    {
      effects.GetParameters(nameInput.value, result);
      return result;
    }
  }
}
