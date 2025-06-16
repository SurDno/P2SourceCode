using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Services.Inputs;
using Engine.Source.Settings;
using InputServices;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Engine.Impl.UI.Menu.Main
{
  public class KeySettingsView : SettingsView
  {
    [SerializeField]
    private KeySettingsItemView selectableViewPrefab;
    [SerializeField]
    private Button buttonCancel;
    [SerializeField]
    private Button buttonBack;
    private GameActionService gameActionService;
    private NamedIntSettingsValueView joystickLayout;
    private List<KeySettingsItemView> items = new List<KeySettingsItemView>();
    private SelectableSettingsItemView listeningItem = (SelectableSettingsItemView) null;

    protected override void Awake()
    {
      this.gameActionService = ServiceLocator.GetService<GameActionService>();
      this.layout = UnityEngine.Object.Instantiate<LayoutContainer>(this.listLayoutPrefab, this.transform, false);
      this.joystickLayout = UnityEngine.Object.Instantiate<NamedIntSettingsValueView>(this.namedIntValueViewPrefab, (Transform) this.layout.Content, false);
      this.joystickLayout.SetName("{UI.Pingle.KeyMaps.ChangeKeyMap}");
      this.joystickLayout.SetValueNames(new string[3]
      {
        "{UI.Pingle.PresetA}",
        "{UI.Pingle.PresetB}",
        "{UI.Pingle.PresetC}"
      });
      this.joystickLayout.SetSetting(InstanceByRequest<InputGameSetting>.Instance.JoystickLayout);
      this.buttonCancel.onClick.AddListener(new UnityAction(this.CancelSelection));
      base.Awake();
    }

    protected override void OnDisable()
    {
      base.OnDisable();
      this.gameActionService.RemoveListener(GameActionType.Cancel, new GameActionHandle(this.OnCancelPressed));
      this.gameActionService.OnKeyPressed -= new Action<KeyCode>(this.OnKeyPressed);
      this.Clear();
      this.gameActionService.OnBindsChange -= new Action(this.OnBindsChange);
      NamedIntSettingsValueView joystickLayout = this.joystickLayout;
      joystickLayout.VisibleValueChangeEvent = joystickLayout.VisibleValueChangeEvent - new Action<SettingsValueView<int>>(JoystickLayoutSwitcher.Instance.OnAutoJoystickValueChange<int>);
    }

    protected override void OnJoystick(bool isUsed)
    {
      base.OnJoystick(isUsed);
      this.joystickLayout.gameObject.SetActive(isUsed);
      if (isUsed)
      {
        this.CancelSelection();
        this.SelectItem(0);
        this.gameActionService.OnKeyPressed -= new Action<KeyCode>(this.OnKeyPressed);
      }
      else
        this.gameActionService.OnKeyPressed += new Action<KeyCode>(this.OnKeyPressed);
    }

    protected override void OnEnable()
    {
      this.Fill();
      this.gameActionService.AddListener(GameActionType.Cancel, new GameActionHandle(this.OnCancelPressed));
      base.OnEnable();
      this.gameActionService.OnBindsChange += new Action(this.OnBindsChange);
      NamedIntSettingsValueView joystickLayout = this.joystickLayout;
      joystickLayout.VisibleValueChangeEvent = joystickLayout.VisibleValueChangeEvent + new Action<SettingsValueView<int>>(JoystickLayoutSwitcher.Instance.OnAutoJoystickValueChange<int>);
    }

    private void OnBindsChange()
    {
      this.Clear();
      this.Fill();
      LayoutRebuilder.ForceRebuildLayoutImmediate(this.GetComponent<RectTransform>());
    }

    public void Fill()
    {
      foreach (ActionGroup bind in this.gameActionService.GetBinds())
      {
        if (!bind.Hide)
        {
          KeySettingsItemView settingsItemView = UnityEngine.Object.Instantiate<KeySettingsItemView>(this.selectableViewPrefab, (Transform) this.layout.Content, false);
          this.items.Add(settingsItemView);
          settingsItemView.GameActionGroup = bind;
          if (bind.IsChangeble)
            settingsItemView.Selectable.ClickEvent += new Action<SelectableSettingsItemView>(this.OnItemClicked);
          InputService.Instance.onJoystickUsedChanged += new Action<bool>(settingsItemView.OnJoystickUsedChanged);
        }
      }
      this.joystickLayout.RevertVisibleValue();
    }

    public void Clear()
    {
      this.SelectItem((SelectableSettingsItemView) null);
      for (int index = 0; index < this.items.Count; ++index)
      {
        InputService.Instance.onJoystickUsedChanged -= new Action<bool>(this.items[index].OnJoystickUsedChanged);
        this.items[index].Selectable.ClickEvent -= new Action<SelectableSettingsItemView>(this.OnItemClicked);
        UnityEngine.Object.Destroy((UnityEngine.Object) this.items[index].gameObject);
      }
      this.items.Clear();
    }

    protected override void OnButtonReset()
    {
      if (InputService.Instance.JoystickUsed)
      {
        InputGameSetting instance = InstanceByRequest<InputGameSetting>.Instance;
        instance.JoystickLayout.Value = 0;
        instance.Apply();
        CoroutineService.Instance.WaitFrame((Action) (() => this.SelectItem(0)));
      }
      else
        this.gameActionService.ResetAllRebinds();
    }

    private void SelectItem(SelectableSettingsItemView item)
    {
      if ((UnityEngine.Object) this.listeningItem == (UnityEngine.Object) item)
        return;
      if ((UnityEngine.Object) this.listeningItem != (UnityEngine.Object) null)
        this.listeningItem.Selected = false;
      this.listeningItem = item;
      if ((UnityEngine.Object) this.listeningItem != (UnityEngine.Object) null)
      {
        this.listeningItem.Selected = true;
        this.buttonCancel.gameObject.SetActive(true);
        this.buttonBack.gameObject.SetActive(false);
      }
      else
      {
        this.buttonCancel.gameObject.SetActive(false);
        this.buttonBack.gameObject.SetActive(true);
      }
    }

    private void OnItemClicked(SelectableSettingsItemView item) => this.SelectItem(item);

    private void OnKeyPressed(KeyCode code)
    {
      if (!this.IsWaiting)
        return;
      this.gameActionService.AddRebind(this.listeningItem.GetComponent<KeySettingsItemView>().GameActionGroup, code);
    }

    private bool OnCancelPressed(GameActionType type, bool down)
    {
      if (!down || !this.IsWaiting)
        return false;
      this.CancelSelection();
      return true;
    }

    private void CancelSelection() => this.SelectItem((SelectableSettingsItemView) null);

    public bool IsWaiting => (UnityEngine.Object) this.listeningItem != (UnityEngine.Object) null;
  }
}
