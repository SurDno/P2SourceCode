using Engine.Common;
using Engine.Common.Components.AttackerPlayer;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Commons.Abilities;
using Engine.Source.Commons.Effects;
using Inspectors;

namespace Engine.Source.Effects
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class NpcStaggerEffect : IEffect
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
    public ShotType punchType;

    public string Name => this.name;

    [Inspected]
    public AbilityItem AbilityItem { get; set; }

    public IEntity Target { get; set; }

    public ParameterEffectQueueEnum Queue => this.queue;

    public void Cleanup()
    {
    }

    public bool Prepare(float currentRealTime, float currentGameTime)
    {
      EnemyBase component = ((IEntityView) this.AbilityItem.Self).GameObject.GetComponent<EnemyBase>();
      ((IEntityView) this.Target).GameObject.GetComponent<EnemyBase>()?.Stagger(component);
      return true;
    }

    public bool Compute(float currentRealTime, float currentGameTime) => false;
  }
}
