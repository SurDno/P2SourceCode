using System.Collections.Generic;
using UnityEngine;

public class GroupActivityObject : MonoBehaviour {
	public List<POIBase> childPOIs;
	public bool IsDialogActivity;
	private bool isBusy;
	public static List<GroupActivityObject> ActiveGroupObjects = new();

	public bool IsBusy {
		get => isBusy;
		set => isBusy = value;
	}

	public void Awake() {
		IsBusy = false;
		var component = GetComponent<POIBase>();
		if (component != null && !childPOIs.Contains(component))
			childPOIs.Add(component);
		foreach (var childPoI in childPOIs)
			childPoI.IsChildPOI = true;
	}

	private void OnEnable() {
		if (ActiveGroupObjects.Contains(this))
			return;
		ActiveGroupObjects.Add(this);
	}

	private void OnDisable() {
		if (!ActiveGroupObjects.Contains(this))
			return;
		ActiveGroupObjects.Remove(this);
	}
}