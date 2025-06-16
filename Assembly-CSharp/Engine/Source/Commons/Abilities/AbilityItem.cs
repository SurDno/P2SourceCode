using System.Collections.Generic;
using Cofe.Meta;
using Engine.Common;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons.Effects;
using Engine.Source.Components;
using Inspectors;

namespace Engine.Source.Commons.Abilities
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class AbilityItem
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected(Header = true, Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    protected string name = "";
    [DataReadProxy(Name = "AbilityController")]
    [DataWriteProxy(Name = "AbilityController")]
    [CopyableProxy]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected IAbilityController controller;
    [DataReadProxy(Name = "AbilityProjectile")]
    [DataWriteProxy(Name = "AbilityProjectile")]
    [CopyableProxy]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected IAbilityProjectile projectile;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected AbilityTargetEnum targets;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected List<IEffect> effects = new List<IEffect>();
    [Inspected]
    private HashSet<IEffect> dependEffects = new HashSet<IEffect>();
    private bool active;
    private IEntity item;
    private IEntity self;

    public IEntity Item => item;

    public IEntity Self => self;

    public Ability Ability { get; private set; }

    [Inspected(Mutable = true)]
    public bool Active
    {
      get => active;
      set
      {
        if (active == value)
          return;
        if (value)
          Start();
        else
          Stop();
      }
    }

    [Inspected]
    public IAbilityController AbilityController => controller;

    [Inspected]
    public IAbilityProjectile Projectile => projectile;

    [Inspected]
    public AbilityTargetEnum Targets => targets;

    [Inspected]
    public List<IEffect> Effects => effects;

    public void AddDependEffect(IEffect effect) => dependEffects.Add(effect);

    public void AddEffects()
    {
      StorableComponent component = Ability.Owner.GetComponent<StorableComponent>();
      if (component != null)
      {
        item = Ability.Owner;
        self = component.Storage.Owner;
      }
      else
      {
        item = null;
        self = Ability.Owner;
      }
      OutsideAbilityTargets targets = new OutsideAbilityTargets();
      if (projectile != null)
        projectile.ComputeTargets(Self, item, targets);
      if ((this.targets & AbilityTargetEnum.Item) != 0 && CheckTargetPlayer(Item?.GetComponent<EffectsComponent>()))
        AddEffects(Item?.GetComponent<EffectsComponent>());
      if ((this.targets & AbilityTargetEnum.Self) != 0 && CheckTargetPlayer(Self?.GetComponent<EffectsComponent>()))
        AddEffects(Self?.GetComponent<EffectsComponent>());
      if ((this.targets & AbilityTargetEnum.Target) == 0)
        return;
      foreach (EffectsComponent target in targets.Targets)
      {
        if (CheckTargetPlayer(target))
          AddEffects(target);
      }
    }

    private bool CheckTargetPlayer(EffectsComponent target)
    {
      if (target == null || targets == AbilityTargetEnum.Item || targets == AbilityTargetEnum.Self || targets == AbilityTargetEnum.Target)
        return true;
      IEntity owner = target.Owner;
      if ((targets & AbilityTargetEnum.Item) != 0)
        owner = item?.GetComponent<StorableComponent>()?.Storage?.Owner;
      if (owner == null)
        return true;
      PlayerControllerComponent component = owner.GetComponent<PlayerControllerComponent>();
      return (targets == AbilityTargetEnum.ItemPlayer || targets == AbilityTargetEnum.SelfPlayer || targets == AbilityTargetEnum.TargetPlayer) == (component != null);
    }

    private void AddEffects(EffectsComponent target)
    {
      if (target == null)
        return;
      foreach (IEffect effect1 in effects)
      {
        IEffect effect2 = CloneableObjectUtility.Clone(effect1);
        effect2.AbilityItem = this;
        effect2.Target = target.Owner;
        target.AddEffect(effect2);
      }
    }

    public void Initialise(Ability ability)
    {
      Ability = ability;
      if (AbilityController == null)
        return;
      AbilityController.Initialise(this);
    }

    public void Shutdown()
    {
      if (AbilityController == null)
        return;
      AbilityController.Shutdown();
    }

    public void RemoveDependEffects()
    {
      foreach (IEffect dependEffect in dependEffects)
        dependEffect.Target.GetComponent<EffectsComponent>().RemoveEffect(dependEffect);
      dependEffects.Clear();
    }

    private void Start()
    {
      if (active)
        return;
      active = true;
      AddEffects();
    }

    private void Stop()
    {
      if (!active)
        return;
      active = false;
      RemoveDependEffects();
    }

    [ComputeBytes]
    private void ComputeBytes()
    {
      foreach (object effect in effects)
        MetaService.Compute(effect, ComputeBytesAttribute.Id);
    }
  }
}
