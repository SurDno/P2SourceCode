using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Engine.Common;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components.Interactable;
using Engine.Source.Settings;
using Engine.Source.Utility;
using InputServices;
using Inspectors;
using SRDebugger.Services;
using SRF.Service;
using UnityEngine;

namespace Engine.Source.Services.Inputs;

[RuntimeService(typeof(GameActionService))]
public class GameActionService : IInitialisable, IUpdatable {
	public ActionGroupContext context = ActionGroupContext.None;
	[Inspected] private Dictionary<GameActionType, List<GameActionHandle>> actions = new();
	[Inspected] private HashSet<ActionGroup> computedBinds = new();
	private IDebugService debug;
	[Inspected] private List<ActionGroup> binds = new();
	private bool initialise;
	private HashSet<KeyCode> tmp = new();
	private HashSet<string> tmp2 = new();
	private List<ActionGroup> tmpBinds = new();
	private List<GameActionHandle> tmpListeners = new();

	public event Action<KeyCode> OnKeyPressed;

	public event Action<GameActionType> OnGameAction;

	public event Action OnBindsChange;

	public void AddListener(GameActionType type, GameActionHandle action, bool isHighPriority = false) {
		if (!initialise)
			throw new Exception(GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
		List<GameActionHandle> gameActionHandleList;
		if (!actions.TryGetValue(type, out gameActionHandleList)) {
			gameActionHandleList = new List<GameActionHandle>();
			actions.Add(type, gameActionHandleList);
		}

		if (HasListener(type, action))
			return;
		if (isHighPriority)
			gameActionHandleList.Insert(0, action);
		else
			gameActionHandleList.Add(action);
	}

	public bool HasListener(GameActionType type, GameActionHandle action) {
		if (!initialise)
			throw new Exception(GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
		List<GameActionHandle> gameActionHandleList;
		return actions.TryGetValue(type, out gameActionHandleList) && gameActionHandleList.Contains(action);
	}

	public void RemoveListener(GameActionType type, GameActionHandle action) {
		List<GameActionHandle> gameActionHandleList;
		if (!actions.TryGetValue(type, out gameActionHandleList)) {
			gameActionHandleList = new List<GameActionHandle>();
			actions.Add(type, gameActionHandleList);
		}

		if (!gameActionHandleList.Contains(action))
			return;
		gameActionHandleList.Remove(action);
	}

	public void Initialise() {
		initialise = true;
		InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable(this);
		InstanceByRequest<EngineApplication>.Instance.OnApplicationFocusEvent += OnApplicationFocusEvent;
		JoystickLayoutSwitcher.Instance.OnLayoutChanged += OnLayoutChanged;
		LoadSettings();
	}

	private void OnLayoutChanged(JoystickLayoutSwitcher.KeyLayouts newLayout) {
		ResetKeys();
		LoadSettings();
	}

	public void Terminate() {
		InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable(this);
		InstanceByRequest<EngineApplication>.Instance.OnApplicationFocusEvent -= OnApplicationFocusEvent;
		JoystickLayoutSwitcher.Instance.OnLayoutChanged -= OnLayoutChanged;
		initialise = false;
	}

	private void LoadSettings() {
		if (!initialise)
			throw new Exception(GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
		ResetRebinds();
		foreach (var keyValuePair in InstanceByRequest<InputGameSetting>.Instance.KeySettings.Value)
			SetRebind(keyValuePair.Key, keyValuePair.Value);
		var onBindsChange = OnBindsChange;
		if (onBindsChange == null)
			return;
		onBindsChange();
	}

	private void SaveSettings() {
		if (!initialise)
			throw new Exception(GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
		var source = new List<KeyValuePair<string, KeyCode>>();
		foreach (var bind1 in binds) {
			var bind = bind1;
			if (bind.Context != ActionGroupContext.Debug &&
			    !source.Any(x => x.Key == bind.Name && x.Value == bind.Key) &&
			    !JoystickLayoutSwitcher.Instance.Groups.Any(x => x.Name == bind.Name && x.Key == bind.Key))
				source.Add(new KeyValuePair<string, KeyCode>(bind.Name, bind.Key));
		}

		InstanceByRequest<InputGameSetting>.Instance.KeySettings.Value = source;
	}

	private bool SetRebind(string name, KeyCode key) {
		ActionGroup actionGroup = null;
		foreach (var bind in binds)
			if (bind.Name == name && bind.IsChangeble) {
				actionGroup = bind;
				break;
			}

		if (actionGroup == null || actionGroup.Key == key)
			return false;
		actionGroup.Key = key;
		for (var index = 0; index < binds.Count; ++index) {
			var bind = binds[index];
			if (bind != actionGroup && bind.IsChangeble && bind.Context == actionGroup.Context && bind.Key == key)
				bind.Key = KeyCode.None;
		}

		return true;
	}

	public void AddRebind(ActionGroup action, KeyCode key) {
		if (!initialise)
			throw new Exception(GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
		if (!SetRebind(action.Name, key))
			return;
		SaveSettings();
		var onBindsChange = OnBindsChange;
		if (onBindsChange == null)
			return;
		onBindsChange();
	}

	public void ResetAllRebinds() {
		if (!initialise)
			throw new Exception(GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
		ResetRebinds();
		SaveSettings();
		var onBindsChange = OnBindsChange;
		if (onBindsChange == null)
			return;
		onBindsChange();
	}

	private void ResetRebinds() {
		binds.Clear();
		foreach (var group in JoystickLayoutSwitcher.Instance.Groups)
			binds.Add(CloneableObjectUtility.Clone(group));
		foreach (var debugAction in InteractUtility.DebugActions) {
			var actionGroup = ServiceCache.Factory.Create<ActionGroup>();
			actionGroup.Name = debugAction.Action.ToString();
			actionGroup.Key = debugAction.Key;
			actionGroup.IsChangeble = false;
			actionGroup.Hide = true;
			actionGroup.Interact = true;
			actionGroup.Context = ActionGroupContext.Debug;
			actionGroup.Actions.Add(debugAction.Action);
			binds.Add(actionGroup);
		}
	}

	public List<ActionGroup> GetBinds() {
		if (!initialise)
			throw new Exception(GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
		return binds;
	}

	public void ComputeUpdate() {
		if (!initialise)
			throw new Exception(GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
		if (debug == null)
			debug = SRServiceManager.GetService<IDebugService>();
		if (debug != null && debug.IsDebugPanelVisible)
			ResetKeys();
		else {
			tmp.Clear();
			tmp2.Clear();
			tmpBinds.Clear();
			tmpBinds.AddRange(GetBinds());
			foreach (var tmpBind in tmpBinds)
				if (!computedBinds.Contains(tmpBind) && !tmp.Contains(tmpBind.Key) &&
				    !tmp2.Contains(tmpBind.Joystick) && IsKeyDown(tmpBind) && ComputeAction(tmpBind, true)) {
					computedBinds.Add(tmpBind);
					tmp.Add(tmpBind.Key);
					tmp2.Add(tmpBind.Joystick);
				}

			ComputeReleased();
			if (!Input.anyKeyDown)
				return;
			foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
				if (keyCode < KeyCode.JoystickButton0) {
					if (Input.GetKeyDown(keyCode)) {
						if (OnKeyPressed != null)
							FireKeyPressed(keyCode);
						InputService.Instance.JoystickUsed = false;
					}
				} else
					break;
		}
	}

	private void ComputeReleased() {
		label_1:
		foreach (var computedBind in computedBinds)
			if (IsKeyRelease(computedBind)) {
				computedBinds.Remove(computedBind);
				ComputeAction(computedBind, false);
				goto label_1;
			}
	}

	private bool IsKeyDown(ActionGroup bind) {
		return (!debug.IsDebugging
			       ? Input.GetKeyDown(bind.Key)
			       : InputUtility.IsKeyDown(bind.Key) || InputUtility.IsKeyDown(bind.Key, KeyModifficator.Shift) ||
			         InputUtility.IsKeyDown(bind.Key, KeyModifficator.Alt)) |
		       InputService.Instance.GetButtonDown(bind.Joystick, bind.JoystickHold);
	}

	private bool IsKeyRelease(ActionGroup bind) {
		return !Input.GetKey(bind.Key) && !InputService.Instance.GetButton(bind.Joystick, bind.JoystickHold);
	}

	private void FireKeyPressed(KeyCode code) {
		if (!initialise)
			throw new Exception(GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
		OnKeyPressed(code);
	}

	private bool ComputeAction(ActionGroup group, bool down) {
		if (!initialise)
			throw new Exception(GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
		foreach (var action in group.Actions)
			if (TryGetListeners(action, tmpListeners))
				foreach (var tmpListener in tmpListeners)
					if (tmpListener(action, down)) {
						if (down) {
							var onGameAction = OnGameAction;
							if (onGameAction != null)
								onGameAction(action);
						}

						return true;
					}

		return false;
	}

	private void ComputeAction(ActionGroup group, bool down, HashSet<GameActionType> result) {
		foreach (var action in group.Actions)
			result.Add(action);
	}

	private void OnApplicationFocusEvent(bool focus) {
		if (!initialise)
			throw new Exception(GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
		if (focus)
			return;
		ResetKeys();
	}

	private void ResetKeys() {
		if (!initialise)
			throw new Exception(GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
		foreach (var computedBind in computedBinds)
			ComputeAction(computedBind, false);
		computedBinds.Clear();
	}

	private bool TryGetListeners(GameActionType type, List<GameActionHandle> listeners) {
		listeners.Clear();
		List<GameActionHandle> collection;
		if (!actions.TryGetValue(type, out collection))
			return false;
		listeners.AddRange(collection);
		return true;
	}
}