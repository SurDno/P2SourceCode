using System;
using System.Collections.Generic;
using Engine.Behaviours.Components;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Parameters;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using Inspectors;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Pathologic.Prototype;

public class PlagueFace : MonoBehaviour, IEntityAttachable {
	private IParameter<float> DamageParameter;
	private bool _isInitialized;
	private bool initedRecently;
	private PlagueFacePoint startingPoint;
	private float _lastAttackValue;
	private Vector3 _lastPlayerPos;
	private float _tilleNextPrototypeMessage;
	public float aggessionDecrease = 0.05f;
	[Range(0.0f, 1f)] public float aggresion;
	[Range(0.0f, 1f)] public float attack;
	public float attackDecreaseRate = 1f;
	public float attackIncreaseRate = 1f;
	public float attackMessageFrequency = 2f;
	[Header("Prototype")] public float attackThreshold = 0.1f;
	public float eyeExtent = 0.1f;
	public float eyeSize = 0.3f;
	[Header("Hearing")] public float farHearingRadius = 10f;
	[Space] public Graphic graphic;
	public float maxPlayerVelocity = 1f;
	public Navigation navigation;
	public float nearHearingRadius = 5f;
	public float noiseAggessionIncrease = 0.2f;
	[Header("State")] public Vector3 playerPosition;
	public float playerSize = 0.3f;
	public LayerMask sightObstacles;
	[Header("Sight and Attack")] public float sightRadius = 10f;
	public int sightRaysPerTick = 5;
	public Sound sound;
	[Header("Init")] public GameObject StartingPoint;
	private bool canHearPlayer;

	[Inspected] public IEntity Owner { get; private set; }

	void IEntityAttachable.Attach(IEntity owner) {
		Owner = owner;
		var component1 = Owner.GetComponent<NavigationComponent>();
		if (component1 != null) {
			NavigationComponent_OnTeleport(component1, Owner);
			component1.OnTeleport += NavigationComponent_OnTeleport;
		}

		if (!_isInitialized) {
			var component2 = Owner.GetComponent<CrowdItemComponent>();
			if (component2 != null)
				InitCrowdItem(component2);
		}

		var component3 = Owner.GetComponent<ParametersComponent>();
		if (component3 == null)
			return;
		DamageParameter = component3.GetByName<float>(ParameterNameEnum.InfectionDamage);
	}

	void IEntityAttachable.Detach() {
		var component = Owner.GetComponent<NavigationComponent>();
		if (component != null)
			component.OnTeleport -= NavigationComponent_OnTeleport;
		Owner = null;
		DamageParameter = null;
	}

	private void NavigationComponent_OnTeleport(
		INavigationComponent navigationComponent,
		IEntity entity) {
		if (navigationComponent == null)
			return;
		var setupPoint = ((NavigationComponent)navigationComponent).SetupPoint;
		if (setupPoint == null)
			return;
		GetPathFromGameObject(((IEntityView)setupPoint).GameObject);
	}

	private void InitCrowdItem(CrowdItemComponent crowdItem) {
		GameObject setupPointGo = null;
		if (crowdItem != null && crowdItem.Point != null)
			setupPointGo = crowdItem.Point.GameObject;
		GetPathFromGameObject(setupPointGo);
	}

	private void GetPathFromGameObject(GameObject setupPointGo) {
		if (setupPointGo == null)
			return;
		var componentInChildren = setupPointGo.GetComponentInChildren<PlagueFacePoint>();
		if (componentInChildren == null)
			return;
		StartingPoint = componentInChildren.gameObject;
		if (StartingPoint == null)
			return;
		InitializeAt(StartingPoint.GetComponent<PlagueFacePoint>());
	}

	private static float VisibilityWeight(Transform point, Vector3 playerPosition) {
		var rhs = playerPosition - point.position;
		if (Vector3.Dot(point.forward, rhs) <= 0.0)
			return 0.0f;
		var magnitude = rhs.magnitude;
		return magnitude == 0.0 ? float.PositiveInfinity : 1f / magnitude;
	}

	private void Awake() { }

	public void InitializeAt(PlagueFacePoint startingPoint) {
		if (startingPoint == null)
			Debug.LogWarning("InitializeAt(PlagueFacePoint) doesn't accept null.");
		else {
			this.startingPoint = startingPoint;
			transform.SetParent(startingPoint.transform, false);
			transform.position = startingPoint.transform.position;
			transform.rotation = startingPoint.transform.rotation;
			transform.localScale = Vector3.one;
			aggresion = 0.0f;
			attack = 0.0f;
			navigation.Initialize(startingPoint);
			sound.Initialize();
			graphic.Initialize();
			_tilleNextPrototypeMessage = 1f / attackMessageFrequency;
			_isInitialized = true;
			enabled = true;
			initedRecently = true;
		}
	}

	public void FixedUpdate() {
		if (!_isInitialized) {
			Debug.LogWarning("PlagueFace must not be enabled until InitializeAt(PlagueFacePoint) is called");
			enabled = false;
		} else {
			if (initedRecently && startingPoint != null) {
				transform.position = startingPoint.transform.position;
				transform.rotation = startingPoint.transform.rotation;
				initedRecently = false;
			}

			var player = ServiceLocator.GetService<ISimulation>().Player;
			if (player == null)
				return;
			var gameObject = ((IEntityView)player).GameObject;
			if (gameObject == null)
				return;
			var component1 = gameObject.GetComponent<Pivot>();
			var component2 = (DetectorComponent)Owner.GetComponent<IDetectorComponent>();
			var component3 = player.GetComponent<DetectableComponent>();
			if (component2 != null && component3 != null)
				canHearPlayer = component2.Hearing.Contains(component3);
			playerPosition = component1 == null ? gameObject.transform.position : component1.Chest.transform.position;
			_tilleNextPrototypeMessage -= Time.fixedDeltaTime;
			if (_tilleNextPrototypeMessage < 0.0) {
				_tilleNextPrototypeMessage += 1f / attackMessageFrequency;
				if (attack > (double)attackThreshold)
					_lastAttackValue = attack;
				else if (_lastAttackValue != 0.0)
					_lastAttackValue = 0.0f;
			}

			var isVisible = navigation.IsVisible(transform, playerPosition, false);
			var wasMoved = !isVisible && navigation.TryMove(transform, playerPosition);
			PercieveAndAttack(isVisible);
			graphic.Update(Time.fixedDeltaTime, isVisible, wasMoved);
			sound.Update(this, Time.fixedDeltaTime);
			navigation.Navigate(this, aggresion);
		}
	}

	private void OnDisable() {
		navigation.Disable();
	}

	private void OnValidate() {
		if (nearHearingRadius < 0.0)
			nearHearingRadius = 0.0f;
		if (farHearingRadius <= (double)nearHearingRadius) {
			farHearingRadius = (float)((nearHearingRadius + (double)farHearingRadius) * 0.5 + 1.0 / 1000.0);
			nearHearingRadius = farHearingRadius - 1f / 500f;
		}

		if (maxPlayerVelocity < 0.0)
			maxPlayerVelocity = 0.0f;
		if (aggessionDecrease < 0.0)
			aggessionDecrease = 0.0f;
		if (noiseAggessionIncrease < 0.0)
			noiseAggessionIncrease = 0.0f;
		if (sightRadius < 0.0)
			sightRadius = 0.0f;
		if (playerSize < 0.0)
			playerSize = 0.0f;
		if (eyeExtent < 0.0)
			eyeExtent = 0.0f;
		if (eyeSize < 0.0)
			eyeSize = 0.0f;
		if (sightRaysPerTick < 0)
			sightRaysPerTick = 0;
		if (attackIncreaseRate < 0.0)
			attackIncreaseRate = 0.0f;
		if (attackDecreaseRate < 0.0)
			attackDecreaseRate = 0.0f;
		navigation.Validate();
	}

	private void PercieveAndAttack(bool isVisible) {
		aggresion -= Time.fixedDeltaTime * aggessionDecrease;
		attack -= Time.fixedDeltaTime * attackDecreaseRate;
		var rhs = playerPosition - transform.position;
		var magnitude = rhs.magnitude;
		if (canHearPlayer)
			aggresion += (noiseAggessionIncrease + aggessionDecrease) * Time.fixedDeltaTime *
			             Mathf.Min(Vector3.Distance(playerPosition, _lastPlayerPos) / Time.fixedDeltaTime,
				             maxPlayerVelocity) *
			             Mathf.Min(
				             (float)((farHearingRadius - (double)magnitude) /
				                     (farHearingRadius - (double)nearHearingRadius)), 1f);
		_lastPlayerPos = playerPosition;
		if (magnitude < (double)sightRadius && Vector3.Dot(transform.forward, rhs) > 0.0) {
			var num1 = 0;
			for (var index = 0; index < sightRaysPerTick; ++index) {
				var vector2 = Random.insideUnitCircle * eyeSize;
				var vector3 = transform.localToWorldMatrix.MultiplyPoint(new Vector3(vector2.x, vector2.y, eyeExtent));
				var b = playerPosition + Random.insideUnitSphere * playerSize;
				if (!Physics.Raycast(vector3, b - vector3, out var _, Vector3.Distance(vector3, b), sightObstacles))
					++num1;
			}

			var num2 = num1 / (float)sightRaysPerTick;
			if (num1 > 0) {
				aggresion += num2 * Time.fixedDeltaTime;
				if (isVisible)
					num2 = 0.0f;
				if (num2 > 0.5)
					attack += (attackDecreaseRate + (float)(num2 * 2.0 - 1.0) * attackIncreaseRate) *
					          Time.fixedDeltaTime;
				else
					attack += (float)(attackDecreaseRate * (double)num2 * 2.0) * Time.fixedDeltaTime;
			}
		}

		attack = Mathf.Clamp01(attack);
		aggresion = Mathf.Clamp01(aggresion);
		if (DamageParameter == null)
			return;
		DamageParameter.Value = attack;
	}

	[Serializable]
	public class Graphic {
		private float _lastChangeTime;
		private float _opacity;
		private MaterialPropertyBlock _properties;
		private int _textureIndex;
		public float fadeInTime = 0.5f;
		public float fadeOutTime = 5f;
		[Range(0.0f, 1f)] public float lowOpacity;
		public float minChangeTime = 5f;
		public Renderer renderer;
		public Texture2D[] textures;

		public void Initialize() {
			if (!(renderer != null))
				return;
			_opacity = 1f;
			_properties = new MaterialPropertyBlock();
			_properties.SetFloat("_Opacity", _opacity);
			if (textures.Length != 0) {
				_textureIndex = Random.Range(0, textures.Length);
				_properties.SetTexture("_MainTex", textures[_textureIndex]);
			}

			_lastChangeTime = Time.time;
			renderer.SetPropertyBlock(_properties);
		}

		public void Update(float deltaTime, bool isVisible, bool wasMoved) {
			if (!(renderer != null))
				return;
			if (isVisible)
				_opacity = Mathf.MoveTowards(_opacity, 0.0f, deltaTime / fadeOutTime);
			else {
				_opacity = Mathf.MoveTowards(_opacity, 1f, deltaTime / fadeInTime);
				if (wasMoved && textures.Length != 0 && Time.time >= _lastChangeTime + (double)minChangeTime) {
					_textureIndex = Random.Range(0, textures.Length);
					_properties.SetTexture("_MainTex", textures[_textureIndex]);
					_lastChangeTime = Time.time;
				}
			}

			_properties.SetColor("_Color", new Color(1f, 1f, 1f, 1f));
			_properties.SetFloat("_Opacity", Mathf.Lerp(lowOpacity, 1f, _opacity));
			renderer.SetPropertyBlock(_properties);
			renderer.GetPropertyBlock(_properties);
		}
	}

	[Serializable]
	public class Navigation {
		private CullingGroup _cullingGroup;
		private BoundingSphere[] _cullingSpheres;
		private PlagueFacePoint _currentPoint;
		private float _destinationQuality;
		private bool _isInitialized;
		private float _lastJumpTime;
		private int _nextSearchPoint;
		private PathfinderAStar<PlagueFacePoint, float> _pathfinder;
		private PlagueFacePoint[] _points;
		private List<PlagueFacePoint> _route;
		public Vector4[] boundingSpheres;
		[Range(0.0f, 1f)] public float chaseAggresion = 0.95f;
		public bool manualDestination;
		public Camera playerCamera;
		[Range(0.0f, 1f)] public float roamAggresion = 0.5f;
		public float rotationSpeed = 45f;
		public int searchPointsPerTick = 50;
		public int searchRaysPerTick = 5;
		public float speed = 0.5f;

		private PlagueFacePoint[] CollectPointFrom(PlagueFacePoint startingPoint) {
			var plagueFacePointList = new List<PlagueFacePoint>();
			var plagueFacePointQueue = new Queue<PlagueFacePoint>();
			plagueFacePointQueue.Enqueue(startingPoint);
			while (plagueFacePointQueue.Count > 0) {
				var plagueFacePoint = plagueFacePointQueue.Dequeue();
				if (!plagueFacePointList.Contains(plagueFacePoint)) {
					plagueFacePointList.Add(plagueFacePoint);
					for (var index = 0; index < plagueFacePoint.neighbors.Length; ++index)
						if (plagueFacePoint.neighbors[index] != null)
							plagueFacePointQueue.Enqueue(plagueFacePoint.neighbors[index]);
				}
			}

			return plagueFacePointList.ToArray();
		}

		public void Disable() {
			if (_cullingGroup == null)
				return;
			_cullingGroup.Dispose();
			_cullingGroup = null;
		}

		public PlagueFacePoint[] GetNeighbors(PlagueFacePoint point) {
			return point.neighbors;
		}

		public void Initialize(PlagueFacePoint startingPoint) {
			Disable();
			if (_isInitialized) {
				var flag = false;
				for (var index = 0; index < _points.Length; ++index)
					if (_points[index] == startingPoint) {
						flag = true;
						break;
					}

				if (!flag)
					_points = CollectPointFrom(startingPoint);
				_route.Clear();
			} else {
				_points = CollectPointFrom(startingPoint);
				_route = new List<PlagueFacePoint>();
				_cullingSpheres = new BoundingSphere[boundingSpheres.Length * 2];
				_pathfinder = new PathfinderAStar<PlagueFacePoint, float>(GetNeighbors, TimeToJump, TimeToJump,
					(x, y) => x + y, (x, y) => x < (double)y, 0.0f);
				_isInitialized = true;
			}

			_currentPoint = startingPoint;
			_lastJumpTime = Time.time;
			_destinationQuality = 0.0f;
			_nextSearchPoint = 0;
		}

		public bool IsMoving() {
			return _route.Count > 0;
		}

		public bool IsVisible(Transform transform, Vector3 playerPosition, bool nextPoint) {
			if (VisibilityWeight(transform, playerPosition) <= 0.0)
				return false;
			if (_cullingGroup == null)
				return true;
			for (var index = 0; index < boundingSpheres.Length; ++index)
				if (_cullingGroup.IsVisible(nextPoint ? index + boundingSpheres.Length : index))
					return true;
			return false;
		}

		private float TimeToJump(Transform from, Transform to) {
			return Mathf.Max(Vector3.Distance(from.position, to.position) / speed,
				Quaternion.Angle(from.rotation, to.rotation) / rotationSpeed);
		}

		private float TimeToJump(PlagueFacePoint from, PlagueFacePoint to) {
			return TimeToJump(from.transform, to.transform);
		}

		public bool TryMove(Transform face, Vector3 playerPosition) {
			if (_route.Count <= 0 ||
			    TimeToJump(face, _route[_route.Count - 1].transform) >= Time.time - (double)_lastJumpTime ||
			    IsVisible(_route[_route.Count - 1].transform, playerPosition, true))
				return false;
			_currentPoint = _route[_route.Count - 1];
			_route.RemoveAt(_route.Count - 1);
			face.SetParent(_currentPoint.transform, false);
			face.localPosition = Vector3.zero;
			face.localRotation = Quaternion.identity;
			face.localScale = Vector3.one;
			_lastJumpTime = Time.time;
			return true;
		}

		public void Navigate(PlagueFace face, float aggression) {
			if (!manualDestination && aggression >= (double)roamAggresion) {
				if (aggression < (double)chaseAggresion) {
					Vector3 vector3;
					if (_route.Count == 0) {
						var normalized1 = (face.playerPosition - _currentPoint.transform.position).normalized;
						var index1 = Random.Range(0, _currentPoint.neighbors.Length);
						var flag = false;
						var num1 = float.NegativeInfinity;
						for (var index2 = 0; index2 < _currentPoint.neighbors.Length; ++index2)
							if (!flag || _currentPoint.neighbors[index2].roamable) {
								if (!flag && _currentPoint.neighbors[index2].roamable)
									num1 = float.NegativeInfinity;
								vector3 = _currentPoint.neighbors[index2].transform.position -
								          _currentPoint.transform.position;
								var normalized2 = vector3.normalized;
								var num2 = Vector3.Dot(normalized1, normalized2) * Random.value;
								if (num2 > (double)num1) {
									index1 = index2;
									flag = _currentPoint.neighbors[index2].roamable;
									num1 = num2;
								}
							}

						_route.Add(_currentPoint.neighbors[index1]);
						_destinationQuality = 0.0f;
					}

					if (_route.Count < 2) {
						vector3 = _route[0].transform.position - _currentPoint.transform.position;
						var normalized3 = vector3.normalized;
						var index3 = Random.Range(0, _route[0].neighbors.Length);
						var flag = false;
						var num3 = float.NegativeInfinity;
						for (var index4 = 0; index4 < _route[0].neighbors.Length; ++index4)
							if (!flag || _route[0].neighbors[index4].roamable) {
								if (!flag && _route[0].neighbors[index4].roamable)
									num3 = float.NegativeInfinity;
								vector3 = _route[0].neighbors[index4].transform.position - _route[0].transform.position;
								var normalized4 = vector3.normalized;
								var num4 = Vector3.Dot(normalized3, normalized4) * Random.value;
								if (num4 > (double)num3) {
									index3 = index4;
									flag = _route[0].neighbors[index4].roamable;
									num3 = num4;
								}
							}

						_route.Add(_route[0]);
						_route[0] = _route[0].neighbors[index3];
						_destinationQuality = 0.0f;
					}
				} else {
					var num5 = Mathf.Max(searchPointsPerTick, _points.Length);
					for (var searchRaysPerTick = this.searchRaysPerTick; num5 > 0 && searchRaysPerTick > 0; --num5) {
						if (_nextSearchPoint >= _points.Length)
							_nextSearchPoint = 0;
						var num6 = VisibilityWeight(_points[_nextSearchPoint].transform, face.playerPosition);
						if (num6 > 0.0) {
							var vector2 = Random.insideUnitCircle * face.eyeSize;
							var vector3 = _points[_nextSearchPoint].transform.localToWorldMatrix
								.MultiplyPoint(new Vector3(vector2.x, vector2.y, face.eyeExtent));
							var b = face.playerPosition + Random.insideUnitSphere * face.playerSize;
							if (Physics.Raycast(vector3, b - vector3, out var _, Vector3.Distance(vector3, b),
								    face.sightObstacles))
								num6 = 0.0f;
							--searchRaysPerTick;
						}

						if (_route.Count > 0 && _points[_nextSearchPoint] == _route[0])
							_destinationQuality = num6;
						else if (num6 > (double)_destinationQuality) {
							SetDestination(_points[_nextSearchPoint]);
							_destinationQuality = num6;
						}

						++_nextSearchPoint;
					}
				}
			}

			if (_cullingGroup == null) {
				_cullingGroup = new CullingGroup();
				_cullingGroup.SetBoundingSpheres(_cullingSpheres);
			}

			if (GameCamera.Instance.Camera != null)
				_cullingGroup.targetCamera = GameCamera.Instance.Camera;
			for (var index = 0; index < boundingSpheres.Length; ++index)
				_cullingSpheres[index] =
					new BoundingSphere(_currentPoint.transform.TransformPoint(boundingSpheres[index]),
						boundingSpheres[index].w);
			if (_route.Count > 0) {
				for (var index = 0; index < boundingSpheres.Length; ++index)
					_cullingSpheres[index + boundingSpheres.Length] = new BoundingSphere(
						_route[_route.Count - 1].transform.TransformPoint(boundingSpheres[index]),
						boundingSpheres[index].w);
				_cullingGroup.SetBoundingSphereCount(_cullingSpheres.Length);
			} else
				_cullingGroup.SetBoundingSphereCount(boundingSpheres.Length);
		}

		public void SetDestination(PlagueFacePoint destination) {
			_route.Clear();
			if (destination == _currentPoint)
				return;
			_pathfinder.AddReversedRoute(_currentPoint, destination, _route);
		}

		public void Validate() {
			if (speed < 0.0)
				speed = 0.0f;
			if (searchPointsPerTick < 1)
				searchPointsPerTick = 1;
			if (searchRaysPerTick < 1)
				searchRaysPerTick = 1;
			if (chaseAggresion >= (double)roamAggresion)
				return;
			chaseAggresion = (float)((chaseAggresion + (double)roamAggresion) * 0.5);
			roamAggresion = chaseAggresion;
		}
	}

	[Serializable]
	public class Sound {
		private Vector3 _soundWeights;
		private Vector3 _velocity;
		private Vector3 _worldPosition;
		public Transform anchor;
		[Range(0.0f, 1f)] public float attackMaxVolume = 1f;
		public AudioSource attackSource;
		[Space] public float fadeTime = 1f;
		public float moveSmoothness = 0.5f;
		[Range(0.0f, 1f)] public float roamMaxVolume = 1f;
		public AudioSource roamSource;
		[Range(0.0f, 1f)] public float sleepMaxVolume = 1f;
		public AudioSource sleepSource;
		public Transform transform;

		public void Initialize() {
			if (transform != null && anchor != null) {
				_worldPosition = anchor.position;
				transform.position = _worldPosition;
			}

			_soundWeights = new Vector3(1f, 0.0f, 0.0f);
			if (sleepSource != null)
				sleepSource.volume = sleepMaxVolume;
			if (roamSource != null)
				roamSource.volume = 0.0f;
			if (!(attackSource != null))
				return;
			attackSource.volume = 0.0f;
		}

		public void Update(PlagueFace face, float deltaTime) {
			if (transform != null && anchor != null) {
				_worldPosition = Vector3.SmoothDamp(_worldPosition, anchor.position, ref _velocity, moveSmoothness,
					float.PositiveInfinity, deltaTime);
				transform.position = _worldPosition;
			}

			_soundWeights = Vector3.MoveTowards(_soundWeights,
				face.attack <= 0.0
					? face.aggresion < (double)face.navigation.roamAggresion
						? new Vector3(1f, 0.0f, 0.0f)
						: new Vector3(0.0f, 1f, 0.0f)
					: new Vector3(0.0f, 0.0f, 1f), deltaTime / fadeTime);
			if (sleepSource != null)
				sleepSource.volume = _soundWeights.x * sleepMaxVolume;
			if (roamSource != null)
				roamSource.volume = _soundWeights.y * roamMaxVolume;
			if (!(attackSource != null))
				return;
			attackSource.volume = _soundWeights.z * attackMaxVolume;
		}
	}
}