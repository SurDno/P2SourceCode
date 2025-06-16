using Cofe.Utility;
using Engine.Behaviours.Components;
using Engine.Source.Audio;
using UnityEngine;
using UnityEngine.Audio;

namespace Engine.Behaviours.Unity.Mecanim;

[DisallowMultipleComponent]
public class AnimatorPivot3DSound : StateMachineBehaviour {
	[SerializeField] private AudioClip Clip;
	[SerializeField] private AudioMixerGroup AudioMixer2;
	[SerializeField] private float Volume = 1f;
	[SerializeField] private float MinDistance = 1f;
	[SerializeField] private float MaxDistance = 10f;
	[SerializeField] private Pivot.AimWeaponType BodyPart = Pivot.AimWeaponType.Head;

	public override void OnStateEnter(
		Animator animator,
		AnimatorStateInfo stateInfo,
		int layerIndex) {
		if (Clip == null)
			return;
		var pivot = animator.gameObject.GetComponent<Pivot>();
		if (pivot == null)
			Debug.LogWarningFormat("{0} doesn't contain {1} unity component.", animator.gameObject.name,
				typeof(Pivot).Name);
		else
			CoroutineService.Instance.WaitFrame(() => SoundUtility.PlayAudioClip3D(pivot.GetAimTransform(BodyPart),
				Clip, AudioMixer2, Volume, MinDistance, MaxDistance, true, 0.0f,
				context: TypeUtility.GetTypeName(GetType()) + " " + name));
	}
}