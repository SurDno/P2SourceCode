using System.Collections.Generic;
using Engine.Common.Services;
using Engine.Impl.UI.Controls;
using Engine.Source.Components.BoundCharacters;
using Engine.Source.Services;
using UnityEngine;

public class BoundCharactersWindowModes : MonoBehaviour {
	[SerializeField] private HideableView allCharactersView;
	[SerializeField] private BoundCharactersStateChangeView changedCharactersView;

	private void OnEnable() {
		var items = ServiceLocator.GetService<BoundCharactersService>().Items;
		var flag = false;
		foreach (var characterComponent in items)
			if (characterComponent.PreRollStateStored) {
				flag = true;
				break;
			}

		if (flag) {
			changedCharactersView.FinishedEvent += OnChangedCharacterViewFinished;
			changedCharactersView.Show();
		} else {
			allCharactersView.Visible = true;
			allCharactersView.SkipAnimation();
		}
	}

	private void OnDisable() {
		changedCharactersView.FinishedEvent -= OnChangedCharacterViewFinished;
		changedCharactersView.FinishAll();
		allCharactersView.Visible = false;
		allCharactersView.SkipAnimation();
	}

	private void OnChangedCharacterViewFinished() {
		changedCharactersView.FinishedEvent -= OnChangedCharacterViewFinished;
		allCharactersView.Visible = true;
	}
}