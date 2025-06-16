using System.Collections.Generic;
using UnityEngine;

namespace Engine.Source.Services;

public class POIGroupActivity {
	private bool isDialog;
	private GroupActivityObject activityObject;
	private Dictionary<GameObject, bool> CharactersReady;
	private List<GameObject> characters = new();

	public bool IsDialog => isDialog;

	public GroupActivityObject ActivityObject {
		get => activityObject;
		set {
			activityObject = value;
			if (!(activityObject != null))
				return;
			isDialog = activityObject.IsDialogActivity;
		}
	}

	public List<GameObject> Characters {
		get => characters;
		set {
			characters = value;
			CharactersReady = new Dictionary<GameObject, bool>();
			foreach (var character in characters)
				CharactersReady[character] = false;
		}
	}

	public void SetCharacterReady(GameObject character) {
		if (!CharactersReady.ContainsKey(character))
			return;
		CharactersReady[character] = true;
	}

	public bool NoCharactersReady() {
		return !CharactersReady.ContainsValue(true);
	}

	public bool AllCharactersReady() {
		return !CharactersReady.ContainsValue(false);
	}
}