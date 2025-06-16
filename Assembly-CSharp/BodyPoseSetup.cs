using Inspectors;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Animation))]
public class BodyPoseSetup : MonoBehaviour {
	private Animation animation;
	[SerializeField] private AnimationClip[] clips;
	[SerializeField] [HideInInspector] private int currentClip;

	[Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
	private int CurrentClip {
		get => currentClip;
		set {
			currentClip = value;
			Sample();
		}
	}

	private void Awake() {
		animation = GetComponent<Animation>();
		if (!(animation == null))
			return;
		Debug.LogError(typeof(BodyPoseSetup).Name + ":" + gameObject.name + " should contain Animation component");
	}

	private void Start() {
		Sample();
	}

	private void Sample() {
		if (animation == null || clips.Length < 0 || clips.Length <= currentClip)
			return;
		animation.AddClip(clips[currentClip], clips[currentClip].name);
		animation.clip = clips[currentClip];
		var name = animation.clip.name;
		animation[name].time = 0.0f;
		animation[name].weight = 1f;
		animation[name].enabled = true;
		animation.Sample();
		animation[name].enabled = false;
	}
}