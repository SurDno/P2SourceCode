using System.Collections.Generic;
using Engine.Common.Services;
using Engine.Common.Types;
using Engine.Impl.Services;
using Engine.Impl.UI.Controls;
using Engine.Source.Services;
using Engine.Source.Services.Inputs;
using InputServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Engine.Source.UI.Menu.Protagonist.MindMap;

public class MMSideBar : MonoBehaviour {
	[SerializeField] private MMWindow mindMap;
	[SerializeField] private GameObject globalButton;
	[SerializeField] private GameObject pageButtonPrototype;
	[SerializeField] private RectTransform pageButtonsAnchor;
	private List<GameObject> pageButtons = new();
	private int currentIndex;

	private void OnDisable() {
		ServiceLocator.GetService<MMService>().ChangeUndiscoveredEvent -= UpdateUndiscovered;
		InputService.Instance.onJoystickUsedChanged -= OnJoystick;
		var service = ServiceLocator.GetService<GameActionService>();
		service.RemoveListener(GameActionType.DPadDown, ChangeChapter);
		service.RemoveListener(GameActionType.DPadUp, ChangeChapter);
	}

	private void OnEnable() {
		ServiceLocator.GetService<MMService>().ChangeUndiscoveredEvent += UpdateUndiscovered;
		InputService.Instance.onJoystickUsedChanged += OnJoystick;
		var service = ServiceLocator.GetService<GameActionService>();
		service.AddListener(GameActionType.DPadDown, ChangeChapter);
		service.AddListener(GameActionType.DPadUp, ChangeChapter);
		OnJoystick(InputService.Instance.JoystickUsed);
	}

	private void OnJoystick(bool joystick) {
		EventSystem.current.SetSelectedGameObject(null);
		if (!joystick)
			return;
		EventSystem.current.SetSelectedGameObject(pageButtons[currentIndex]);
	}

	private bool ChangeChapter(GameActionType type, bool down) {
		if ((type == GameActionType.DPadUp) & down) {
			--currentIndex;
			ChangeSelection();
			return true;
		}

		if (!((type == GameActionType.DPadDown) & down))
			return false;
		++currentIndex;
		ChangeSelection();
		return true;
	}

	private void ChangeSelection() {
		var pageCount = mindMap.PageCount;
		if (mindMap.GlobalPage != null)
			++pageCount;
		if (currentIndex > pageCount - 1)
			currentIndex = 0;
		if (currentIndex < 0)
			currentIndex = pageCount - 1;
		EventSystem.current.SetSelectedGameObject(currentIndex < mindMap.PageCount
			? pageButtons[currentIndex]
			: globalButton);
		ExecuteEvents.Execute(currentIndex < mindMap.PageCount ? pageButtons[currentIndex] : globalButton,
			new PointerEventData(EventSystem.current), ExecuteEvents.submitHandler);
	}

	public void UpdateButtons() {
		if (mindMap.GlobalPage != null) {
			globalButton.GetComponent<Button>().interactable = mindMap.OpenedPage != mindMap.GlobalPage;
			string str = null;
			var title = mindMap.GlobalPage.Title;
			if (title != LocalizedText.Empty)
				str = ServiceLocator.GetService<LocalizationService>().GetText(title);
			if (string.IsNullOrEmpty(str))
				str = "Global";
			globalButton.GetComponent<Text>().text = str;
			globalButton.SetActive(true);
			if (mindMap.OpenedPage == mindMap.GlobalPage)
				currentIndex = mindMap.PageCount;
		} else
			globalButton.SetActive(false);

		var pageCount = mindMap.PageCount;
		var openedPage = mindMap.OpenedPage;
		while (pageButtons.Count < pageCount) {
			var index = pageButtons.Count;
			var gameObject = Instantiate(pageButtonPrototype);
			gameObject.transform.SetParent(pageButtonsAnchor, false);
			pageButtons.Add(gameObject);
			gameObject.GetComponent<Button>().onClick.AddListener(() => OpenPage(index));
		}

		for (var index = 0; index < pageButtons.Count; ++index)
			if (index < pageCount) {
				var page = mindMap.GetPage(index);
				pageButtons[index].GetComponent<Button>().interactable = mindMap.OpenedPage != page;
				string str = null;
				var title = page.Title;
				if (title != LocalizedText.Empty)
					str = ServiceLocator.GetService<LocalizationService>().GetText(title);
				if (string.IsNullOrEmpty(str))
					str = index.ToString();
				pageButtons[index].GetComponent<Text>().text = str;
				pageButtons[index].SetActive(true);
				if (mindMap.OpenedPage == page)
					currentIndex = index;
			} else
				pageButtons[index].SetActive(false);

		UpdateUndiscovered();
	}

	public void OpenPage(int index) {
		currentIndex = index;
		mindMap.OpenPage(mindMap.GetPage(index));
	}

	public void OpenGlobal() {
		mindMap.OpenPage(mindMap.GlobalPage);
	}

	public void UpdateUndiscovered() {
		if (mindMap.GlobalPage != null)
			globalButton.GetComponent<HideableView>().Visible = mindMap.GlobalPage.HasUndiscovered();
		var pageCount = mindMap.PageCount;
		for (var index = 0; index < pageCount; ++index)
			pageButtons[index].GetComponent<HideableView>().Visible = mindMap.GetPage(index).HasUndiscovered();
	}
}