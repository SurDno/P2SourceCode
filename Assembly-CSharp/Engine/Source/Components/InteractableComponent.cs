using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
using UnityEngine;

namespace Engine.Source.Components;

[Factory(typeof(IInteractableComponent))]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave |
               TypeEnum.StateLoad)]
public class InteractableComponent : EngineComponent, IInteractableComponent, IComponent, INeedSave {
	[StateSaveProxy]
	[StateLoadProxy]
	[DataReadProxy]
	[DataWriteProxy]
	[Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	[CopyableProxy]
	protected bool isEnabled = true;

	[DataReadProxy] [DataWriteProxy] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)] [CopyableProxy()]
	protected List<InteractItem> items = new();

	private static List<GameActionType> occupiedTypes = new();
	private Coroutine showSmallLoadingCoroutine;
	private bool gameWasPausedAtBeginInteract;

	[Inspected(Mutable = true)]
	public bool IsEnabled {
		get => isEnabled;
		set {
			isEnabled = value;
			OnChangeEnabled();
		}
	}

	[StateSaveProxy]
	[StateLoadProxy]
	[Inspected]
	public LocalizedText Title { get; set; }

	public List<InteractItem> Items => items;

	public bool NeedSave {
		get {
			if (!(Owner.Template is IEntity template)) {
				Debug.LogError("Template not found, owner : " + Owner.GetInfo());
				return true;
			}

			var component = template.GetComponent<InteractableComponent>();
			if (component == null) {
				Debug.LogError(GetType().Name + " not found, owner : " + Owner.GetInfo());
				return true;
			}

			return isEnabled != component.isEnabled || Title != component.Title;
		}
	}

	public event Action<IEntity, IInteractableComponent, IInteractItem> BeginInteractEvent;

	public event Action<IEntity, IInteractableComponent, IInteractItem> EndInteractEvent;

	public IEnumerable<InteractItemInfo> GetValidateItems(IEntity owner) {
		var player = ServiceLocator.GetService<ISimulation>().Player;
		var playerLocationItem = player.GetComponent<ILocationItemComponent>();
		var thisLocationItem = Owner.GetComponent<ILocationItemComponent>();
		foreach (var item in items) {
			var result = InteractValidationService.Validate(this, item);
			yield return new InteractItemInfo {
				Item = item,
				Invalid = !result.Result,
				Reason = result.Reason,
				Crime = PlayerInteractableComponentUtility.GetInteractCriminal(this, item)
			};
			result = new ValidateResult();
		}
	}

	private IEnumerator ShowSmallLoading() {
		yield return new WaitForSeconds(0.05f);
		ServiceLocator.GetService<UIService>().SmallLoading.gameObject.SetActive(true);
		showSmallLoadingCoroutine = null;
	}

	public void BeginInteract(IEntity player, InteractType type) {
		var item = items.FirstOrDefault(o => o.Type == type);
		if (item == null)
			return;
		FireBeginInteract(player, item);
		var blueprint = item.Blueprint;
		if (item.Blueprint.Id == Guid.Empty)
			FireEndInteract(player, item);
		else if (ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.InteractableAsyncBlueprintStart) {
			showSmallLoadingCoroutine = CoroutineService.Instance.StartCoroutine(ShowSmallLoading());
			gameWasPausedAtBeginInteract = InstanceByRequest<EngineApplication>.Instance.IsPaused;
			InstanceByRequest<EngineApplication>.Instance.IsPaused = true;
			BlueprintServiceUtility.StartAsync(item.Blueprint, Owner, (Action)(() => {
				if (showSmallLoadingCoroutine != null)
					CoroutineService.Instance.StopCoroutine(showSmallLoadingCoroutine);
				else
					ServiceLocator.GetService<UIService>().SmallLoading.gameObject.SetActive(false);
				InstanceByRequest<EngineApplication>.Instance.IsPaused = gameWasPausedAtBeginInteract;
			}), (Action)(() => FireEndInteract(player, item)), true, Owner.GetInfo());
		} else
			BlueprintServiceUtility.Start(item.Blueprint.Value, Owner, (Action)(() => FireEndInteract(player, item)),
				Owner.GetInfo());
	}

	private void FireBeginInteract(IEntity player, InteractItem item) {
		var beginInteractEvent = BeginInteractEvent;
		if (beginInteractEvent != null)
			beginInteractEvent(player, this, item);
		player.GetComponent<ControllerComponent>()?.FireBeginInteract(this, item);
	}

	private void FireEndInteract(IEntity player, InteractItem item) {
		var endInteractEvent = EndInteractEvent;
		if (endInteractEvent != null)
			endInteractEvent(player, this, item);
		player.GetComponent<ControllerComponent>()?.FireEndInteract(this, item);
	}
}