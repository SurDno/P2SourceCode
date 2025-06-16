using System.Collections.Generic;
using Cofe.Utility;
using Engine.Impl.UI.Controls;
using Engine.Source.Services.Inputs;
using Engine.Source.Utility;
using InputServices;
using UnityEngine;
using UnityEngine.UI;

public class InteractableWindow : MonoBehaviour {
	[SerializeField] private Image blockedImage;
	[SerializeField] private Image lockedImage;
	[SerializeField] private Image normalImage;
	[SerializeField] private Text text;
	[SerializeField] private Transform textGroupContainer;
	[SerializeField] private Title titlePrefab;
	[SerializeField] private ProgressHiddenCircle progressCircle;
	private List<Title> _spawnedTitleList = new();
	private List<GameActionType> listeners = new();
	private List<KeyValuePair<GameActionType, bool>> cachedActions;

	public void SetInfo(IconType info, string text) {
		DeactivateAllTitles();
		if (!text.IsNullOrEmpty()) {
			this.text.gameObject.SetActive(true);
			this.text.text = text;
		}

		switch (info) {
			case IconType.Normal:
				normalImage.gameObject.SetActive(true);
				break;
			case IconType.Locked:
				lockedImage.gameObject.SetActive(true);
				break;
			case IconType.Blocked:
				blockedImage.gameObject.SetActive(true);
				break;
		}
	}

	public void SetInfo(
		IconType info,
		string[] texts,
		List<KeyValuePair<Sprite, bool>> iconSprites,
		List<KeyValuePair<GameActionType, bool>> actions) {
		normalImage.gameObject.SetActive(false);
		lockedImage.gameObject.SetActive(false);
		blockedImage.gameObject.SetActive(false);
		text.gameObject.SetActive(false);
		progressCircle.Progress = 0.0f;
		cachedActions = actions;
		var index = 0;
		for (var length = texts.Length; index < length || index < _spawnedTitleList.Count; ++index)
			if (index < length) {
				if (index >= _spawnedTitleList.Count)
					_spawnedTitleList.Add(Instantiate(titlePrefab, textGroupContainer));
				var spawnedTitle = _spawnedTitleList[index];
				if (!spawnedTitle.gameObject.activeSelf)
					spawnedTitle.gameObject.SetActive(true);
				var iconSprite = iconSprites[index];
				spawnedTitle.SetText(texts[index], iconSprite.Key, iconSprite.Value);
			} else if (_spawnedTitleList[index].gameObject.activeSelf)
				_spawnedTitleList[index].gameObject.SetActive(false);

		UpdateProgress();
		switch (info) {
			case IconType.Normal:
				normalImage.gameObject.SetActive(true);
				break;
			case IconType.Locked:
				lockedImage.gameObject.SetActive(true);
				break;
			case IconType.Blocked:
				blockedImage.gameObject.SetActive(true);
				break;
		}

		LayoutRebuilder.ForceRebuildLayoutImmediate(textGroupContainer.GetComponent<RectTransform>());
	}

	public void UpdateProgress() {
		var num = 0.0f;
		if (cachedActions != null)
			for (var index = 0; index < cachedActions.Count; ++index) {
				var cachedAction = cachedActions[index];
				if (cachedAction.Value) {
					var instance = InputService.Instance;
					cachedAction = cachedActions[index];
					var actionWithoutHold = InputUtility.GetHotKeyNameByActionWithoutHold(cachedAction.Key);
					var holdProgress = instance.GetHoldProgress(actionWithoutHold);
					if (holdProgress > (double)num)
						num = holdProgress;
				}
			}

		progressCircle.Progress = num;
	}

	public void DeactivateAllTitles() {
		progressCircle.Progress = 0.0f;
		normalImage.gameObject.SetActive(false);
		lockedImage.gameObject.SetActive(false);
		blockedImage.gameObject.SetActive(false);
		text.gameObject.SetActive(false);
		var index = 0;
		for (var count = _spawnedTitleList.Count; index < count; ++index)
			if (_spawnedTitleList[index].gameObject.activeSelf)
				_spawnedTitleList[index].gameObject.SetActive(false);
	}

	public enum IconType {
		None,
		Normal,
		Locked,
		Blocked
	}
}