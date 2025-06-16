using System;
using System.Collections.Generic;
using Engine.Common;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Commons;
using Engine.Source.Settings.External;
using UnityEngine;
using Random = UnityEngine.Random;

public class LeafManager : MonoBehaviourInstance<LeafManager>, IUpdatable {
	[Tooltip("Основной префаб для создания траекторий")]
	public GameObject animatorPrefab;

	[Tooltip("Настройки новых листьев будут выбраны случайно в диапазоне между префабом и этим состоянием")]
	public LeafAnimator alternativeSettings;

	[Tooltip("Отсюда будет случайным образом выбираться листья")]
	public GameObject[] leafPrefabs;

	public int poolCapacity = 64;
	public LayerMask collideLayers = byte.MaxValue;
	[Space] public float radius = 30f;

	[Tooltip("Высота над playerPosition, с которой начинается поиск столкновений. Должно быть выше крыш")]
	public float raycastOriginElevation = 50f;

	[Space] [Tooltip("Если Y нормали в точке столкновения ниже, лист растворится без приземления")]
	public float minLandingNormalY = 0.75f;

	[Space] public Vector2 wind;
	public float deviation;
	[Space] public Vector3 playerPosition;
	public float rate = 1f;
	[Space] public float minSimulationTime = 0.25f;
	private Stack<LeafAnimator> _pool;
	private float _spawnPhase = 1f;
	private const float VelocitySampleLength = 2f;
	private const float MaxVelocity = 10f;
	private float _velocitySampleTime = -1f;
	private Vector3 _velocitySamplePos;
	private Vector3 _halfPlayerVelocity;
	private TimeSpan prevTime;
	private TimeService timeService;
	private IUpdater updater;

	public void ReturnAnimator(LeafAnimator leafAnimator) {
		if (_pool.Count < poolCapacity) {
			leafAnimator.gameObject.SetActive(false);
			_pool.Push(leafAnimator);
		} else
			Destroy(leafAnimator.gameObject);
	}

	private void Spawn() {
		LeafAnimator leafAnimator = null;
		do {
			if (_pool.Count > 0)
				leafAnimator = _pool.Pop();
			else
				goto label_4;
		} while (leafAnimator == null);

		leafAnimator.gameObject.SetActive(true);
		label_4:
		if (leafAnimator == null) {
			var gameObject1 = Instantiate(animatorPrefab);
			gameObject1.transform.SetParent(transform, false);
			leafAnimator = gameObject1.GetComponent<LeafAnimator>();
			leafAnimator.manager = this;
			var gameObject2 = Instantiate(leafPrefabs[Random.Range(0, leafPrefabs.Length)]);
			gameObject2.transform.SetParent(leafAnimator.transform, false);
			leafAnimator.leafTransform = gameObject2.transform;
			leafAnimator.leafRenderer = gameObject2.GetComponent<Renderer>();
		}

		var t = Random.value;
		leafAnimator.flyFade = Mathf.Lerp(leafAnimator.flyFade, alternativeSettings.flyFade, t);
		leafAnimator.layFade = Mathf.Lerp(leafAnimator.layFade, alternativeSettings.layFade, t);
		leafAnimator.firstPeriod = Mathf.Lerp(leafAnimator.firstPeriod, alternativeSettings.firstPeriod, t);
		leafAnimator.firstRadius = Mathf.Lerp(leafAnimator.firstRadius, alternativeSettings.firstRadius, t);
		leafAnimator.flyTime = Mathf.Lerp(leafAnimator.flyTime, alternativeSettings.flyTime, t);
		leafAnimator.landingBlend = Mathf.Lerp(leafAnimator.landingBlend, alternativeSettings.landingBlend, t);
		leafAnimator.layTime = Mathf.Lerp(leafAnimator.layTime, alternativeSettings.layTime, t);
		leafAnimator.secondPeriod = Mathf.Lerp(leafAnimator.secondPeriod, alternativeSettings.secondPeriod, t);
		leafAnimator.secondRadius = Mathf.Lerp(leafAnimator.secondRadius, alternativeSettings.secondRadius, t);
		leafAnimator.slope = Mathf.Lerp(leafAnimator.slope, alternativeSettings.slope, t);
		leafAnimator.velocity = Vector3.Lerp(leafAnimator.velocity, alternativeSettings.velocity, t);
		leafAnimator.rotation = Random.Range(0.0f, 360f);
		var vector3_1 = playerPosition + _halfPlayerVelocity * leafAnimator.flyTime;
		var vector2_1 = Random.insideUnitCircle * this.radius;
		var vector3_2 = new Vector3(vector3_1.x + vector2_1.x, playerPosition.y, vector3_1.z + vector2_1.y);
		var vector2_2 = Random.insideUnitCircle * deviation + wind;
		var direction = new Vector3(vector2_2.x, -1f, vector2_2.y);
		var radius = leafAnimator.firstRadius + leafAnimator.secondRadius;
		var origin1 = vector3_2 - direction * raycastOriginElevation;
		RaycastHit hitInfo;
		Vector3 vector3_3;
		if (Physics.SphereCast(origin1, radius, direction, out hitInfo,
			    (float)(raycastOriginElevation * (double)direction.magnitude * 2.0), collideLayers,
			    QueryTriggerInteraction.Ignore)) {
			var origin2 = origin1 + direction.normalized * hitInfo.distance;
			if (Physics.Raycast(origin2, direction, out hitInfo, (float)(radius * (double)direction.magnitude * 1.5),
				    collideLayers, QueryTriggerInteraction.Ignore)) {
				if (hitInfo.normal.y > (double)minLandingNormalY) {
					leafAnimator.landing = true;
					leafAnimator.landingNormal = hitInfo.normal;
				} else
					leafAnimator.landing = false;

				vector3_3 = hitInfo.point;
			} else {
				leafAnimator.landing = false;
				vector3_3 = origin2 + radius * direction.normalized;
			}
		} else {
			vector3_3 = vector3_2 + leafAnimator.velocity * leafAnimator.flyTime * 0.5f;
			leafAnimator.landing = false;
		}

		leafAnimator.transform.position = vector3_3;
		leafAnimator.velocity = -direction * leafAnimator.velocity.y;
		leafAnimator.SetToStart();
	}

	private void SampleVelocity() {
		if (_velocitySampleTime == -1.0) {
			_velocitySampleTime = Time.time;
			_velocitySamplePos = playerPosition;
			_halfPlayerVelocity = Vector3.zero;
		} else {
			var num = Time.time - _velocitySampleTime;
			if (num >= 2.0) {
				_halfPlayerVelocity = (playerPosition - _velocitySamplePos) / num;
				var magnitude = _halfPlayerVelocity.magnitude;
				if (magnitude > 10.0)
					_halfPlayerVelocity *= (float)(10.0 / magnitude * 0.5);
				_velocitySamplePos = playerPosition;
				_velocitySampleTime = Time.time;
			}
		}
	}

	private void Start() {
		_pool = new Stack<LeafAnimator>(poolCapacity);
		timeService = ServiceLocator.GetService<TimeService>();
		prevTime = timeService.RealTime;
		updater = InstanceByRequest<UpdateService>.Instance.LeafSpawner;
		updater.AddUpdatable(this);
	}

	private void OnDestroy() {
		updater.RemoveUpdatable(this);
	}

	void IUpdatable.ComputeUpdate() {
		if (ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.DisableLeaf ||
		    !gameObject.activeInHierarchy)
			return;
		var timeSpan = timeService.RealTime - prevTime;
		prevTime = timeService.RealTime;
		SampleVelocity();
		for (_spawnPhase += (float)timeSpan.TotalSeconds * rate; _spawnPhase >= 1.0; --_spawnPhase)
			Spawn();
	}
}