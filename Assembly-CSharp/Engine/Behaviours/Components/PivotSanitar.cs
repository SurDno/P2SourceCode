using System;
using System.Collections.Generic;
using Engine.Behaviours.Engines.Controllers;
using Engine.Common;
using UnityEngine;
using UnityEngine.Serialization;

namespace Engine.Behaviours.Components;

[RequireComponent(typeof(IKController))]
[DisallowMultipleComponent]
public class PivotSanitar : MonoBehaviour {
	[SerializeField] [FormerlySerializedAs("FlamethrowerPS")]
	private Flamethrower flamethrowerPS;

	private Transform flamethrowerPSParent;
	private Vector3 flamethrowerLocalPosition;
	private Quaternion flamethrowerLocalRotation;
	private Animator animator;
	private FightAnimatorBehavior.AnimatorState animatorState;
	private bool flamethrower;
	private float stanceOnPoseWeigth;
	private bool attackStance;
	private float aimingTime;
	private Transform targetObject;

	private FightAnimatorBehavior.AnimatorState AnimatorState {
		get {
			if (animatorState == null)
				animatorState = FightAnimatorBehavior.GetAnimatorState(animator);
			return animatorState;
		}
	}

	public event Action OnInvalidate;

	public bool AttackStance {
		get => attackStance;
		set {
			if (attackStance == value)
				return;
			attackStance = value;
			var onInvalidate = OnInvalidate;
			if (onInvalidate == null)
				return;
			onInvalidate();
		}
	}

	public bool Flamethrower {
		get => flamethrower;
		set {
			if (flamethrower == value)
				return;
			flamethrower = value;
			var onInvalidate = OnInvalidate;
			if (onInvalidate == null)
				return;
			onInvalidate();
		}
	}

	public float AimingTime => aimingTime;

	public Transform TargetObject {
		get => targetObject;
		set {
			if (targetObject == null)
				aimingTime = 0.0f;
			targetObject = value;
		}
	}

	public IEnumerable<IEntity> Targets {
		get {
			if (flamethrowerPS != null && Flamethrower)
				foreach (var flamable in flamethrowerPS.MovablesHit) {
					var movable = flamable;
					if (movable != null && !(movable.gameObject == null)) {
						var entityMovable = EntityUtility.GetEntity(movable.gameObject);
						if (entityMovable != null)
							yield return entityMovable;
						entityMovable = null;
						movable = null;
					}
				}
		}
	}

	public bool IsIndoor {
		set {
			if (!(flamethrowerPS != null))
				return;
			flamethrowerPS.SetIndoor(value);
		}
	}

	private void Awake() {
		animator = gameObject.GetComponent<Pivot>().GetAnimator();
		if (animator == null)
			Debug.LogErrorFormat("{0} doesn't contain {1} unity component.", gameObject.name, typeof(Animator).Name);
		else {
			animatorState = FightAnimatorBehavior.GetAnimatorState(animator);
			if (!(flamethrowerPS == null))
				return;
			Debug.LogWarningFormat("{0}: doesn't contain FlamethrowerPS", gameObject.name);
		}
	}

	private void Start() {
		if (!(flamethrowerPS != null))
			return;
		flamethrowerLocalPosition = flamethrowerPS.transform.localPosition;
		flamethrowerLocalRotation = flamethrowerPS.transform.localRotation;
		flamethrowerPSParent = flamethrowerPS.transform.parent;
		flamethrowerPS.transform.parent = null;
	}

	private void Update() {
		if (flamethrowerPS != null) {
			flamethrowerPS.Fire = flamethrower;
			flamethrowerPS.transform.position = flamethrowerPSParent.TransformPoint(flamethrowerLocalPosition);
			flamethrowerPS.transform.rotation = flamethrowerPSParent.rotation * flamethrowerLocalRotation;
		}

		stanceOnPoseWeigth = Mathf.Clamp01(stanceOnPoseWeigth + (AttackStance ? 1f : -1f) * Time.deltaTime);
		aimingTime += Time.deltaTime;
	}

	private void OnDestroy() {
		if (!(flamethrowerPS != null))
			return;
		Destroy(flamethrowerPS.gameObject);
	}
}