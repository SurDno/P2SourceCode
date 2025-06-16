using Cofe.Meta;
using Engine.Common;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons.Effects;
using Engine.Source.Components;
using Inspectors;
using System.Collections.Generic;

namespace Engine.Source.Commons.Abilities
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class AbilityItem
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Header = true, Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    protected string name = "";
    [DataReadProxy(MemberEnum.None, Name = "AbilityController")]
    [DataWriteProxy(MemberEnum.None, Name = "AbilityController")]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected IAbilityController controller;
    [DataReadProxy(MemberEnum.None, Name = "AbilityProjectile")]
    [DataWriteProxy(MemberEnum.None, Name = "AbilityProjectile")]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected IAbilityProjectile projectile;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected AbilityTargetEnum targets;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected List<IEffect> effects = new List<IEffect>();
    [Inspected]
    private HashSet<IEffect> dependEffects = new HashSet<IEffect>();
    private bool active;
    private IEntity item;
    private IEntity self;

    public IEntity Item => this.item;

    public IEntity Self => this.self;

    public Ability Ability { get; private set; }

    [Inspected(Mutable = true)]
    public bool Active
    {
      get => this.active;
      set
      {
        if (this.active == value)
          return;
        if (value)
          this.Start();
        else
          this.Stop();
      }
    }

    [Inspected]
    public IAbilityController AbilityController => this.controller;

    [Inspected]
    public IAbilityProjectile Projectile => this.projectile;

    [Inspected]
    public AbilityTargetEnum Targets => this.targets;

    [Inspected]
    public List<IEffect> Effects => this.effects;

    public void AddDependEffect(IEffect effect) => this.dependEffects.Add(effect);

    public void AddEffects()
    {
      StorableComponent component = this.Ability.Owner.GetComponent<StorableComponent>();
      if (component != null)
      {
        this.item = this.Ability.Owner;
        this.self = component.Storage.Owner;
      }
      else
      {
        this.item = (IEntity) null;
        this.self = this.Ability.Owner;
      }
      OutsideAbilityTargets targets = new OutsideAbilityTargets();
      if (this.projectile != null)
        this.projectile.ComputeTargets(this.Self, this.item, targets);
      if ((this.targets & AbilityTargetEnum.Item) != (AbilityTargetEnum) 0 && this.CheckTargetPlayer(this.Item?.GetComponent<EffectsComponent>()))
        this.AddEffects(this.Item?.GetComponent<EffectsComponent>());
      if ((this.targets & AbilityTargetEnum.Self) != (AbilityTargetEnum) 0 && this.CheckTargetPlayer(this.Self?.GetComponent<EffectsComponent>()))
        this.AddEffects(this.Self?.GetComponent<EffectsComponent>());
      if ((this.targets & AbilityTargetEnum.Target) == 0)
        return;
      foreach (EffectsComponent target in targets.Targets)
      {
        if (this.CheckTargetPlayer(target))
          this.AddEffects(target);
      }
    }

    private bool CheckTargetPlayer(EffectsComponent target)
    {
      if (target == null || this.targets == AbilityTargetEnum.Item || this.targets == AbilityTargetEnum.Self || this.targets == AbilityTargetEnum.Target)
        return true;
      IEntity owner = target.Owner;
      if ((this.targets & AbilityTargetEnum.Item) != 0)
        owner = this.item?.GetComponent<StorableComponent>()?.Storage?.Owner;
      if (owner == null)
        return true;
      PlayerControllerComponent component = owner.GetComponent<PlayerControllerComponent>();
      return (this.targets == AbilityTargetEnum.ItemPlayer || this.targets == AbilityTargetEnum.SelfPlayer || this.targets == AbilityTargetEnum.TargetPlayer) == (component != null);
    }

    private void AddEffects(EffectsComponent target)
    {
      if (target == null)
        return;
      foreach (IEffect effect1 in this.effects)
      {
        IEffect effect2 = CloneableObjectUtility.Clone<IEffect>(effect1);
        effect2.AbilityItem = this;
        effect2.Target = target.Owner;
        target.AddEffect(effect2);
      }
    }

    public void Initialise(Ability ability)
    {
      this.Ability = ability;
      if (this.AbilityController == null)
        return;
      this.AbilityController.Initialise(this);
    }

    public void Shutdown()
    {
      if (this.AbilityController == null)
        return;
      this.AbilityController.Shutdown();
    }

    public void RemoveDependEffects()
    {
      foreach (IEffect dependEffect in this.dependEffects)
        dependEffect.Target.GetComponent<EffectsComponent>().RemoveEffect(dependEffect);
      this.dependEffects.Clear();
    }

    private void Start()
    {
      if (this.active)
        return;
      this.active = true;
      this.AddEffects();
    }

    private void Stop()
    {
      if (!this.active)
        return;
      this.active = false;
      this.RemoveDependEffects();
    }

    [global::ComputeBytes]
    private void ComputeBytes()
    {
      foreach (object effect in this.effects)
        MetaService.Compute(effect, ComputeBytesAttribute.Id);
    }
  }
}
