using Engine.Common;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons.Abilities;
using Engine.Source.Commons.Effects;
using Engine.Source.Components;
using Engine.Source.Effects.Values;
using Inspectors;

namespace Engine.Source.Effects
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class ShootEffect : IEffect
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected ParameterEffectQueueEnum queue = ParameterEffectQueueEnum.None;
    [DataReadProxy(MemberEnum.None, Name = "Action")]
    [DataWriteProxy(MemberEnum.None, Name = "Action")]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected ShootEffectEnum actionType;

    public string Name => this.GetType().Name;

    [Inspected]
    public AbilityItem AbilityItem { get; set; }

    public IEntity Target { get; set; }

    public ParameterEffectQueueEnum Queue => this.queue;

    public bool Prepare(float currentRealTime, float currentGameTime) => true;

    public bool Compute(float currentRealTime, float currentGameTime)
    {
      NpcControllerComponent component = this.Target.GetComponent<NpcControllerComponent>();
      if (component != null)
      {
        PlayerControllerComponent controllerComponent1 = this.AbilityItem.Self.GetComponent<PlayerControllerComponent>() ?? this.AbilityItem.Self.GetComponent<ParentComponent>()?.GetRootParent()?.GetComponent<PlayerControllerComponent>();
        if (controllerComponent1 != null)
        {
          if (this.actionType == ShootEffectEnum.Shoot)
            controllerComponent1.ComputeShoot(component);
          if (this.actionType == ShootEffectEnum.Hit)
            controllerComponent1.ComputeHit(component);
          if (this.actionType == ShootEffectEnum.HitOther)
            controllerComponent1.ComputeHitAnotherNPC(component);
        }
        else
        {
          NpcControllerComponent controllerComponent2 = this.AbilityItem.Self.GetComponent<NpcControllerComponent>() ?? this.AbilityItem.Self.GetComponent<ParentComponent>()?.GetRootParent()?.GetComponent<NpcControllerComponent>();
          if (controllerComponent2 != null)
          {
            if (this.actionType == ShootEffectEnum.Shoot)
              controllerComponent2.ComputeShoot(component);
            if (this.actionType == ShootEffectEnum.Hit)
              controllerComponent2.ComputeHit(component);
          }
        }
      }
      return false;
    }

    public void Cleanup()
    {
    }
  }
}
