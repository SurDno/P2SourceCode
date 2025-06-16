using System.Collections.Generic;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Impl.UI.Menu;
using Engine.Source.Commons;
using Engine.Source.Services;
using Engine.Source.Services.CameraServices;
using Engine.Source.Services.Inputs;
using Engine.Source.Services.Profiles;
using InputServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Engine.Source.UI.Menu.Main;

public abstract class ProfileWindow : UIWindow {
	private CameraKindEnum lastCameraKind;

	[Header("Sounds")] [SerializeField] [FormerlySerializedAs("ClickSound")]
	private AudioClip clickSound;

	[SerializeField] private ProfileItem itemPrefab;
	[SerializeField] private RectTransform keyView;
	[SerializeField] private Button currentButton;
	[SerializeField] private Button deleteButton;
	private ProfileItem selection;
	private List<ProfileItem> items = new();

	protected abstract void RegisterLayer();

	public override void Initialize() {
		RegisterLayer();
		var componentsInChildren = GetComponentsInChildren<Button>(true);
		for (var index = 0; index < componentsInChildren.Length; ++index) {
			componentsInChildren[index].gameObject.AddComponent<EventTrigger>();
			var entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerClick;
			entry.callback.AddListener(eventData => Button_Click_Handler());
			componentsInChildren[index].gameObject.GetComponent<EventTrigger>().triggers.Add(entry);
		}

		base.Initialize();
	}

	private void Fill() {
		var text1 = ServiceLocator.GetService<LocalizationService>().GetText("{UI.CurrentProfile}");
		var service = ServiceLocator.GetService<ProfilesService>();
		var current = service.Current;
		foreach (var profile in service.Profiles) {
			var profileItem = Instantiate(itemPrefab);
			profileItem.transform.SetParent(keyView, false);
			var component = profileItem.GetComponent<ProfileItem>();
			items.Add(component);
			var text2 = profile.Name + "  [" + ProfilesUtility.GetSaveCount(profile.Name) + "]  ";
			if (profile == current)
				text2 += text1;
			component.Name = profile.Name;
			component.SetText(text2);
			component.OnPressed += OnKeyItemPressed;
		}

		UpdateButtons();
	}

	private void Clear() {
		selection = null;
		foreach (var profileItem in items) {
			profileItem.OnPressed -= OnKeyItemPressed;
			Destroy(profileItem.gameObject);
		}

		items.Clear();
	}

	private void OnKeyItemPressed(ProfileItem item) {
		if (selection != null)
			selection.Slection = false;
		selection = item;
		selection.Slection = true;
		UpdateButtons();
	}

	public void Button_Click_Handler() {
		if (!gameObject.activeInHierarchy)
			return;
		gameObject.GetComponent<AudioSource>().PlayOneShot(clickSound);
	}

	public void Button_Back_Click_Handler() {
		ServiceLocator.GetService<UIService>().Pop();
	}

	public void Button_Current_Click_Handler() {
		if (selection == null)
			return;
		ServiceLocator.GetService<ProfilesService>().SetCurrent(selection.Name);
		Clear();
		Fill();
	}

	public void Button_Delete_Click_Handler() {
		if (selection == null)
			return;
		ServiceLocator.GetService<ProfilesService>().DeleteProfile(selection.Name);
		Clear();
		Fill();
	}

	protected override void OnEnable() {
		base.OnEnable();
		Fill();
		lastCameraKind = ServiceLocator.GetService<CameraService>().Kind;
		ServiceLocator.GetService<CameraService>().Kind = CameraKindEnum.Unknown;
		InstanceByRequest<EngineApplication>.Instance.IsPaused = true;
		CursorService.Instance.Free = CursorService.Instance.Visible = true;
		ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Cancel, CancelListener);
	}

	protected override void OnDisable() {
		ServiceLocator.GetService<CameraService>().Kind = lastCameraKind;
		InstanceByRequest<EngineApplication>.Instance.IsPaused = false;
		ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Cancel, CancelListener);
		Clear();
		base.OnDisable();
	}

	private void UpdateButtons() {
		if (currentButton != null)
			currentButton.interactable = selection != null;
		if (!(deleteButton != null))
			return;
		deleteButton.interactable = selection != null;
	}
}