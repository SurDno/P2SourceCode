using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Common;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Components.Interactable;
using Engine.Source.Services;
using Engine.Source.Services.Inputs;
using Engine.Source.Settings.External;
using Engine.Source.UI;
using Engine.Source.Utility;
using InputServices;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Components;

[Factory]
[Required(typeof(LocationItemComponent))]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class PlayerInteractableComponent : EngineComponent, IUpdatable, IPlayerActivated {
	[Inspected] private IEntity currentTarget;
	[Inspected] private InteractableComponent currentInteractable;
	[Inspected] private List<InteractItemInfo> validateItems = new();
	[Inspected] private HashSet<GameActionType> pressed = new();
	[FromLocator] private GameActionService gameActionService;
	[FromLocator] private UIService uiService;
	[FromLocator] private PickingService pickingService;
	private List<InteractItemInfo> tmp = new();
	private bool isJoystick;
	private IOrderedEnumerable<InteractItemInfo> validateItemsOrdered;
	private List<GameActionType> cachedLastInteractionTypes = new();
	private List<GameActionType> comparationList = new();

	public InteractableComponent Interactable => currentInteractable;

	public IEnumerable<InteractItemInfo> ValidateItems => validateItems;

	public override void OnChangeEnabled() {
		base.OnChangeEnabled();
		currentTarget = null;
		currentInteractable = null;
		validateItems.Clear();
	}

	public void ComputeUpdate() {
		if (!PlayerUtility.IsPlayerCanControlling)
			ClearActions();
		else {
			if (isJoystick != InputService.Instance.JoystickUsed) {
				isJoystick = InputService.Instance.JoystickUsed;
				cachedLastInteractionTypes = null;
			}

			var entity = pickingService.TargetEntity;
			if (entity != null && pickingService.TargetEntityDistance >
			    (double)ExternalSettingsInstance<ExternalCommonSettings>.Instance.InteractionDistance)
				entity = null;
			if (entity != currentTarget) {
				currentTarget = entity;
				currentInteractable = null;
				cachedLastInteractionTypes = null;
				if (currentTarget != null)
					currentInteractable = currentTarget.GetComponent<InteractableComponent>();
			}

			UpdateActions();
		}
	}

	private void ClearActions() {
		validateItems.Clear();
		cachedLastInteractionTypes = null;
	}

	private void UpdateActions() {
		var isSame = false;
		if (currentInteractable != null && !currentInteractable.IsDisposed &&
		    currentInteractable.Owner.IsEnabledInHierarchy && currentInteractable.IsEnabled) {
			comparationList.Clear();
			currentInteractable.Items.ForEach(i => {
				if (!InteractValidationService.Validate(currentInteractable, i).Result)
					return;
				comparationList.Add(i.Action);
			});
			if (cachedLastInteractionTypes == null || !cachedLastInteractionTypes.SequenceEqual(comparationList)) {
				ClearActions();
				validateItems.AddRange(currentInteractable.GetValidateItems(Owner));
				cachedLastInteractionTypes = new List<GameActionType>(comparationList);
			} else
				isSame = true;
		} else
			ClearActions();

		if (!isSame) {
			if (validateItemsOrdered == null)
				validateItemsOrdered = validateItems.OrderBy(o => o.Invalid);
			tmp.Clear();
			tmp.AddRange(validateItemsOrdered);
			for (var index = 1; index < tmp.Count && !tmp[index].Invalid; ++index)
				if (tmp[index].Item.Action == tmp[index - 1].Item.Action) {
					tmp[index].Invalid = true;
					tmp[index].Dublicate = true;
				}

			var index1 = 0;
			for (var index2 = 0; index2 < validateItems.Count; ++index2) {
				var validateItem = validateItems[index2];
				if (validateItem.Invalid) {
					validateItem.OverrideAction = index1 < InteractUtility.DebugActions.Length
						? InteractUtility.DebugActions[index1].Action
						: InteractUtility.DebugActions[InteractUtility.DebugActions.Length - 1].Action;
					++index1;
				}
			}

			CoroutineService.Instance.WaitFrame(1, (Action)(() => UpdateIcons(false)));
		}

		UpdateIcons(isSame);
	}

	private void UpdateIcons(bool isSame) {
		var hudWindow = uiService.Get<IHudWindow>();
		if (hudWindow == null)
			return;
		var interactableInterface = hudWindow.InteractableInterface;
		if (interactableInterface == null)
			return;
		if (!isSame) {
			var info = InteractableWindow.IconType.None;
			if (!InputService.Instance.JoystickUsed) {
				var text = "";
				if (currentInteractable != null && !currentInteractable.IsDisposed) {
					info = DefaultInteractableMapping.GetIconType(currentInteractable, validateItems);
					text = DefaultInteractableMapping.GetText(validateItems);
				}

				interactableInterface.SetInfo(info, text);
			} else {
				List<KeyValuePair<Sprite, bool>> iconSprites = null;
				if (currentInteractable != null && !currentInteractable.IsDisposed) {
					var iconType = DefaultInteractableMapping.GetIconType(currentInteractable, validateItems);
					List<KeyValuePair<GameActionType, bool>> actions;
					var text = DefaultInteractableMapping.GetText(validateItems, out iconSprites, out actions);
					interactableInterface.SetInfo(iconType, text, iconSprites, actions);
				} else
					interactableInterface.DeactivateAllTitles();
			}
		} else
			interactableInterface.UpdateProgress();
	}

	private bool Listener(GameActionType type, bool down) {
		if (!PlayerUtility.IsPlayerCanControlling || !down || currentInteractable == null ||
		    currentInteractable.IsDisposed)
			return false;
		InteractItemInfo interactItemInfo = null;
		foreach (var validateItem in validateItems)
			if (validateItem.Invalid) {
				if (InstanceByRequest<EngineApplication>.Instance.IsDebug && validateItem.OverrideAction == type) {
					interactItemInfo = validateItem;
					break;
				}
			} else if (validateItem.Item.Action == type) {
				interactItemInfo = validateItem;
				break;
			}

		if (interactItemInfo == null)
			return false;
		currentInteractable.BeginInteract(Owner, interactItemInfo.Item.Type);
		currentInteractable = null;
		currentTarget = null;
		validateItems.Clear();
		UpdateIcons(false);
		return true;
	}

	public void PlayerActivated() {
		InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable(this);
		foreach (var interactAction in InteractUtility.InteractActions)
			gameActionService.AddListener(interactAction, Listener);
		JoystickLayoutSwitcher.Instance.OnLayoutChanged += OnLayoutChanged;
	}

	public void PlayerDeactivated() {
		InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable(this);
		foreach (var interactAction in InteractUtility.InteractActions)
			gameActionService.RemoveListener(interactAction, Listener);
		JoystickLayoutSwitcher.Instance.OnLayoutChanged -= OnLayoutChanged;
	}

	private void OnLayoutChanged(JoystickLayoutSwitcher.KeyLayouts newLayout) {
		cachedLastInteractionTypes = null;
		UpdateActions();
	}
}