using System;
using Engine.Common;
using Engine.Source.Commons;
using UnityEngine;
using Random = UnityEngine.Random;

public class FlockChild : MonoBehaviour, IUpdatable {
	[NonSerialized] public FlockController flockController;
	[NonSerialized] public Vector3 wayPoint;
	[NonSerialized] public float speed;
	[NonSerialized] public bool dived = true;
	private float stuckCounter;
	[NonSerialized] public float damping;
	private bool soar = true;
	[NonSerialized] public bool landing;
	[NonSerialized] public float targetSpeed;
	[NonSerialized] public bool move = true;
	public GameObject _model;
	[NonSerialized] public float avoidValue;
	[NonSerialized] public float avoidDistance;
	[NonSerialized] public bool avoid = true;
	private float soarTimer;
	private bool instantiated;
	public Vector3 _landingPosOffset;
	private float time;
	private float time2;
	private FlockChildProxy proxy;

	public void Initialise(FlockController flockController) {
		this.flockController = flockController;
		Wander(0.0f);
		SetRandomScale();
		transform.localPosition = FindWaypoint();
		RandomizeStartAnimationFrame();
		InitAvoidanceValues();
		speed = flockController._minSpeed;
		instantiated = true;
	}

	private void Awake() {
		time = Time.time;
		time2 = Time.time;
		InstanceByRequest<UpdateService>.Instance.FlockMoveUpdater.AddUpdatable(this);
		proxy = new FlockChildProxy(this);
	}

	private void OnDestroy() {
		proxy.Dispose();
		proxy = null;
		InstanceByRequest<UpdateService>.Instance.FlockMoveUpdater.RemoveUpdatable(this);
	}

	public void ComputeUpdate() {
		var delta = Time.time - time;
		time = Time.time;
		MoveForward(delta);
		RotationBasedOnWaypoint(delta);
		LimitRotationOfModel(delta);
		SoarTimeLimit(delta);
		CheckForDistanceToWaypoint(delta);
	}

	public void ProxyUpdate() {
		var delta = Time.time - time2;
		time2 = Time.time;
		ComputeAvoidance(delta);
	}

	private void OnEnable() {
		if (!instantiated)
			return;
		if (landing)
			_model.GetComponentNonAlloc<Animation>().Play(flockController._idleAnimation);
		else
			_model.GetComponentNonAlloc<Animation>().Play(flockController._flapAnimation);
	}

	private void OnDisable() {
		CancelInvoke();
	}

	private void RandomizeStartAnimationFrame() {
		foreach (AnimationState animationState in _model.GetComponentNonAlloc<Animation>())
			animationState.time = Random.value * animationState.length;
	}

	private void InitAvoidanceValues() {
		avoidValue = Random.Range(0.3f, 0.1f);
		if (flockController._birdAvoidDistanceMax != (double)flockController._birdAvoidDistanceMin)
			avoidDistance = Random.Range(flockController._birdAvoidDistanceMax, flockController._birdAvoidDistanceMin);
		else
			avoidDistance = flockController._birdAvoidDistanceMin;
	}

	private void SetRandomScale() {
		var num = Random.Range(flockController._minScale, flockController._maxScale);
		transform.localScale = new Vector3(num, num, num);
	}

	private void SoarTimeLimit(float delta) {
		if (!soar || flockController._soarMaxTime <= 0.0)
			return;
		if (soarTimer > (double)flockController._soarMaxTime) {
			Flap();
			soarTimer = 0.0f;
		} else
			soarTimer += delta;
	}

	private void CheckForDistanceToWaypoint(float delta) {
		if (transform == null || flockController == null)
			return;
		if (!landing && (transform.localPosition - wayPoint).magnitude <
		    flockController._waypointDistance + (double)stuckCounter) {
			Wander(0.0f);
			stuckCounter = 0.0f;
		} else if (!landing)
			stuckCounter += delta;
		else
			stuckCounter = 0.0f;
	}

	private void MoveForward(float delta) {
		var forward = landing ? wayPoint - transform.position : wayPoint - transform.localPosition;
		if (targetSpeed > -1.0 && forward != Vector3.zero)
			transform.rotation =
				Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(forward), delta * damping);
		speed = Mathf.Lerp(speed, targetSpeed, delta * 2.5f);
		if (!move)
			return;
		transform.localPosition += transform.forward * speed * delta;
	}

	private void RotationBasedOnWaypoint(float delta) {
		if (!flockController._childTriggerPos ||
		    (transform.localPosition - flockController.targetPosition).magnitude >= 1.0)
			return;
		flockController.SetFlockRandomPosition();
	}

	private void ComputeAvoidance(float delta) {
		if (!move || !avoid || !flockController._birdAvoid)
			return;
		Avoidance(delta);
	}

	private void Avoidance(float delta) {
		var hitInfo = new RaycastHit();
		var forward = _model.transform.forward;
		var right = _model.transform.right;
		var rotation = transform.rotation;
		var eulerAngles = transform.rotation.eulerAngles;
		var localPosition = transform.localPosition;
		if (Physics.Raycast(transform.localPosition, forward + right * avoidValue, out hitInfo, avoidDistance,
			    flockController._avoidanceMask)) {
			eulerAngles.y -= flockController._birdAvoidHorizontalForce * delta * damping;
			rotation.eulerAngles = eulerAngles;
			transform.rotation = rotation;
		} else if (Physics.Raycast(transform.localPosition, forward + right * -avoidValue, out hitInfo, avoidDistance,
			           flockController._avoidanceMask)) {
			eulerAngles.y += flockController._birdAvoidHorizontalForce * delta * damping;
			rotation.eulerAngles = eulerAngles;
			transform.rotation = rotation;
		}

		if (flockController._birdAvoidDown && !landing && Physics.Raycast(transform.localPosition, -Vector3.up,
			    out hitInfo, avoidDistance, flockController._avoidanceMask)) {
			eulerAngles.x -= flockController._birdAvoidVerticalForce * delta * damping;
			rotation.eulerAngles = eulerAngles;
			transform.rotation = rotation;
			localPosition.y += (float)(flockController._birdAvoidVerticalForce * (double)delta * 0.0099999997764825821);
			transform.localPosition = localPosition;
		} else {
			if (!flockController._birdAvoidUp || landing || !Physics.Raycast(transform.localPosition, Vector3.up,
				    out hitInfo, avoidDistance, flockController._avoidanceMask))
				return;
			eulerAngles.x += flockController._birdAvoidVerticalForce * delta * damping;
			rotation.eulerAngles = eulerAngles;
			transform.rotation = rotation;
			localPosition.y -= (float)(flockController._birdAvoidVerticalForce * (double)delta * 0.0099999997764825821);
			transform.localPosition = localPosition;
		}
	}

	private void LimitRotationOfModel(float delta) {
		var localRotation = _model.transform.localRotation;
		var eulerAngles = localRotation.eulerAngles;
		if ((((soar && flockController._flatSoar) || (flockController._flatFly && !soar)) &&
		     wayPoint.y > (double)transform.localPosition.y) || landing) {
			eulerAngles.x = Mathf.LerpAngle(_model.transform.localEulerAngles.x, -transform.localEulerAngles.x,
				delta * 1.75f);
			localRotation.eulerAngles = eulerAngles;
			_model.transform.localRotation = localRotation;
		} else {
			eulerAngles.x = Mathf.LerpAngle(_model.transform.localEulerAngles.x, 0.0f, delta * 1.75f);
			localRotation.eulerAngles = eulerAngles;
			_model.transform.localRotation = localRotation;
		}
	}

	public void Wander(float delay) {
		if (landing)
			return;
		damping = Random.Range(flockController._minDamping, flockController._maxDamping);
		targetSpeed = Random.Range(flockController._minSpeed, flockController._maxSpeed);
		Invoke("SetRandomMode", delay);
	}

	private void SetRandomMode() {
		CancelInvoke(nameof(SetRandomMode));
		if (!dived && Random.value < (double)flockController._soarFrequency)
			Soar();
		else if (!dived && Random.value < (double)flockController._diveFrequency)
			Dive();
		else
			Flap();
	}

	public void Flap() {
		if (!move)
			return;
		if (_model != null)
			_model.GetComponentNonAlloc<Animation>().CrossFade(flockController._flapAnimation, 0.5f);
		soar = false;
		AnimationSpeed();
		wayPoint = FindWaypoint();
		dived = false;
	}

	private Vector3 FindWaypoint() {
		return Vector3.zero with {
			x = Random.Range(-flockController._spawnSphereWidth, flockController._spawnSphereWidth) +
			    flockController.targetPosition.x,
			y = Random.Range(-flockController._spawnSphereHeight, flockController._spawnSphereHeight) +
			    flockController.targetPosition.y,
			z = Random.Range(-flockController._spawnSphereDepth, flockController._spawnSphereDepth) +
			    flockController.targetPosition.z
		};
	}

	private void Soar() {
		if (!move)
			return;
		_model.GetComponentNonAlloc<Animation>().CrossFade(flockController._soarAnimation, 1.5f);
		wayPoint = FindWaypoint();
		soar = true;
	}

	private void Dive() {
		if (flockController._soarAnimation != null)
			_model.GetComponentNonAlloc<Animation>().CrossFade(flockController._soarAnimation, 1.5f);
		else
			foreach (AnimationState animationState in _model.GetComponentNonAlloc<Animation>())
				if (transform.localPosition.y < wayPoint.y + 25.0)
					animationState.speed = 0.1f;
		wayPoint = FindWaypoint();
		wayPoint.y -= flockController._diveValue;
		dived = true;
	}

	private void AnimationSpeed() {
		foreach (AnimationState animationState in _model.GetComponentNonAlloc<Animation>())
			animationState.speed = dived || landing
				? flockController._maxAnimationSpeed
				: Random.Range(flockController._minAnimationSpeed, flockController._maxAnimationSpeed);
	}
}