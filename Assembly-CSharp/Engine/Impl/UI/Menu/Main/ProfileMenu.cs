using System;
using System.Collections;
using System.Collections.Generic;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Impl.UI.Controls;
using Engine.Source.Services;
using Engine.Source.Services.Inputs;
using Engine.Source.Services.Profiles;
using Engine.Source.UI;
using InputServices;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Engine.Impl.UI.Menu.Main;

public class ProfileMenu : MonoBehaviour {
	[SerializeField] private LayoutContainer listLayoutPrefab;
	[SerializeField] private SelectableSettingsItemView selectableViewPrefab;
	[SerializeField] private Button loadButton;
	[SerializeField] private Button deleteButton;
	[SerializeField] private HideableView hasSelectedView;
	[SerializeField] private StringView selectedFileView;
	[SerializeField] private ConfirmationWindow confirmationPrefab;
	[SerializeField] private List<ProfileData> profiles;
	[SerializeField] private GameObject removeTipObject;
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
	private Coroutine scrollCoroutine;

	private void Awake() {
		profilesService = ServiceLocator.GetService<ProfilesService>();
		layout = Instantiate(listLayoutPrefab, transform, false);
		scroll = layout.transform.GetChild(0).GetComponent<ScrollRect>();
	}

	private void Subscribe() {
		_controlSwitcher.SubmitAction(loadButton, GameActionType.Submit, LoadSelected);
		_controlSwitcher.SubmitAction(deleteButton, GameActionType.Split, TryDeleteSelected);
	}

	private bool SelectPrevious(GameActionType type, bool down) {
		if (down) {
			SelectItem(items[currentSelected > 0 ? currentSelected - 1 : items.Count - 1]);
			scrollCoroutine = StartCoroutine(ScrollCoroutine(true));
		} else if (scrollCoroutine != null)
			StopCoroutine(scrollCoroutine);

		return true;
	}

	private bool SelectNext(GameActionType type, bool down) {
		if (down) {
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

	private void TryDeleteSelected() {
		if (!canDelete)
			return;
		ShowConfirmation("{UI.Menu.Main.Profile.DeleteConfirmation}", DeleteSelected);
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

		profilesService.DeleteProfile(selectedItem.name);
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

	public void Clear() {
		SelectItem(null);
		for (var index = 0; index < items.Count; ++index) {
			items[index].GetComponent<SelectableSettingsItemView>().ClickEvent -= OnClickItem;
			Destroy(items[index].gameObject);
		}

		items.Clear();
	}

	public void Fill() {
		var current = profilesService.Current;
		SelectableSettingsItemView settingsItemView1 = null;
		foreach (var profile in profilesService.Profiles)
			profiles.Add(profile);
		for (var index = profiles.Count - 1; index >= 0; --index) {
			var profile = profiles[index];
			var settingsItemView2 = Instantiate(selectableViewPrefab, layout.Content, false);
			items.Add(settingsItemView2);
			var str = ProfilesUtility.ConvertProfileName(profile.Name, "{UI.Menu.Main.Profile.LongFormat}");
			if (profile == current) {
				str = str + " " + ServiceLocator.GetService<LocalizationService>().GetText("{UI.CurrentProfile}");
				settingsItemView1 = settingsItemView2;
			}

			settingsItemView2.SetName(str);
			settingsItemView2.ClickEvent += OnClickItem;
			settingsItemView2.name = profile.Name;
			var lastSave = ProfilesUtility.GetLastSave(profile.Name);
			if (lastSave != "") {
				var saveCreationTime = ProfilesUtility.GetSaveCreationTime(profile.Name, lastSave);
				if (saveCreationTime != DateTime.MinValue)
					settingsItemView2.SetValue(
						ProfilesUtility.ConvertCreationTime(saveCreationTime, "{SaveDateTimeFormat}"));
				else
					settingsItemView2.SetValue("");
			} else
				settingsItemView2.SetValue("");
		}

		profiles.Clear();
		SelectItem(settingsItemView1);
	}

	private void LoadSelected() {
		if (!(bool)(Object)selectedItem || !canLoad)
			return;
		profilesService.SetCurrent(selectedItem.name);
		ServiceLocator.GetService<UIService>().Swap<IStartLoadGameWindow>();
	}

	private void OnDisable() {
		if (confirmationInstance != null)
			confirmationInstance.Hide();
		Clear();
		profiles = null;
		ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.LStickUp, SelectPrevious);
		ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.LStickDown, SelectNext);
		_controlSwitcher.Dispose();
	}

	private void OnEnable() {
		profiles = new List<ProfileData>();
		Fill();
		Subscribe();
		ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.LStickUp, SelectPrevious);
		ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.LStickDown, SelectNext);
	}

	private void OnClickItem(SelectableSettingsItemView item) {
		if (item == selectedItem)
			LoadSelected();
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
		var flag = false;
		if ((bool)(Object)selectedItem) {
			this.selectedItem.Selected = true;
			selectedFileView.StringValue = this.selectedItem.name;
			flag = profilesService.Current.Name == this.selectedItem.name;
		}

		loadButton.interactable = (bool)(Object)selectedItem;
		canLoad = (bool)(Object)selectedItem;
		deleteButton.interactable = (bool)(Object)selectedItem && !flag;
		canDelete = (bool)(Object)selectedItem && !flag;
		removeTipObject.SetActive(canDelete);
		hasSelectedView.Visible = (bool)(Object)selectedItem;
		currentSelected = items.IndexOf(item);
		FillForSelected(currentSelected);
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
}