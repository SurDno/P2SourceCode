using System;
using Inspectors;
using Scripts.Utility;
using UnityEngine;

public class ContainerAnimator : MonoBehaviour {
	[SerializeField] private TransformInfo Opened;
	[SerializeField] private TransformInfo Closed;
	private bool isOpened;
	private float progress;
	private float speed = 2f;

	public bool IsOpened {
		get => isOpened;
		set {
			if (isOpened == value)
				return;
			isOpened = value;
		}
	}

	private void Update() {
		if (isOpened) {
			if (progress == 1.0)
				return;
			progress += Time.deltaTime * speed;
			if (progress > 1.0)
				progress = 1f;
		} else {
			if (progress == 0.0)
				return;
			progress -= Time.deltaTime * speed;
			if (progress < 0.0)
				progress = 0.0f;
		}

		var t = EasyUtility.QuarticEaseOut(progress);
		transform.localPosition = Vector3.LerpUnclamped(Closed.Position, Opened.Position, t);
		transform.localRotation = Quaternion.LerpUnclamped(Closed.Rotation, Opened.Rotation, t);
	}

	[Inspected(Mode = ExecuteMode.Edit)]
	private void SaveAsClosed() {
		Closed.Position = transform.localPosition;
		Closed.Rotation = transform.localRotation;
	}

	[Inspected(Mode = ExecuteMode.Edit)]
	private void SaveAsOpened() {
		Opened.Position = transform.localPosition;
		Opened.Rotation = transform.localRotation;
	}

	[Inspected(Mode = ExecuteMode.EditAndRuntime)]
	private void Close() {
		progress = 0.0f;
		transform.localPosition = Closed.Position;
		transform.localRotation = Closed.Rotation;
	}

	[Inspected(Mode = ExecuteMode.EditAndRuntime)]
	private void Open() {
		progress = 1f;
		transform.localPosition = Opened.Position;
		transform.localRotation = Opened.Rotation;
	}

	[Serializable]
	public struct TransformInfo {
		public Vector3 Position;
		public Quaternion Rotation;
	}
}