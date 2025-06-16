using Engine.Common;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Commons.Abilities;
using Engine.Source.Commons.Abilities.Controllers;
using Engine.Source.Commons.Effects;
using Engine.Source.Components;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Effects
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class NpcKnockDownEffect : IEffect
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Header = true, Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    protected string name = "";
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected ParameterEffectQueueEnum queue = ParameterEffectQueueEnum.None;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Header = true, Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    protected float holdTime = 5f;
    private IParameter<bool> movementBlockParameter;
    private float startTime;

    public string Name => this.name;

    [Inspected]
    public AbilityItem AbilityItem { get; set; }

    public IEntity Target { get; set; }

    public ParameterEffectQueueEnum Queue => this.queue;

    public void Cleanup()
    {
      if (this.movementBlockParameter == null)
        return;
      this.movementBlockParameter.Value = false;
    }

    public bool Prepare(float currentRealTime, float currentGameTime)
    {
      EnemyBase component1 = ((IEntityView) this.AbilityItem.Self).GameObject.GetComponent<EnemyBase>();
      EnemyBase component2 = ((IEntityView) this.Target).GameObject.GetComponent<EnemyBase>();
      if (!(this.AbilityItem.AbilityController is CloseCombatAbilityController))
      {
        Debug.LogError((object) (typeof (NpcKnockDownEffect).Name + " requires " + typeof (CloseCombatAbilityController).Name));
        return false;
      }
      component2?.KnockDown(component1);
      this.startTime = currentRealTime;
      ParametersComponent component3 = this.Target.GetComponent<ParametersComponent>();
      if (component3 != null)
      {
        this.movementBlockParameter = component3.GetByName<bool>(ParameterNameEnum.MovementControlBlock);
        if (this.movementBlockParameter != null)
          this.movementBlockParameter.Value = true;
      }
      return true;
    }

    public bool Compute(float currentRealTime, float currentGameTime)
    {
      return this.movementBlockParameter != null && (double) currentRealTime - (double) this.startTime < (double) this.holdTime;
    }
  }
}
