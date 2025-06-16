using System;
using UnityEngine;

namespace RootMotion.Dynamics;

[Serializable]
public class SubBehaviourCOM : SubBehaviourBase {
	public Mode mode;
	public float velocityDamper = 1f;
	public float velocityLerpSpeed = 5f;
	public float velocityMax = 1f;
	public float centerOfPressureSpeed = 5f;
	public Vector3 offset;
	[HideInInspector] public bool[] groundContacts;
	[HideInInspector] public Vector3[] groundContactPoints;
	private LayerMask groundLayers;

	public Vector3 position { get; private set; }

	public Vector3 direction { get; private set; }

	public float angle { get; private set; }

	public Vector3 velocity { get; private set; }

	public Vector3 centerOfPressure { get; private set; }

	public Quaternion rotation { get; private set; }

	public Quaternion inverseRotation { get; private set; }

	public bool isGrounded { get; private set; }

	public float lastGroundedTime { get; private set; }

	public void Initiate(BehaviourBase behaviour, LayerMask groundLayers) {
		this.behaviour = behaviour;
		this.groundLayers = groundLayers;
		rotation = Quaternion.identity;
		groundContacts = new bool[behaviour.puppetMaster.muscles.Length];
		groundContactPoints = new Vector3[groundContacts.Length];
		behaviour.OnPreActivate += OnPreActivate;
		behaviour.OnPreLateUpdate += OnPreLateUpdate;
		behaviour.OnPreDeactivate += OnPreDeactivate;
		behaviour.OnPreMuscleCollision += OnPreMuscleCollision;
		behaviour.OnPreMuscleCollisionExit += OnPreMuscleCollisionExit;
		behaviour.OnHierarchyChanged += OnHierarchyChanged;
	}

	private void OnHierarchyChanged() {
		Array.Resize(ref groundContacts, behaviour.puppetMaster.muscles.Length);
		Array.Resize(ref groundContactPoints, behaviour.puppetMaster.muscles.Length);
	}

	private void OnPreMuscleCollision(MuscleCollision c) {
		if (!LayerMaskExtensions.Contains(groundLayers, c.collision.gameObject.layer) ||
		    c.collision.contacts.Length == 0)
			return;
		lastGroundedTime = Time.time;
		groundContacts[c.muscleIndex] = true;
		if (mode != Mode.CenterOfPressure)
			return;
		groundContactPoints[c.muscleIndex] = GetCollisionCOP(c.collision);
	}

	private void OnPreMuscleCollisionExit(MuscleCollision c) {
		if (!LayerMaskExtensions.Contains(groundLayers, c.collision.gameObject.layer))
			return;
		groundContacts[c.muscleIndex] = false;
		groundContactPoints[c.muscleIndex] = Vector3.zero;
	}

	private void OnPreActivate() {
		position = GetCenterOfMass();
		centerOfPressure = GetFeetCentroid();
		direction = position - centerOfPressure;
		angle = Vector3.Angle(direction, Vector3.up);
		velocity = Vector3.zero;
	}

	private void OnPreLateUpdate() {
		isGrounded = IsGrounded();
		if (mode == Mode.FeetCentroid || !isGrounded)
			centerOfPressure = GetFeetCentroid();
		else {
			var b = isGrounded ? GetCenterOfPressure() : GetFeetCentroid();
			centerOfPressure = centerOfPressureSpeed <= 2.0
				? b
				: Vector3.Lerp(centerOfPressure, b, Time.deltaTime * centerOfPressureSpeed);
		}

		position = GetCenterOfMass();
		var vector3 = (GetCenterOfMassVelocity() - position) with {
			y = 0.0f
		};
		vector3 = Vector3.ClampMagnitude(vector3, velocityMax);
		velocity = velocityLerpSpeed <= 0.0
			? vector3
			: Vector3.Lerp(velocity, vector3, Time.deltaTime * velocityLerpSpeed);
		position += velocity * velocityDamper;
		position += behaviour.puppetMaster.targetRoot.rotation * offset;
		direction = position - centerOfPressure;
		rotation = Quaternion.FromToRotation(Vector3.up, direction);
		inverseRotation = Quaternion.Inverse(rotation);
		angle = Quaternion.Angle(Quaternion.identity, rotation);
	}

	private void OnPreDeactivate() {
		velocity = Vector3.zero;
	}

	private Vector3 GetCollisionCOP(Collision collision) {
		var zero = Vector3.zero;
		for (var index = 0; index < collision.contacts.Length; ++index)
			zero += collision.contacts[index].point;
		return zero / collision.contacts.Length;
	}

	private bool IsGrounded() {
		for (var index = 0; index < groundContacts.Length; ++index)
			if (groundContacts[index])
				return true;
		return false;
	}

	private Vector3 GetCenterOfMass() {
		var zero = Vector3.zero;
		var num = 0.0f;
		foreach (var muscle in behaviour.puppetMaster.muscles) {
			zero += muscle.rigidbody.worldCenterOfMass * muscle.rigidbody.mass;
			num += muscle.rigidbody.mass;
		}

		Vector3 vector3;
		return vector3 = zero / num;
	}

	private Vector3 GetCenterOfMassVelocity() {
		var vector3_1 = Vector3.zero;
		var num = 0.0f;
		foreach (var muscle in behaviour.puppetMaster.muscles) {
			vector3_1 = vector3_1 + muscle.rigidbody.worldCenterOfMass * muscle.rigidbody.mass +
			            muscle.rigidbody.velocity * muscle.rigidbody.mass;
			num += muscle.rigidbody.mass;
		}

		Vector3 vector3_2;
		return vector3_2 = vector3_1 / num;
	}

	private Vector3 GetMomentum() {
		var zero = Vector3.zero;
		for (var index = 0; index < behaviour.puppetMaster.muscles.Length; ++index)
			zero += behaviour.puppetMaster.muscles[index].rigidbody.velocity *
			        behaviour.puppetMaster.muscles[index].rigidbody.mass;
		return zero;
	}

	private Vector3 GetCenterOfPressure() {
		var zero = Vector3.zero;
		var num = 0;
		for (var index = 0; index < groundContacts.Length; ++index)
			if (groundContacts[index]) {
				zero += groundContactPoints[index];
				++num;
			}

		return zero / num;
	}

	private Vector3 GetFeetCentroid() {
		var zero = Vector3.zero;
		var num = 0;
		for (var index = 0; index < behaviour.puppetMaster.muscles.Length; ++index)
			if (behaviour.puppetMaster.muscles[index].props.group == Muscle.Group.Foot) {
				zero += behaviour.puppetMaster.muscles[index].rigidbody.worldCenterOfMass;
				++num;
			}

		return zero / num;
	}

	[Serializable]
	public enum Mode {
		FeetCentroid,
		CenterOfPressure
	}
}