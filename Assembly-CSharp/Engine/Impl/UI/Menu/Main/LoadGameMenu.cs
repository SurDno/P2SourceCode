using System;
using System.Collections;
using System.Collections.Generic;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Impl.UI.Controls;
using Engine.Source.Commons;
using Engine.Source.Services;
using Engine.Source.Services.Inputs;
using Engine.Source.Services.Profiles;
using Engine.Source.UI.Menu.Main;
using InputServices;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Engine.Impl.UI.Menu.Main;

public class LoadGameMenu : MonoBehaviour {
	[SerializeField] private LayoutContainer listLayoutPrefab;
	[SerializeField] private SelectableSettingsItemView selectableViewPrefab;
	[SerializeField] private Button loadButton;
	[SerializeField] private Button deleteButton;
	[SerializeField] private HideableView hasSelectedView;
	[SerializeField] private StringView selectedFileView;
	[SerializeField] private ConfirmationWindow confirmationPrefab;
	[SerializeField] private EventView viewChangeProfile;
	[SerializeField] private EventView viewBack;
	[SerializeField] private GameObject removeTipObject;
	[SerializeField] private GameObject back;
	[SerializeField] private GameObject changeProfile;
	private ScrollRect scroll;
	private ProfilesService profilesService;
	private LayoutContainer layout;
	private List<SelectableSettingsItemView> items = new();
	private SelectableSettingsItemView selectedItem;
	private ConfirmationWindow confirmationInstance;
	private int currentSelected;
	private ControlSwitcher _controlSwitcher = new();
	private bool canLoad;
	private bool canDelete;
	private bool canChangeProfile;
	private Coroutine scrollCoroutine;

	private void Awake() {
		profilesService = ServiceLocator.GetService<ProfilesService>();
		layout = Instantiate(listLayoutPrefab, transform, false);
		scroll = layout.transform.GetChild(0).GetComponent<ScrollRect>();
	}

	private void Subscribe() {
		_controlSwitcher.SubmitAction(loadButton, GameActionType.Submit, TryLoadSelected);
		_controlSwitcher.SubmitAction(deleteButton, GameActionType.Split, TryDeleteSelected);
	}

	private bool SelectPrevious(GameActionType type, bool down) {
		if (down) {
			if (scrollCoroutine != null)
				StopCoroutine(scrollCoroutine);
			SelectItem(items[currentSelected > 0 ? currentSelected - 1 : items.Count - 1]);
			scrollCoroutine = StartCoroutine(ScrollCoroutine(true));
		} else if (scrollCoroutine != null)
			StopCoroutine(scrollCoroutine);

		return true;
	}

	private bool SelectNext(GameActionType type, bool down) {
		if (down) {
			if (scrollCoroutine != null)
				StopCoroutine(scrollCoroutine);
			SelectItem(items[currentSelected < items.Count - 1 ? currentSelected + 1 : 0]);
			scrollCoroutine = StartCoroutine(ScrollCoroutine(false));
		} else if (scrollCoroutine != null)
			StopCoroutine(scrollCoroutine);

		return true;
	}

	private IEnumerator ScrollCoroutine(bool isUp) {
		yield return new WaitForSeconds(0.5f);
		while (true) {
			var sellected = !isUp ? currentSelected < items.Count - 1 ? currentSelected + 1 : 0 :
				currentSelected > 0 ? currentSelected - 1 : items.Count - 1;
			SelectItem(items[sellected]);
			yield return new WaitForSeconds(0.05f);
		}
	}

	private bool LoadSelected(GameActionType type, bool down) {
		if (down && InputService.Instance.JoystickUsed)
			LoadSelected();
		return true;
	}

	private void OnDisable() {
		if (confirmationInstance != null)
			confirmationInstance.Hide();
		Clear();
		_controlSwitcher.Dispose();
		ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.LStickUp, SelectPrevious);
		ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.LStickDown, SelectNext);
		ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Cancel, Back);
	}

	private bool Back(GameActionType type, bool down) {
		if (!down)
			return false;
		if (canChangeProfile)
			viewChangeProfile.Invoke();
		else
			viewBack.Invoke();
		return true;
	}

	private void OnEnable() {
		Fill();
		Subscribe();
		ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.LStickUp, SelectPrevious);
		ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.LStickDown, SelectNext);
		ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Cancel, Back, true);
		canChangeProfile = !GameLoadGameWindow.IsActive;
		changeProfile.SetActive(canChangeProfile);
		back.SetActive(!canChangeProfile);
	}

	public void Fill() {
		var current = profilesService.Current;
		if (current == null)
			return;
		var saveNames = ProfilesUtility.GetSaveNames(current.Name);
		var lastSave = current.LastSave;
		SelectableSettingsItemView settingsItemView1 = null;
		foreach (var str1 in saveNames) {
			var str2 = ProfilesUtility.ConvertSaveName(str1);
			var settingsItemView2 = Instantiate(selectableViewPrefab, layout.Content, false);
			items.Add(settingsItemView2);
			if (str1 == lastSave) {
				str2 = str2 + " " + ServiceLocator.GetService<LocalizationService>().GetText("{UI.CurrentSave}");
				settingsItemView1 = settingsItemView2;
			}

			settingsItemView2.SetName(str2);
			settingsItemView2.ClickEvent += OnClickItem;
			settingsItemView2.name = str1;
			var saveCreationTime = ProfilesUtility.GetSaveCreationTime(current.Name, str1);
			settingsItemView2.SetValue(ProfilesUtility.ConvertCreationTime(saveCreationTime, "{SaveDateTimeFormat}"));
		}

		SelectItem(settingsItemView1);
	}

	public void Clear() {
		SelectItem(null);
		for (var index = 0; index < items.Count; ++index) {
			items[index].GetComponent<SelectableSettingsItemView>().ClickEvent -= OnClickItem;
			Destroy(items[index].gameObject);
		}

		items.Clear();
	}

	private void TryLoadSelected() {
		if (InstanceByRequest<EngineApplication>.Instance.ViewEnabled)
			ShowConfirmation("{UI.Menu.Main.Save.LoadConfirmation}", LoadSelected);
		else
			LoadSelected();
	}

	private void LoadSelected() {
		ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.LStickUp, SelectPrevious, true);
		ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.LStickDown, SelectNext, true);
		if (!(bool)(Object)selectedItem || !canLoad)
			return;
		var current = profilesService.Current;
		if (current == null)
			Debug.LogError("Profile not found");
		else {
			var name = selectedItem.name;
			var str = ProfilesUtility.SavePath(current.Name, name);
			if (!ProfilesUtility.IsSaveExist(str))
				Debug.LogError("Save name not found : " + name);
			else
				CoroutineService.Instance.Route(LoadGameUtility.RestartGameWithSave(str));
		}
	}

	private void TryDeleteSelected() {
		if (!canDelete)
			return;
		ShowConfirmation("{UI.Menu.Main.Save.DeleteConfirmation}", DeleteSelected);
	}

	private void DeleteSelected() {
		ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.LStickUp, SelectPrevious, true);
		ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.LStickDown, SelectNext, true);
		if (!(bool)(Object)selectedItem || !canDelete)
			return;
		var selectedIndex = 0;
		for (var index = 0; index < items.Count; ++index)
			if (items[index] == selectedItem) {
				selectedIndex = index;
				break;
			}

		profilesService.DeleteSave(selectedItem.name);
		Clear();
		Fill();
		CoroutineService.Instance.WaitFrame(1, (Action)(() => {
			LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
			if (items.Count <= 0)
				return;
			selectedIndex = Mathf.Min(selectedIndex, items.Count - 1);
			SelectItem(items[selectedIndex]);
		}));
	}

	private void OnClickItem(SelectableSettingsItemView item) {
		if (item == selectedItem)
			TryLoadSelected();
		else
			SelectItem(item);
	}

	private void SelectItem(SelectableSettingsItemView item) {
		if (item == this.selectedItem)
			return;
		if ((bool)(Object)this.selectedItem)
			this.selectedItem.Selected = false;
		this.selectedItem = item;
		var selectedItem = this.selectedItem;
		if ((bool)(Object)selectedItem) {
			this.selectedItem.Selected = true;
			selectedFileView.StringValue = this.selectedItem.name;
		}

		loadButton.interactable = (bool)(Object)selectedItem;
		deleteButton.interactable = (bool)(Object)selectedItem;
		canLoad = (bool)(Object)selectedItem;
		canDelete = (bool)(Object)selectedItem;
		removeTipObject.SetActive(canDelete);
		hasSelectedView.Visible = (bool)(Object)selectedItem;
		currentSelected = items.IndexOf(item);
		FillForSelected(currentSelected);
	}

	public void FillForSelected(int index) {
		var height = layout.Content.parent.parent.GetComponent<RectTransform>().rect.height;
		var num1 = items[0].GetComponent<RectTransform>().rect.height + 1f;
		var num2 = (index + 2) * num1;
		var component = layout.Content.parent.GetComponent<RectTransform>();
		var anchoredPosition = component.anchoredPosition;
		if (num2 - (double)anchoredPosition.y > height)
			anchoredPosition.y = num2 + (double)anchoredPosition.y > height ? num2 - height : 0.0f;
		else if (num2 - num1 * 2.0 - anchoredPosition.y < 0.0)
			anchoredPosition.y = num2 - num1 * 2f;
		component.anchoredPosition = anchoredPosition;
	}

	private void ShowConfirmation(string text, Action onAccept) {
		if (confirmationInstance == null)
			confirmationInstance = Instantiate(confirmationPrefab, transform, false);
		confirmationInstance.Show(text, onAccept, (Action)(() => {
			ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.LStickUp, SelectPrevious, true);
			ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.LStickDown, SelectNext, true);
		}));
		ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.LStickUp, SelectPrevious);
		ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.LStickDown, SelectNext);
	}
}