using System.Collections.Generic;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Components.Crowds;
using Engine.Source.Connections;
using Engine.Source.Services;
using Inspectors;

namespace Engine.Source.Components
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class CollectControllerComponent : 
    EngineComponent,
    IUpdatable,
    ICrowdContextComponent,
    IComponent
  {
    [DataReadProxy(Name = "Storable")]
    [DataWriteProxy(Name = "Storable")]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    [CopyableProxy]
    protected Typed<IEntity> itemEntity;
    [DataReadProxy]
    [DataWriteProxy]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    [CopyableProxy()]
    protected bool sendActionEvent = false;
    [FromThis]
    private DynamicModelComponent model = null;
    [FromThis]
    private ParametersComponent parameters = null;
    private IParameter<bool> collectedParameter;

    public bool ValidateCollect()
    {
      return collectedParameter != null && !collectedParameter.Value;
    }

    public void Collect()
    {
      if (collectedParameter == null || collectedParameter.Value)
        return;
      collectedParameter.Value = true;
      IStorableComponent component1 = itemEntity.Value?.GetComponent<IStorableComponent>();
      if (component1 != null)
      {
        IStorageComponent component2 = ServiceLocator.GetService<ISimulation>().Player?.GetComponent<IStorageComponent>();
        if (component2 != null)
        {
          IEntity entity = ServiceLocator.GetService<IFactory>().Instantiate(component1.Owner);
          ServiceLocator.GetService<ISimulation>().Add(entity, ServiceLocator.GetService<ISimulation>().Storables);
          component2.AddItemOrDrop(entity.GetComponent<IStorableComponent>(), null);
          ServiceLocator.GetService<NotificationService>().AddNotify(NotificationEnum.ItemRecieve, component1.Owner);
        }
      }
      if (!sendActionEvent)
        return;
      ServiceLocator.GetService<ISimulation>().Player.GetComponent<PlayerControllerComponent>()?.ComputeAction(ActionEnum.CollectItem);
    }

    public override void OnAdded()
    {
      base.OnAdded();
      if (parameters != null)
        collectedParameter = parameters.GetByName<bool>(ParameterNameEnum.Collected);
      InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable(this);
    }

    public override void OnRemoved()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable(this);
      collectedParameter = null;
      base.OnRemoved();
    }

    public void ComputeUpdate()
    {
      if (model == null || collectedParameter == null)
        return;
      bool flag = !collectedParameter.Value;
      if (flag == model.IsEnabled)
        return;
      model.IsEnabled = flag;
    }

    public void StoreState(List<IParameter> states, bool indoor)
    {
      CrowdContextUtility.Store(parameters, states, ParameterNameEnum.Collected);
    }

    public void RestoreState(List<IParameter> states, bool indoor)
    {
      CrowdContextUtility.Restore(parameters, states, ParameterNameEnum.Collected);
    }
  }
}
