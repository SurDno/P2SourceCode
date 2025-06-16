using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PathPart : MonoBehaviour {
	private List<Transform> pointsList = new();

	private void Start() {
		UpdateList();
	}

	public List<Transform> PointsList => pointsList;

	private void UpdateList() {
		pointsList.Clear();
		var childCount = transform.childCount;
		for (var index = 0; index < childCount; ++index)
			pointsList.Add(transform.GetChild(index));
	}
}