// Decompiled with JetBrains decompiler
// Type: Engine.Source.Components.TriggerComponent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Inspectors;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable disable
namespace Engine.Source.Components
{
  [Factory(typeof (ITriggerComponent))]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class TriggerComponent : EngineComponent, ITriggerComponent, IComponent
  {
    [Inspected]
    private HashSet<IEntity> entities = new HashSet<IEntity>();

    public event TriggerHandler EntityEnterEvent;

    public event TriggerHandler EntityExitEvent;

    public bool Contains(IEntity entity) => this.entities.Contains(entity);

    public override void OnAdded() => base.OnAdded();

    public override void OnRemoved()
    {
      this.Clear();
      base.OnRemoved();
    }

    public void Enter(GameObject go)
    {
      if ((Object) go == (Object) null)
        return;
      IEntity entity = EntityUtility.GetEntity(go);
      if (entity == null || !this.entities.Add(entity))
        return;
      Debug.Log((object) ObjectInfoUtility.GetStream().Append("Enter trigger, actor : ").GetInfo((object) entity).Append(" , trigger : ").GetInfo((object) this.Owner));
      EventArgument<IEntity, ITriggerComponent> eventArguments = new EventArgument<IEntity, ITriggerComponent>()
      {
        Actor = entity,
        Target = (ITriggerComponent) this
      };
      TriggerHandler entityEnterEvent = this.EntityEnterEvent;
      if (entityEnterEvent == null)
        return;
      entityEnterEvent(ref eventArguments);
    }

    public void Exit(GameObject go)
    {
      if ((Object) go == (Object) null)
        return;
      IEntity entity = EntityUtility.GetEntity(go);
      if (entity == null)
        return;
      this.Exit(entity);
    }

    private void Exit(IEntity entity)
    {
      if (!this.entities.Remove(entity))
        return;
      Debug.Log((object) ObjectInfoUtility.GetStream().Append("Exit trigger, actor : ").GetInfo((object) entity).Append(" , trigger : ").GetInfo((object) this.Owner));
      EventArgument<IEntity, ITriggerComponent> eventArguments = new EventArgument<IEntity, ITriggerComponent>()
      {
        Actor = entity,
        Target = (ITriggerComponent) this
      };
      TriggerHandler entityExitEvent = this.EntityExitEvent;
      if (entityExitEvent == null)
        return;
      entityExitEvent(ref eventArguments);
    }

    public override void OnChangeEnabled()
    {
      base.OnChangeEnabled();
      if (this.Owner.IsEnabledInHierarchy)
        return;
      while (this.entities.Count != 0)
        this.Exit(this.entities.First<IEntity>());
    }

    public void Clear()
    {
      foreach (IEntity entity in this.entities)
      {
        EventArgument<IEntity, ITriggerComponent> eventArguments = new EventArgument<IEntity, ITriggerComponent>()
        {
          Actor = entity,
          Target = (ITriggerComponent) this
        };
        TriggerHandler entityExitEvent = this.EntityExitEvent;
        if (entityExitEvent != null)
          entityExitEvent(ref eventArguments);
      }
      this.entities.Clear();
    }

    public void Attach()
    {
    }

    public void Detach() => this.Clear();
  }
}
