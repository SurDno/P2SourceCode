using System.Collections.Generic;
using System.Linq;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Inspectors;

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

    public bool Contains(IEntity entity) => entities.Contains(entity);

    public override void OnAdded() => base.OnAdded();

    public override void OnRemoved()
    {
      Clear();
      base.OnRemoved();
    }

    public void Enter(GameObject go)
    {
      if ((Object) go == (Object) null)
        return;
      IEntity entity = EntityUtility.GetEntity(go);
      if (entity == null || !entities.Add(entity))
        return;
      Debug.Log((object) ObjectInfoUtility.GetStream().Append("Enter trigger, actor : ").GetInfo(entity).Append(" , trigger : ").GetInfo(Owner));
      EventArgument<IEntity, ITriggerComponent> eventArguments = new EventArgument<IEntity, ITriggerComponent> {
        Actor = entity,
        Target = this
      };
      TriggerHandler entityEnterEvent = EntityEnterEvent;
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
      Exit(entity);
    }

    private void Exit(IEntity entity)
    {
      if (!entities.Remove(entity))
        return;
      Debug.Log((object) ObjectInfoUtility.GetStream().Append("Exit trigger, actor : ").GetInfo(entity).Append(" , trigger : ").GetInfo(Owner));
      EventArgument<IEntity, ITriggerComponent> eventArguments = new EventArgument<IEntity, ITriggerComponent> {
        Actor = entity,
        Target = this
      };
      TriggerHandler entityExitEvent = EntityExitEvent;
      if (entityExitEvent == null)
        return;
      entityExitEvent(ref eventArguments);
    }

    public override void OnChangeEnabled()
    {
      base.OnChangeEnabled();
      if (Owner.IsEnabledInHierarchy)
        return;
      while (entities.Count != 0)
        Exit(entities.First());
    }

    public void Clear()
    {
      foreach (IEntity entity in entities)
      {
        EventArgument<IEntity, ITriggerComponent> eventArguments = new EventArgument<IEntity, ITriggerComponent> {
          Actor = entity,
          Target = this
        };
        TriggerHandler entityExitEvent = EntityExitEvent;
        if (entityExitEvent != null)
          entityExitEvent(ref eventArguments);
      }
      entities.Clear();
    }

    public void Attach()
    {
    }

    public void Detach() => Clear();
  }
}
