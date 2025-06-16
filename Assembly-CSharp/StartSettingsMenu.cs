using System;
using System.Collections.Generic;
using Engine.Common.Services;
using Engine.Source.Services.Inputs;
using Engine.Source.UI;
using InputServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StartSettingsMenu : MonoBehaviour {
	[SerializeField] private Button languageButton;
	[SerializeField] private Button difficultyButton;
	[SerializeField] private Button displayButton;
	[SerializeField] private Button graphicsButton;
	[SerializeField] private Button controlButton;
	[SerializeField] private Button keysButton;
	[SerializeField] private Button soundButton;
	[SerializeField] private Button backerUnlocksButton;
	[SerializeField] private Button xboxLiveButton;
	[SerializeField] private Image selectedLine;
	[SerializeField] private GameObject toolTip;
	private static int currentIndex;
	private static int selectedIndex;
	private int bufferedViewIndex = -1;
	private Button[] buttons;

	private void Awake() {
		languageButton.onClick.AddListener(SettingsMenuHelper.Instatnce.ShowSettings<IStartLanguageSettingsWindow>);
		difficultyButton.onClick.AddListener(SettingsMenuHelper.Instatnce.ShowSettings<IStartDifficultySettingsWindow>);
		displayButton.onClick.AddListener(SettingsMenuHelper.Instatnce.ShowSettings<IStartDisplaySettingsWindow>);
		graphicsButton.onClick.AddListener(SettingsMenuHelper.Instatnce.ShowSettings<IStartGraphicsSettingsWindow>);
		controlButton.onClick.AddListener(SettingsMenuHelper.Instatnce.ShowSettings<IStartControlSettingsWindow>);
		keysButton.onClick.AddListener(SettingsMenuHelper.Instatnce.ShowSettings<IStartKeySettingsWindow>);
		soundButton.onClick.AddListener(SettingsMenuHelper.Instatnce.ShowSettings<IStartSoundSettingsWindow>);
		backerUnlocksButton.onClick.AddListener(SettingsMenuHelper.Instatnce.ShowSettings<IStartBackerUnlocksWindow>);
		ShowSelectableButton();
	}

	private void ShowSelectableButton() {
		var componentInParent = GetComponentInParent<IWindow>();
		languageButton.interactable = !(componentInParent is IStartLanguageSettingsWindow);
		difficultyButton.interactable = !(componentInParent is IStartDifficultySettingsWindow);
		displayButton.interactable = !(componentInParent is IStartDisplaySettingsWindow);
		graphicsButton.interactable = !(componentInParent is IStartGraphicsSettingsWindow);
		controlButton.interactable = !(componentInParent is IStartControlSettingsWindow);
		keysButton.interactable = !(componentInParent is IStartKeySettingsWindow);
		soundButton.interactable = !(componentInParent is IStartSoundSettingsWindow);
		backerUnlocksButton.interactable = !(componentInParent is IStartBackerUnlocksWindow);
	}

	private void OnJoystick(bool isUsed) {
		buttons = new List<Button>(GetComponentsInChildren<Button>()).FindAll(b => b.gameObject.activeInHierarchy)
			.ToArray();
		if (isUsed) {
			toolTip.SetActive(!SettingsMenuHelper.Instatnce.isSelected);
			for (var index = 0; index < buttons.Length; ++index) {
				if (!buttons[index].interactable)
					selectedIndex = index;
				buttons[index].interactable = true;
			}

			currentIndex = selectedIndex;
			ChangeSelection();
		} else {
			if (bufferedViewIndex != -1) {
				Select(GameActionType.Submit, true);
				bufferedViewIndex = -1;
			}

			ShowSelectableButton();
		}

		selectedLine.gameObject.SetActive(isUsed);
	}

	private bool RefreshCurrentIndex(GameActionType type, bool down) {
		currentIndex = 0;
		return false;
	}

	private bool ChangeSelectedItem(GameActionType type, bool down) {
		if ((type == GameActionType.LStickUp) & down) {
			--currentIndex;
			ChangeSelection();
			return true;
		}

		if (!((type == GameActionType.LStickDown) & down))
			return false;
		++currentIndex;
		ChangeSelection();
		return true;
	}

	private void ChangeSelection() {
		buttons = new List<Button>(GetComponentsInChildren<Button>()).FindAll(b => b.gameObject.activeInHierarchy)
			.ToArray();
		if (currentIndex > buttons.Length - 1)
			currentIndex = 0;
		if (currentIndex < 0)
			currentIndex = buttons.Length - 1;
		bufferedViewIndex = currentIndex;
		EventSystem.current.SetSelectedGameObject(buttons[currentIndex].gameObject);
		ChangeLinePosition();
	}

	private bool Select(GameActionType type, bool down) {
		if (!down)
			return false;
		if (selectedIndex == currentIndex) {
			SettingsMenuHelper.Instatnce.SetSelectedState();
			return false;
		}

		ChangeLinePosition();
		var eventData = new PointerEventData(EventSystem.current);
		ExecuteEvents.Execute(buttons[currentIndex].gameObject, eventData, ExecuteEvents.submitHandler);
		selectedIndex = currentIndex;
		SettingsMenuHelper.Instatnce.SetSelectedState();
		return true;
	}

	private void ChangeLinePosition() {
		selectedLine.transform.SetParent(buttons[currentIndex].transform, false);
		selectedLine.rectTransform.anchoredPosition = selectedLine.rectTransform.anchoredPosition with {
			y = 0.0f
		};
		selectedLine.rectTransform.sizeDelta = selectedLine.rectTransform.sizeDelta with {
			x = buttons[currentIndex].GetComponentInChildren<Text>().preferredWidth
		};
	}

	public void OnEnable() {
		CursorService.Instance.Free = true;
		if (!InputService.Instance.JoystickUsed)
			CursorService.Instance.Visible = true;
		var service = ServiceLocator.GetService<GameActionService>();
		service.AddListener(GameActionType.LStickDown, ChangeSelectedItem, true);
		service.AddListener(GameActionType.LStickUp, ChangeSelectedItem, true);
		service.AddListener(GameActionType.Submit, Select, true);
		SettingsMenuHelper.Instatnce.Activate(true);
		SettingsMenuHelper.Instatnce.OnStateSelected += OnStateSelected;
		InputService.Instance.onJoystickUsedChanged += OnJoystick;
		CoroutineService.Instance.WaitFrame(1, (Action)(() => {
			OnJoystick(InputService.Instance.JoystickUsed);
			ChangeLinePosition();
		}));
	}

	public void OnDisable() {
		var service = ServiceLocator.GetService<GameActionService>();
		service.RemoveListener(GameActionType.LStickDown, ChangeSelectedItem);
		service.RemoveListener(GameActionType.LStickUp, ChangeSelectedItem);
		service.RemoveListener(GameActionType.Submit, Select);
		SettingsMenuHelper.Instatnce.OnStateSelected -= OnStateSelected;
		InputService.Instance.onJoystickUsedChanged -= OnJoystick;
		SettingsMenuHelper.Instatnce.Activate(false);
		CoroutineService.Instance.WaitFrame(1, (Action)(() => {
			if (SettingsMenuHelper.Instatnce.isSelected)
				return;
			currentIndex = 0;
			selectedIndex = 0;
		}));
		selectedLine.gameObject.SetActive(false);
	}

	private void OnStateSelected(bool isSelected) {
		var service = ServiceLocator.GetService<GameActionService>();
		toolTip?.SetActive(!isSelected);
		if (isSelected) {
			service.RemoveListener(GameActionType.LStickDown, ChangeSelectedItem);
			service.RemoveListener(GameActionType.LStickUp, ChangeSelectedItem);
			service.RemoveListener(GameActionType.Submit, Select);
		} else {
			service.AddListener(GameActionType.LStickDown, ChangeSelectedItem, true);
			service.AddListener(GameActionType.LStickUp, ChangeSelectedItem, true);
			service.AddListener(GameActionType.Submit, Select);
		}
	}
}