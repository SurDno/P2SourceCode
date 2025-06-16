using Engine.Source.Utility;
using UnityEngine;
using UnityEngine.Audio;

namespace Engine.Source.Behaviours.Controllers;

public class PlayerStepsController : StepsController {
	[Tooltip("Влияет на частоту проигрывания шагов.")] [SerializeField]
	private float playerStepSize = 3f;

	private CharacterController characterController;
	private bool footLeft;
	private float footDistance;

	protected override AudioMixerGroup FootAudioMixer =>
		ScriptableObjectInstance<GameSettingsData>.Instance.ProtagonistFootMixer;

	protected override AudioMixerGroup FootEffectsAudioMixer =>
		ScriptableObjectInstance<GameSettingsData>.Instance.ProtagonistFootEffectsMixer;

	protected override void Awake() {
		base.Awake();
		characterController = GetComponent<CharacterController>();
	}

	private void FixedUpdate() {
		if (!PlayerUtility.IsPlayerCanControlling)
			return;
		if (characterController == null)
			Debug.LogError("{1} needs to have charackter controller if used as player");
		else {
			if (!characterController.isGrounded)
				return;
			footDistance += characterController.velocity.magnitude * Time.deltaTime;
			var num = Mathf.Sin(6.28318548f * footDistance / playerStepSize);
			if (num < 0.0 && !footLeft) {
				OnStep("Skeleton.Humanoid.Foot_Left", false);
				footLeft = true;
			} else if (num > 0.0 && footLeft) {
				OnStep("Skeleton.Humanoid.Foot_Right", false);
				footLeft = false;
			}
		}
	}
}