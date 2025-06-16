using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Services;
using Engine.Source.VisualEffects;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints.Effects
{
  [Category("Effects")]
  public class VignetteOutputParameterNode : 
    FlowControlNode,
    IUpdatable,
    IParameter<IntensityParameter<Color>>,
    IParameter
  {
    [Port("Intensity")]
    private ValueInput<float> intensityValueInput;
    [Port("Color")]
    private ValueInput<Color> colorValueInput;
    [Port("Name")]
    private ValueInput<string> nameInput;
    [FromLocator]
    private EffectsService effects;
    private float prevIntensityValue;
    private Color prevColorValue;
    private bool created;
    private bool destroed;
    private float accamulator;

    IntensityParameter<Color> IParameter<IntensityParameter<Color>>.Value
    {
      get
      {
        return new IntensityParameter<Color> {
          Intensity = intensityValueInput.value,
          Value = colorValueInput.value
        };
      }
    }

    public override void OnGraphStarted()
    {
      base.OnGraphStarted();
      InstanceByRequest<UpdateService>.Instance.BlueprintEffectsUpdater.AddUpdatable(this);
    }

    public override void OnGraphStoped()
    {
      InstanceByRequest<UpdateService>.Instance.BlueprintEffectsUpdater.RemoveUpdatable(this);
      base.OnGraphStoped();
    }

    public void ComputeUpdate()
    {
      float num = intensityValueInput.value;
      Color color = colorValueInput.value;
      if (prevIntensityValue == (double) num && !(prevColorValue != color))
        return;
      prevIntensityValue = num;
      prevColorValue = color;
      if (num != 0.0)
        CreateEffect();
      else
        DestroyEffect();
    }

    public override void OnDestroy()
    {
      destroed = true;
      DestroyEffect();
      base.OnDestroy();
    }

    protected void CreateEffect()
    {
      if (destroed || created)
        return;
      created = true;
      effects.AddParameter(nameInput.value, this);
    }

    protected void DestroyEffect()
    {
      if (!created)
        return;
      created = false;
      effects.RemoveParameter(nameInput.value, this);
    }
  }
}
