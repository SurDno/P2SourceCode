using FlowCanvas;
using FlowCanvas.Nodes;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace Engine.Source.Blueprints.Effects
{
  [Category("Effects")]
  public class FocusModeShaderEffectNode : FlowControlNode, IUpdatable
  {
    [Port("Intensity")]
    private ValueInput<float> intensityInput;
    [Port("Color")]
    private ValueInput<Color> colorInput;
    private bool initialized;
    private int propertyId;

    public void Update()
    {
      if (!initialized)
      {
        propertyId = Shader.PropertyToID("_FocusEffectColor");
        initialized = true;
      }
      Shader.SetGlobalColor(propertyId, colorInput.value * intensityInput.value);
    }

    public override void OnDestroy()
    {
      base.OnDestroy();
      if (!initialized)
        return;
      Shader.SetGlobalColor(propertyId, Color.black);
    }
  }
}
