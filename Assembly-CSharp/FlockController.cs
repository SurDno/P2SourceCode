using Engine.Common;
using Engine.Source.Commons;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FlockController : MonoBehaviour, IUpdatable
{
  public FlockChild _childPrefab;
  [Space]
  [Header("Roaming Area")]
  public float _positionSphereWidth = 25f;
  public float _positionSphereHeight = 25f;
  public float _positionSphereDepth = 25f;
  public bool _moveToPlayer;
  public float playerYOffset;
  [Space]
  [Header("Size of the flock")]
  public int _childAmount = 250;
  public float _spawnSphereWidth = 3f;
  public float _spawnSphereHeight = 3f;
  public float _spawnSphereDepth = 3f;
  public bool _slowSpawn;
  [Space]
  [Header("Behaviors and Appearance")]
  public float _minSpeed = 6f;
  public float _maxSpeed = 10f;
  public float _diveValue = 7f;
  public float _diveFrequency = 0.5f;
  public float _soarFrequency = 0.0f;
  public float _soarMaxTime;
  public float _minDamping = 1f;
  public float _maxDamping = 2f;
  public float _minScale = 0.7f;
  public float _maxScale = 1f;
  [Space]
  [Header("Disable Pitch Rotation")]
  public bool _flatSoar;
  public bool _flatFly;
  [Space]
  [Header("Animations")]
  public string _soarAnimation = "Soar";
  public string _flapAnimation = "Flap";
  public string _idleAnimation = "Idle";
  public float _minAnimationSpeed = 2f;
  public float _maxAnimationSpeed = 4f;
  [Space]
  [Header("Bird Trigger Flock Waypoint")]
  public bool _childTriggerPos;
  public float _waypointDistance = 1f;
  [Space]
  [Header("Automatic Flock Waypoint")]
  public float _randomPositionTimer = 10f;
  [Space]
  [Header("Force Bird Waypoints")]
  public bool _forceChildWaypoints;
  public float _forcedRandomDelay = 1.5f;
  [Space]
  [Header("Avoidance")]
  public bool _birdAvoid;
  public LayerMask _avoidanceMask = (LayerMask) -1;
  public int _birdAvoidHorizontalForce = 1000;
  public float _birdAvoidDistanceMin = 5f;
  public float _birdAvoidDistanceMax = 4.5f;
  public bool _birdAvoidDown;
  public bool _birdAvoidUp;
  public int _birdAvoidVerticalForce = 300;
  [NonSerialized]
  public List<FlockChild> childs = new List<FlockChild>();
  [NonSerialized]
  public Vector3 targetPosition;
  private Vector3 playerOffset;

  private void Awake()
  {
    InstanceByRequest<UpdateService>.Instance.FlockSpawnUpdater.AddUpdatable((IUpdatable) this);
  }

  private void Start()
  {
    if (!this._slowSpawn)
      this.AddChild(this._childAmount);
    if ((double) this._randomPositionTimer > 0.0)
      this.InvokeRepeating("SetFlockRandomPosition", this._randomPositionTimer, this._randomPositionTimer);
    this.SetFlockRandomPosition();
    this.UpdatePlayer();
  }

  private void OnDestroy()
  {
    InstanceByRequest<UpdateService>.Instance.FlockSpawnUpdater.RemoveUpdatable((IUpdatable) this);
  }

  private void AddChild(int amount)
  {
    for (int index = 0; index < amount; ++index)
    {
      FlockChild flockChild = UnityEngine.Object.Instantiate<FlockChild>(this._childPrefab);
      flockChild.transform.SetParent(this.transform, false);
      flockChild.Initialise(this);
      this.childs.Add(flockChild);
    }
  }

  private void RemoveChild(int amount)
  {
    for (int index = 0; index < amount; ++index)
    {
      FlockChild child = this.childs[this.childs.Count - 1];
      this.childs.RemoveAt(this.childs.Count - 1);
      UnityEngine.Object.Destroy((UnityEngine.Object) child.gameObject);
    }
  }

  private void UpdateChildAmount()
  {
    if (this._childAmount >= 0 && this._childAmount < this.childs.Count)
    {
      this.RemoveChild(1);
    }
    else
    {
      if (this._childAmount <= this.childs.Count)
        return;
      this.AddChild(1);
    }
  }

  public void SetFlockRandomPosition()
  {
    this.targetPosition.x = UnityEngine.Random.Range(-this._positionSphereWidth, this._positionSphereWidth);
    this.targetPosition.y = UnityEngine.Random.Range(-this._positionSphereHeight, this._positionSphereHeight);
    this.targetPosition.z = UnityEngine.Random.Range(-this._positionSphereDepth, this._positionSphereDepth);
    this.targetPosition += this.playerOffset;
    if (!this._forceChildWaypoints)
      return;
    for (int index = 0; index < this.childs.Count; ++index)
      this.childs[index].Wander(UnityEngine.Random.value * this._forcedRandomDelay);
  }

  public void DestroyBirds()
  {
    for (int index = 0; index < this.childs.Count; ++index)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.childs[index].gameObject);
    this._childAmount = 0;
    this.childs.Clear();
  }

  public void ComputeUpdate()
  {
    if (this.transform.rotation != Quaternion.identity)
      this.transform.rotation = Quaternion.identity;
    this.UpdateChildAmount();
    this.UpdatePlayer();
  }

  private void UpdatePlayer()
  {
    if (!this._moveToPlayer)
      return;
    Vector3 playerPosition = EngineApplication.PlayerPosition;
    this.playerOffset = new Vector3(playerPosition.x - this.transform.position.x, this.playerYOffset, playerPosition.z - this.transform.position.z);
  }

  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.blue;
    Gizmos.DrawWireCube(this.transform.position + this.targetPosition, new Vector3(this._spawnSphereWidth * 2f, this._spawnSphereHeight * 2f, this._spawnSphereDepth * 2f));
    Gizmos.color = Color.cyan;
    Gizmos.DrawWireCube(this.transform.position + this.playerOffset, new Vector3(this._positionSphereWidth * 2f, this._positionSphereHeight * 2f, this._positionSphereDepth * 2f));
  }
}
