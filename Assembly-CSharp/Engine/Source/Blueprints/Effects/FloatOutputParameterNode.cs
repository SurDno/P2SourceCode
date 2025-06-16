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

    public float Value => this.valueInput.value;

    public override void OnGraphStarted()
    {
      base.OnGraphStarted();
      InstanceByRequest<UpdateService>.Instance.BlueprintEffectsUpdater.AddUpdatable((IUpdatable) this);
    }

    public override void OnGraphStoped()
    {
      InstanceByRequest<UpdateService>.Instance.BlueprintEffectsUpdater.RemoveUpdatable((IUpdatable) this);
      base.OnGraphStoped();
    }

    public void ComputeUpdate()
    {
      float num = this.valueInput.value;
      if ((double) this.prevValue == (double) num)
        return;
      this.prevValue = num;
      if ((double) num != 0.0)
        this.CreateEffect();
      else
        this.DestroyEffect();
    }

    public override void OnDestroy()
    {
      this.destroed = true;
      this.DestroyEffect();
      base.OnDestroy();
    }

    protected void CreateEffect()
    {
      if (this.destroed || this.created)
        return;
      this.created = true;
      this.effects.AddParameter(this.nameInput.value, (IParameter) this);
    }

    protected void DestroyEffect()
    {
      if (!this.created)
        return;
      this.created = false;
      this.effects.RemoveParameter(this.nameInput.value, (IParameter) this);
    }
  }
}
