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
    private bool initialized = false;
    private int propertyId;

    public void Update()
    {
      if (!this.initialized)
      {
        this.propertyId = Shader.PropertyToID("_FocusEffectColor");
        this.initialized = true;
      }
      Shader.SetGlobalColor(this.propertyId, this.colorInput.value * this.intensityInput.value);
    }

    public override void OnDestroy()
    {
      base.OnDestroy();
      if (!this.initialized)
        return;
      Shader.SetGlobalColor(this.propertyId, Color.black);
    }
  }
}
