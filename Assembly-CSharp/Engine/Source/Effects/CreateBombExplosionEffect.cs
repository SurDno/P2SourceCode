using Engine.Common;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Commons.Abilities;
using Engine.Source.Commons.Effects;
using Engine.Source.Components;
using Engine.Source.Connections;
using Inspectors;

namespace Engine.Source.Effects
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class CreateBombExplosionEffect : IEffect, IEntityEventsListener
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected ParameterEffectQueueEnum queue = ParameterEffectQueueEnum.None;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected Typed<IEntity> template;
    private IEntity bombEntity;

    public string Name => this.GetType().Name;

    [Inspected]
    public AbilityItem AbilityItem { get; set; }

    public IEntity Target { get; set; }

    public ParameterEffectQueueEnum Queue => this.queue;

    public bool Prepare(float currentRealTime, float currentGameTime)
    {
      this.bombEntity = ServiceLocator.GetService<IFactory>().Instantiate<IEntity>(this.template.Value);
      ((Entity) this.bombEntity).DontSave = true;
      this.bombEntity.GetComponent<ParentComponent>()?.SetParent(this.Target);
      ServiceLocator.GetService<ISimulation>().Add(this.bombEntity, ServiceLocator.GetService<ISimulation>().Objects);
      LocationItemComponent component1 = this.bombEntity.GetComponent<LocationItemComponent>();
      if (component1 != null)
        component1.Location = this.Target?.GetComponent<LocationItemComponent>()?.Location;
      NPCWeaponService component2 = ((IEntityView) this.Target).GameObject.GetComponent<NPCWeaponService>();
      ((IEntityView) this.bombEntity).Position = component2.ProjectileHitPosition;
      ((IEntityView) this.bombEntity).Rotation = component2.ProjectileHitRotation;
      ((Entity) this.Target).AddListener((IEntityEventsListener) this);
      ((Entity) this.bombEntity).AddListener((IEntityEventsListener) this);
      return true;
    }

    public bool Compute(float currentRealTime, float currentGameTime)
    {
      return !this.bombEntity.IsDisposed;
    }

    public void Cleanup()
    {
    }

    public void OnEntityEvent(IEntity sender, EntityEvents kind)
    {
      if (kind != EntityEvents.DisposeEvent)
        return;
      if (sender == this.Target)
      {
        ((Entity) this.Target).RemoveListener((IEntityEventsListener) this);
        this.bombEntity.Dispose();
      }
      else if (sender == this.bombEntity)
      {
        ((Entity) this.Target).RemoveListener((IEntityEventsListener) this);
        ((Entity) this.bombEntity).RemoveListener((IEntityEventsListener) this);
      }
    }
  }
}
