﻿using System.Collections.Generic;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Services.Inputs;
using Engine.Source.Settings;
using InputServices;
using UnityEngine;
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
    private List<KeySettingsItemView> items = [];
    private SelectableSettingsItemView listeningItem;

    protected override void Awake()
    {
      gameActionService = ServiceLocator.GetService<GameActionService>();
      layout = Instantiate(listLayoutPrefab, transform, false);
      joystickLayout = Instantiate(namedIntValueViewPrefab, layout.Content, false);
      joystickLayout.SetName("{UI.Pingle.KeyMaps.ChangeKeyMap}");
      joystickLayout.SetValueNames([
        "{UI.Pingle.PresetA}",
        "{UI.Pingle.PresetB}",
        "{UI.Pingle.PresetC}"
      ]);
      joystickLayout.SetSetting(InstanceByRequest<InputGameSetting>.Instance.JoystickLayout);
      buttonCancel.onClick.AddListener(CancelSelection);
      base.Awake();
    }

    protected override void OnDisable()
    {
      base.OnDisable();
      gameActionService.RemoveListener(GameActionType.Cancel, OnCancelPressed);
      gameActionService.OnKeyPressed -= OnKeyPressed;
      Clear();
      gameActionService.OnBindsChange -= OnBindsChange;
      NamedIntSettingsValueView joystickLayout = this.joystickLayout;
      joystickLayout.VisibleValueChangeEvent = joystickLayout.VisibleValueChangeEvent - JoystickLayoutSwitcher.Instance.OnAutoJoystickValueChange;
    }

    protected override void OnJoystick(bool isUsed)
    {
      base.OnJoystick(isUsed);
      joystickLayout.gameObject.SetActive(isUsed);
      if (isUsed)
      {
        CancelSelection();
        SelectItem(0);
        gameActionService.OnKeyPressed -= OnKeyPressed;
      }
      else
        gameActionService.OnKeyPressed += OnKeyPressed;
    }

    protected override void OnEnable()
    {
      Fill();
      gameActionService.AddListener(GameActionType.Cancel, OnCancelPressed);
      base.OnEnable();
      gameActionService.OnBindsChange += OnBindsChange;
      NamedIntSettingsValueView joystickLayout = this.joystickLayout;
      joystickLayout.VisibleValueChangeEvent = joystickLayout.VisibleValueChangeEvent + JoystickLayoutSwitcher.Instance.OnAutoJoystickValueChange;
    }

    private void OnBindsChange()
    {
      Clear();
      Fill();
      LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

    public void Fill()
    {
      foreach (ActionGroup bind in gameActionService.GetBinds())
      {
        if (!bind.Hide)
        {
          KeySettingsItemView settingsItemView = Instantiate(selectableViewPrefab, layout.Content, false);
          items.Add(settingsItemView);
          settingsItemView.GameActionGroup = bind;
          if (bind.IsChangeble)
            settingsItemView.Selectable.ClickEvent += OnItemClicked;
          InputService.Instance.onJoystickUsedChanged += settingsItemView.OnJoystickUsedChanged;
        }
      }
      joystickLayout.RevertVisibleValue();
    }

    public void Clear()
    {
      SelectItem(null);
      for (int index = 0; index < items.Count; ++index)
      {
        InputService.Instance.onJoystickUsedChanged -= items[index].OnJoystickUsedChanged;
        items[index].Selectable.ClickEvent -= OnItemClicked;
        Destroy(items[index].gameObject);
      }
      items.Clear();
    }

    protected override void OnButtonReset()
    {
      if (InputService.Instance.JoystickUsed)
      {
        InputGameSetting instance = InstanceByRequest<InputGameSetting>.Instance;
        instance.JoystickLayout.Value = 0;
        instance.Apply();
        CoroutineService.Instance.WaitFrame(() => SelectItem(0));
      }
      else
        gameActionService.ResetAllRebinds();
    }

    private void SelectItem(SelectableSettingsItemView item)
    {
      if (listeningItem == item)
        return;
      if (listeningItem != null)
        listeningItem.Selected = false;
      listeningItem = item;
      if (listeningItem != null)
      {
        listeningItem.Selected = true;
        buttonCancel.gameObject.SetActive(true);
        buttonBack.gameObject.SetActive(false);
      }
      else
      {
        buttonCancel.gameObject.SetActive(false);
        buttonBack.gameObject.SetActive(true);
      }
    }

    private void OnItemClicked(SelectableSettingsItemView item) => SelectItem(item);

    private void OnKeyPressed(KeyCode code)
    {
      if (!IsWaiting)
        return;
      gameActionService.AddRebind(listeningItem.GetComponent<KeySettingsItemView>().GameActionGroup, code);
    }

    private bool OnCancelPressed(GameActionType type, bool down)
    {
      if (!down || !IsWaiting)
        return false;
      CancelSelection();
      return true;
    }

    private void CancelSelection() => SelectItem(null);

    public bool IsWaiting => listeningItem != null;
  }
}
