using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Common.Components.Interactable;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Common.Types;
using Engine.Impl.Services;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Components.Interactable;
using Engine.Source.Connections;
using Engine.Source.Services;
using Engine.Source.Services.Inputs;
using Engine.Source.Settings.External;
using Inspectors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Engine.Source.Components
{
  [Factory(typeof (IInteractableComponent))]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave | TypeEnum.StateLoad)]
  public class InteractableComponent : EngineComponent, IInteractableComponent, IComponent, INeedSave
  {
    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    [CopyableProxy(MemberEnum.None)]
    protected bool isEnabled = true;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    [CopyableProxy(MemberEnum.None)]
    protected List<InteractItem> items = new List<InteractItem>();
    private static List<GameActionType> occupiedTypes = new List<GameActionType>();
    private Coroutine showSmallLoadingCoroutine;
    private bool gameWasPausedAtBeginInteract;

    [Inspected(Mutable = true)]
    public bool IsEnabled
    {
      get => this.isEnabled;
      set
      {
        this.isEnabled = value;
        this.OnChangeEnabled();
      }
    }

    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [Inspected]
    public LocalizedText Title { get; set; }

    public List<InteractItem> Items => this.items;

    public bool NeedSave
    {
      get
      {
        if (!(this.Owner.Template is IEntity template))
        {
          Debug.LogError((object) ("Template not found, owner : " + this.Owner.GetInfo()));
          return true;
        }
        InteractableComponent component = template.GetComponent<InteractableComponent>();
        if (component == null)
        {
          Debug.LogError((object) (this.GetType().Name + " not found, owner : " + this.Owner.GetInfo()));
          return true;
        }
        return this.isEnabled != component.isEnabled || this.Title != component.Title;
      }
    }

    public event Action<IEntity, IInteractableComponent, IInteractItem> BeginInteractEvent;

    public event Action<IEntity, IInteractableComponent, IInteractItem> EndInteractEvent;

    public IEnumerable<InteractItemInfo> GetValidateItems(IEntity owner)
    {
      IEntity player = ServiceLocator.GetService<ISimulation>().Player;
      ILocationItemComponent playerLocationItem = player.GetComponent<ILocationItemComponent>();
      ILocationItemComponent thisLocationItem = this.Owner.GetComponent<ILocationItemComponent>();
      foreach (InteractItem item in this.items)
      {
        ValidateResult result = InteractValidationService.Validate((IInteractableComponent) this, item);
        yield return new InteractItemInfo()
        {
          Item = item,
          Invalid = !result.Result,
          Reason = result.Reason,
          Crime = PlayerInteractableComponentUtility.GetInteractCriminal(this, item)
        };
        result = new ValidateResult();
      }
    }

    private IEnumerator ShowSmallLoading()
    {
      yield return (object) new WaitForSeconds(0.05f);
      ServiceLocator.GetService<UIService>().SmallLoading.gameObject.SetActive(true);
      this.showSmallLoadingCoroutine = (Coroutine) null;
    }

    public void BeginInteract(IEntity player, InteractType type)
    {
      InteractItem item = this.items.FirstOrDefault<InteractItem>((Func<InteractItem, bool>) (o => o.Type == type));
      if (item == null)
        return;
      this.FireBeginInteract(player, item);
      UnityAsset<GameObject> blueprint = item.Blueprint;
      if (item.Blueprint.Id == Guid.Empty)
        this.FireEndInteract(player, item);
      else if (ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.InteractableAsyncBlueprintStart)
      {
        this.showSmallLoadingCoroutine = CoroutineService.Instance.StartCoroutine(this.ShowSmallLoading());
        this.gameWasPausedAtBeginInteract = InstanceByRequest<EngineApplication>.Instance.IsPaused;
        InstanceByRequest<EngineApplication>.Instance.IsPaused = true;
        BlueprintServiceUtility.StartAsync(item.Blueprint, this.Owner, (Action) (() =>
        {
          if (this.showSmallLoadingCoroutine != null)
            CoroutineService.Instance.StopCoroutine(this.showSmallLoadingCoroutine);
          else
            ServiceLocator.GetService<UIService>().SmallLoading.gameObject.SetActive(false);
          InstanceByRequest<EngineApplication>.Instance.IsPaused = this.gameWasPausedAtBeginInteract;
        }), (Action) (() => this.FireEndInteract(player, item)), true, this.Owner.GetInfo());
      }
      else
        BlueprintServiceUtility.Start(item.Blueprint.Value, this.Owner, (Action) (() => this.FireEndInteract(player, item)), this.Owner.GetInfo());
    }

    private void FireBeginInteract(IEntity player, InteractItem item)
    {
      Action<IEntity, IInteractableComponent, IInteractItem> beginInteractEvent = this.BeginInteractEvent;
      if (beginInteractEvent != null)
        beginInteractEvent(player, (IInteractableComponent) this, (IInteractItem) item);
      player.GetComponent<ControllerComponent>()?.FireBeginInteract(this, item);
    }

    private void FireEndInteract(IEntity player, InteractItem item)
    {
      Action<IEntity, IInteractableComponent, IInteractItem> endInteractEvent = this.EndInteractEvent;
      if (endInteractEvent != null)
        endInteractEvent(player, (IInteractableComponent) this, (IInteractItem) item);
      player.GetComponent<ControllerComponent>()?.FireEndInteract(this, item);
    }
  }
}
