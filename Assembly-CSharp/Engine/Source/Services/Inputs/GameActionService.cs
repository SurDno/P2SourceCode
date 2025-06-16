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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Engine.Source.Services.Inputs
{
  [RuntimeService(new System.Type[] {typeof (GameActionService)})]
  public class GameActionService : IInitialisable, IUpdatable
  {
    public ActionGroupContext context = ActionGroupContext.None;
    [Inspected]
    private Dictionary<GameActionType, List<GameActionHandle>> actions = new Dictionary<GameActionType, List<GameActionHandle>>();
    [Inspected]
    private HashSet<ActionGroup> computedBinds = new HashSet<ActionGroup>();
    private IDebugService debug;
    [Inspected]
    private List<ActionGroup> binds = new List<ActionGroup>();
    private bool initialise;
    private HashSet<KeyCode> tmp = new HashSet<KeyCode>();
    private HashSet<string> tmp2 = new HashSet<string>();
    private List<ActionGroup> tmpBinds = new List<ActionGroup>();
    private List<GameActionHandle> tmpListeners = new List<GameActionHandle>();

    public event Action<KeyCode> OnKeyPressed;

    public event Action<GameActionType> OnGameAction;

    public event Action OnBindsChange;

    public void AddListener(GameActionType type, GameActionHandle action, bool isHighPriority = false)
    {
      if (!this.initialise)
        throw new Exception(this.GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
      List<GameActionHandle> gameActionHandleList;
      if (!this.actions.TryGetValue(type, out gameActionHandleList))
      {
        gameActionHandleList = new List<GameActionHandle>();
        this.actions.Add(type, gameActionHandleList);
      }
      if (this.HasListener(type, action))
        return;
      if (isHighPriority)
        gameActionHandleList.Insert(0, action);
      else
        gameActionHandleList.Add(action);
    }

    public bool HasListener(GameActionType type, GameActionHandle action)
    {
      if (!this.initialise)
        throw new Exception(this.GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
      List<GameActionHandle> gameActionHandleList;
      return this.actions.TryGetValue(type, out gameActionHandleList) && gameActionHandleList.Contains(action);
    }

    public void RemoveListener(GameActionType type, GameActionHandle action)
    {
      List<GameActionHandle> gameActionHandleList;
      if (!this.actions.TryGetValue(type, out gameActionHandleList))
      {
        gameActionHandleList = new List<GameActionHandle>();
        this.actions.Add(type, gameActionHandleList);
      }
      if (!gameActionHandleList.Contains(action))
        return;
      gameActionHandleList.Remove(action);
    }

    public void Initialise()
    {
      this.initialise = true;
      InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable((IUpdatable) this);
      InstanceByRequest<EngineApplication>.Instance.OnApplicationFocusEvent += new Action<bool>(this.OnApplicationFocusEvent);
      JoystickLayoutSwitcher.Instance.OnLayoutChanged += new Action<JoystickLayoutSwitcher.KeyLayouts>(this.OnLayoutChanged);
      this.LoadSettings();
    }

    private void OnLayoutChanged(JoystickLayoutSwitcher.KeyLayouts newLayout)
    {
      this.ResetKeys();
      this.LoadSettings();
    }

    public void Terminate()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable((IUpdatable) this);
      InstanceByRequest<EngineApplication>.Instance.OnApplicationFocusEvent -= new Action<bool>(this.OnApplicationFocusEvent);
      JoystickLayoutSwitcher.Instance.OnLayoutChanged -= new Action<JoystickLayoutSwitcher.KeyLayouts>(this.OnLayoutChanged);
      this.initialise = false;
    }

    private void LoadSettings()
    {
      if (!this.initialise)
        throw new Exception(this.GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
      this.ResetRebinds();
      foreach (KeyValuePair<string, KeyCode> keyValuePair in InstanceByRequest<InputGameSetting>.Instance.KeySettings.Value)
        this.SetRebind(keyValuePair.Key, keyValuePair.Value);
      Action onBindsChange = this.OnBindsChange;
      if (onBindsChange == null)
        return;
      onBindsChange();
    }

    private void SaveSettings()
    {
      if (!this.initialise)
        throw new Exception(this.GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
      List<KeyValuePair<string, KeyCode>> source = new List<KeyValuePair<string, KeyCode>>();
      foreach (ActionGroup bind1 in this.binds)
      {
        ActionGroup bind = bind1;
        if (bind.Context != ActionGroupContext.Debug && !source.Any<KeyValuePair<string, KeyCode>>((Func<KeyValuePair<string, KeyCode>, bool>) (x => x.Key == bind.Name && x.Value == bind.Key)) && !JoystickLayoutSwitcher.Instance.Groups.Any<ActionGroup>((Func<ActionGroup, bool>) (x => x.Name == bind.Name && x.Key == bind.Key)))
          source.Add(new KeyValuePair<string, KeyCode>(bind.Name, bind.Key));
      }
      InstanceByRequest<InputGameSetting>.Instance.KeySettings.Value = source;
    }

    private bool SetRebind(string name, KeyCode key)
    {
      ActionGroup actionGroup = (ActionGroup) null;
      foreach (ActionGroup bind in this.binds)
      {
        if (bind.Name == name && bind.IsChangeble)
        {
          actionGroup = bind;
          break;
        }
      }
      if (actionGroup == null || actionGroup.Key == key)
        return false;
      actionGroup.Key = key;
      for (int index = 0; index < this.binds.Count; ++index)
      {
        ActionGroup bind = this.binds[index];
        if (bind != actionGroup && bind.IsChangeble && bind.Context == actionGroup.Context && bind.Key == key)
          bind.Key = KeyCode.None;
      }
      return true;
    }

    public void AddRebind(ActionGroup action, KeyCode key)
    {
      if (!this.initialise)
        throw new Exception(this.GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
      if (!this.SetRebind(action.Name, key))
        return;
      this.SaveSettings();
      Action onBindsChange = this.OnBindsChange;
      if (onBindsChange == null)
        return;
      onBindsChange();
    }

    public void ResetAllRebinds()
    {
      if (!this.initialise)
        throw new Exception(this.GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
      this.ResetRebinds();
      this.SaveSettings();
      Action onBindsChange = this.OnBindsChange;
      if (onBindsChange == null)
        return;
      onBindsChange();
    }

    private void ResetRebinds()
    {
      this.binds.Clear();
      foreach (ActionGroup group in JoystickLayoutSwitcher.Instance.Groups)
        this.binds.Add(CloneableObjectUtility.Clone<ActionGroup>(group));
      foreach (ActionInfo debugAction in InteractUtility.DebugActions)
      {
        ActionGroup actionGroup = ServiceCache.Factory.Create<ActionGroup>();
        actionGroup.Name = debugAction.Action.ToString();
        actionGroup.Key = debugAction.Key;
        actionGroup.IsChangeble = false;
        actionGroup.Hide = true;
        actionGroup.Interact = true;
        actionGroup.Context = ActionGroupContext.Debug;
        actionGroup.Actions.Add(debugAction.Action);
        this.binds.Add(actionGroup);
      }
    }

    public List<ActionGroup> GetBinds()
    {
      if (!this.initialise)
        throw new Exception(this.GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
      return this.binds;
    }

    public void ComputeUpdate()
    {
      if (!this.initialise)
        throw new Exception(this.GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
      if (this.debug == null)
        this.debug = SRServiceManager.GetService<IDebugService>();
      if (this.debug != null && this.debug.IsDebugPanelVisible)
      {
        this.ResetKeys();
      }
      else
      {
        this.tmp.Clear();
        this.tmp2.Clear();
        this.tmpBinds.Clear();
        this.tmpBinds.AddRange((IEnumerable<ActionGroup>) this.GetBinds());
        foreach (ActionGroup tmpBind in this.tmpBinds)
        {
          if (!this.computedBinds.Contains(tmpBind) && !this.tmp.Contains(tmpBind.Key) && !this.tmp2.Contains(tmpBind.Joystick) && this.IsKeyDown(tmpBind) && this.ComputeAction(tmpBind, true))
          {
            this.computedBinds.Add(tmpBind);
            this.tmp.Add(tmpBind.Key);
            this.tmp2.Add(tmpBind.Joystick);
          }
        }
        this.ComputeReleased();
        if (!Input.anyKeyDown)
          return;
        foreach (KeyCode keyCode in Enum.GetValues(typeof (KeyCode)))
        {
          if (keyCode < KeyCode.JoystickButton0)
          {
            if (Input.GetKeyDown(keyCode))
            {
              if (this.OnKeyPressed != null)
                this.FireKeyPressed(keyCode);
              InputService.Instance.JoystickUsed = false;
            }
          }
          else
            break;
        }
      }
    }

    private void ComputeReleased()
    {
label_1:
      foreach (ActionGroup computedBind in this.computedBinds)
      {
        if (this.IsKeyRelease(computedBind))
        {
          this.computedBinds.Remove(computedBind);
          this.ComputeAction(computedBind, false);
          goto label_1;
        }
      }
    }

    private bool IsKeyDown(ActionGroup bind)
    {
      return (!this.debug.IsDebugging ? Input.GetKeyDown(bind.Key) : InputUtility.IsKeyDown(bind.Key) || InputUtility.IsKeyDown(bind.Key, KeyModifficator.Shift) || InputUtility.IsKeyDown(bind.Key, KeyModifficator.Alt)) | InputService.Instance.GetButtonDown(bind.Joystick, bind.JoystickHold);
    }

    private bool IsKeyRelease(ActionGroup bind)
    {
      return !Input.GetKey(bind.Key) && !InputService.Instance.GetButton(bind.Joystick, bind.JoystickHold);
    }

    private void FireKeyPressed(KeyCode code)
    {
      if (!this.initialise)
        throw new Exception(this.GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
      this.OnKeyPressed(code);
    }

    private bool ComputeAction(ActionGroup group, bool down)
    {
      if (!this.initialise)
        throw new Exception(this.GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
      foreach (GameActionType action in group.Actions)
      {
        if (this.TryGetListeners(action, this.tmpListeners))
        {
          foreach (GameActionHandle tmpListener in this.tmpListeners)
          {
            if (tmpListener(action, down))
            {
              if (down)
              {
                Action<GameActionType> onGameAction = this.OnGameAction;
                if (onGameAction != null)
                  onGameAction(action);
              }
              return true;
            }
          }
        }
      }
      return false;
    }

    private void ComputeAction(ActionGroup group, bool down, HashSet<GameActionType> result)
    {
      foreach (GameActionType action in group.Actions)
        result.Add(action);
    }

    private void OnApplicationFocusEvent(bool focus)
    {
      if (!this.initialise)
        throw new Exception(this.GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
      if (focus)
        return;
      this.ResetKeys();
    }

    private void ResetKeys()
    {
      if (!this.initialise)
        throw new Exception(this.GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
      foreach (ActionGroup computedBind in this.computedBinds)
        this.ComputeAction(computedBind, false);
      this.computedBinds.Clear();
    }

    private bool TryGetListeners(GameActionType type, List<GameActionHandle> listeners)
    {
      listeners.Clear();
      List<GameActionHandle> collection;
      if (!this.actions.TryGetValue(type, out collection))
        return false;
      listeners.AddRange((IEnumerable<GameActionHandle>) collection);
      return true;
    }
  }
}
