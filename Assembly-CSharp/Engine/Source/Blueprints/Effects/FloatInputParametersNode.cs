using Engine.Common.Services;
using Engine.Source.Services;
using Engine.Source.VisualEffects;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using System.Collections.Generic;

namespace Engine.Source.Blueprints.Effects
{
  [Category("Effects")]
  public class FloatInputParametersNode : FlowControlNode
  {
    [Port("Name")]
    private ValueInput<string> nameInput;
    [FromLocator]
    private EffectsService effects;
    private List<IParameter<float>> result = new List<IParameter<float>>();

    [Port("Value")]
    private IList<IParameter<float>> Value()
    {
      this.effects.GetParameters<float>(this.nameInput.value, this.result);
      return (IList<IParameter<float>>) this.result;
    }
  }
}
