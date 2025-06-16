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
  public class FloatOutputParameterNode : FlowControlNode, IUpdatable, IParameter<float>, IParameter
  {
    [Port("Value")]
    private ValueInput<float> valueInput;
    [Port("Name")]
    private ValueInput<string> nameInput;
    [FromLocator]
    private EffectsService effects;
    private float prevValue;
    private bool created;
    private bool destroed;

    public float Value => valueInput.value;

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
      float num = valueInput.value;
      if (prevValue == (double) num)
        return;
      prevValue = num;
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
