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
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected ParameterEffectQueueEnum queue = ParameterEffectQueueEnum.None;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected Typed<IEntity> template;
    private IEntity bombEntity;

    public string Name => GetType().Name;

    [Inspected]
    public AbilityItem AbilityItem { get; set; }

    public IEntity Target { get; set; }

    public ParameterEffectQueueEnum Queue => queue;

    public bool Prepare(float currentRealTime, float currentGameTime)
    {
      bombEntity = ServiceLocator.GetService<IFactory>().Instantiate(template.Value);
      ((Entity) bombEntity).DontSave = true;
      bombEntity.GetComponent<ParentComponent>()?.SetParent(Target);
      ServiceLocator.GetService<ISimulation>().Add(bombEntity, ServiceLocator.GetService<ISimulation>().Objects);
      LocationItemComponent component1 = bombEntity.GetComponent<LocationItemComponent>();
      if (component1 != null)
        component1.Location = Target?.GetComponent<LocationItemComponent>()?.Location;
      NPCWeaponService component2 = ((IEntityView) Target).GameObject.GetComponent<NPCWeaponService>();
      ((IEntityView) bombEntity).Position = component2.ProjectileHitPosition;
      ((IEntityView) bombEntity).Rotation = component2.ProjectileHitRotation;
      ((Entity) Target).AddListener(this);
      ((Entity) bombEntity).AddListener(this);
      return true;
    }

    public bool Compute(float currentRealTime, float currentGameTime)
    {
      return !bombEntity.IsDisposed;
    }

    public void Cleanup()
    {
    }

    public void OnEntityEvent(IEntity sender, EntityEvents kind)
    {
      if (kind != EntityEvents.DisposeEvent)
        return;
      if (sender == Target)
      {
        ((Entity) Target).RemoveListener(this);
        bombEntity.Dispose();
      }
      else if (sender == bombEntity)
      {
        ((Entity) Target).RemoveListener(this);
        ((Entity) bombEntity).RemoveListener(this);
      }
    }
  }
}
