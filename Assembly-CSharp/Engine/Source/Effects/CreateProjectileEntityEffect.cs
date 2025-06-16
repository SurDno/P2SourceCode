// Decompiled with JetBrains decompiler
// Type: Engine.Source.Effects.CreateProjectileEntityEffect
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

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
using UnityEngine;

#nullable disable
namespace Engine.Source.Effects
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class CreateProjectileEntityEffect : IEffect
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
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected ProjectileSpawnPlaceEnum spawnPlace = ProjectileSpawnPlaceEnum.None;

    public string Name => this.GetType().Name;

    [Inspected]
    public AbilityItem AbilityItem { get; set; }

    public IEntity Target { get; set; }

    public ParameterEffectQueueEnum Queue => this.queue;

    public bool Prepare(float currentRealTime, float currentGameTime)
    {
      IEntity entity = ServiceLocator.GetService<IFactory>().Instantiate<IEntity>(this.template.Value);
      ((Entity) entity).DontSave = true;
      entity.GetComponent<ParentComponent>()?.SetParent(this.Target);
      ServiceLocator.GetService<ISimulation>().Add(entity, ServiceLocator.GetService<ISimulation>().Objects);
      LocationItemComponent component1 = entity.GetComponent<LocationItemComponent>();
      if (component1 != null)
        component1.Location = this.Target?.GetComponent<LocationItemComponent>()?.Location;
      Transform transform = ((IEntityView) this.Target).GameObject.transform;
      NPCWeaponService component2 = ((IEntityView) this.Target).GameObject.GetComponent<NPCWeaponService>();
      if ((Object) component2 != (Object) null && this.spawnPlace == ProjectileSpawnPlaceEnum.Bomb && (Object) component2.BombParent != (Object) null)
        transform = component2.BombParent.transform;
      if ((Object) component2 != (Object) null && this.spawnPlace == ProjectileSpawnPlaceEnum.Samopal && (Object) component2.SamopalParent != (Object) null)
        transform = component2.SamopalParent.transform;
      if ((Object) transform != (Object) null)
      {
        ((IEntityView) entity).Position = transform.position;
        ((IEntityView) entity).Rotation = transform.rotation;
      }
      return true;
    }

    public bool Compute(float currentRealTime, float currentGameTime) => false;

    public void Cleanup()
    {
    }
  }
}
