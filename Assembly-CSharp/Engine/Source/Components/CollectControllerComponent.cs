// Decompiled with JetBrains decompiler
// Type: Engine.Source.Components.CollectControllerComponent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

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
using System.Collections.Generic;

#nullable disable
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
    [DataReadProxy(MemberEnum.None, Name = "Storable")]
    [DataWriteProxy(MemberEnum.None, Name = "Storable")]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    [CopyableProxy(MemberEnum.None)]
    protected Typed<IEntity> itemEntity;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    [CopyableProxy(MemberEnum.None)]
    protected bool sendActionEvent = false;
    [FromThis]
    private DynamicModelComponent model = (DynamicModelComponent) null;
    [FromThis]
    private ParametersComponent parameters = (ParametersComponent) null;
    private IParameter<bool> collectedParameter;

    public bool ValidateCollect()
    {
      return this.collectedParameter != null && !this.collectedParameter.Value;
    }

    public void Collect()
    {
      if (this.collectedParameter == null || this.collectedParameter.Value)
        return;
      this.collectedParameter.Value = true;
      IStorableComponent component1 = this.itemEntity.Value?.GetComponent<IStorableComponent>();
      if (component1 != null)
      {
        IStorageComponent component2 = ServiceLocator.GetService<ISimulation>().Player?.GetComponent<IStorageComponent>();
        if (component2 != null)
        {
          IEntity entity = ServiceLocator.GetService<IFactory>().Instantiate<IEntity>(component1.Owner);
          ServiceLocator.GetService<ISimulation>().Add(entity, ServiceLocator.GetService<ISimulation>().Storables);
          component2.AddItemOrDrop(entity.GetComponent<IStorableComponent>(), (IInventoryComponent) null);
          ServiceLocator.GetService<NotificationService>().AddNotify(NotificationEnum.ItemRecieve, new object[1]
          {
            (object) component1.Owner
          });
        }
      }
      if (!this.sendActionEvent)
        return;
      ServiceLocator.GetService<ISimulation>().Player.GetComponent<PlayerControllerComponent>()?.ComputeAction(ActionEnum.CollectItem);
    }

    public override void OnAdded()
    {
      base.OnAdded();
      if (this.parameters != null)
        this.collectedParameter = this.parameters.GetByName<bool>(ParameterNameEnum.Collected);
      InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable((IUpdatable) this);
    }

    public override void OnRemoved()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable((IUpdatable) this);
      this.collectedParameter = (IParameter<bool>) null;
      base.OnRemoved();
    }

    public void ComputeUpdate()
    {
      if (this.model == null || this.collectedParameter == null)
        return;
      bool flag = !this.collectedParameter.Value;
      if (flag == this.model.IsEnabled)
        return;
      this.model.IsEnabled = flag;
    }

    public void StoreState(List<IParameter> states, bool indoor)
    {
      CrowdContextUtility.Store(this.parameters, states, ParameterNameEnum.Collected);
    }

    public void RestoreState(List<IParameter> states, bool indoor)
    {
      CrowdContextUtility.Restore(this.parameters, states, ParameterNameEnum.Collected);
    }
  }
}
