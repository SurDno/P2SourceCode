// Decompiled with JetBrains decompiler
// Type: Engine.Source.Commons.Abilities.Controllers.FlameAbilityController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Behaviours.Components;
using Engine.Common;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Impl.Services.Factories;
using Inspectors;
using System;

#nullable disable
namespace Engine.Source.Commons.Abilities.Controllers
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class FlameAbilityController : IAbilityController, IUpdatable
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected bool realTime = false;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected double interval = 0.0;
    private IEntity owner;
    private AbilityItem abilityItem;
    private PivotSanitar sanitar;
    private double time;

    public void Initialise(AbilityItem abilityItem)
    {
      this.abilityItem = abilityItem;
      this.owner = abilityItem.Ability.Owner;
      ((IEntityView) this.owner).OnGameObjectChangedEvent += new Action(this.OnGameObjectChangedEvent);
      this.OnGameObjectChangedEvent();
    }

    public void Shutdown()
    {
      ((IEntityView) this.owner).OnGameObjectChangedEvent -= new Action(this.OnGameObjectChangedEvent);
    }

    private void OnGameObjectChangedEvent()
    {
      if ((UnityEngine.Object) this.sanitar != (UnityEngine.Object) null)
      {
        InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable((IUpdatable) this);
        this.sanitar = (PivotSanitar) null;
      }
      if (!((IEntityView) this.owner).IsAttached)
        return;
      this.sanitar = ((IEntityView) this.owner).GameObject.GetComponent<PivotSanitar>();
      if ((UnityEngine.Object) this.sanitar != (UnityEngine.Object) null)
      {
        this.time = 0.0;
        InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable((IUpdatable) this);
      }
    }

    public void ComputeUpdate()
    {
      if ((UnityEngine.Object) this.sanitar == (UnityEngine.Object) null)
        return;
      if (!this.sanitar.Flamethrower)
      {
        this.time = 0.0;
      }
      else
      {
        TimeService service = ServiceLocator.GetService<TimeService>();
        double num = this.realTime ? service.RealTime.TotalSeconds : service.AbsoluteGameTime.TotalSeconds;
        if (this.time + this.interval > num)
          return;
        this.time = num;
        this.abilityItem.Active = true;
        this.abilityItem.Active = false;
      }
    }
  }
}
