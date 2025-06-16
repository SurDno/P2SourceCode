using Engine.Source.Commons;
using UnityEngine;

namespace Engine.Behaviours.Components;

public class PausableAnimator : MonoBehaviour {
	private Animator animator;

	private void OnPauseEvent() {
		if (animator == null)
			return;
		if (InstanceByRequest<EngineApplication>.Instance.IsPaused)
			animator.SetFloat("Mecanim.Speed", 0.0f);
		else
			animator.SetFloat("Mecanim.Speed", 1f);
	}

	private void OnEnable() {
		animator = GetComponent<Animator>();
		if (animator == null)
			return;
		InstanceByRequest<EngineApplication>.Instance.OnPauseEvent += OnPauseEvent;
		OnPauseEvent();
	}

	private void OnDisable() {
		if (animator == null)
			return;
		InstanceByRequest<EngineApplication>.Instance.OnPauseEvent -= OnPauseEvent;
	}
}