using System;
using System.Collections.Generic;
using Engine.Common;
using Engine.Source.Commons;
using UnityEngine;
using Random = UnityEngine.Random;

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
  public float _soarFrequency;
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
  public LayerMask _avoidanceMask = -1;
  public int _birdAvoidHorizontalForce = 1000;
  public float _birdAvoidDistanceMin = 5f;
  public float _birdAvoidDistanceMax = 4.5f;
  public bool _birdAvoidDown;
  public bool _birdAvoidUp;
  public int _birdAvoidVerticalForce = 300;
  [NonSerialized]
  public List<FlockChild> childs = [];
  [NonSerialized]
  public Vector3 targetPosition;
  private Vector3 playerOffset;

  private void Awake()
  {
    InstanceByRequest<UpdateService>.Instance.FlockSpawnUpdater.AddUpdatable(this);
  }

  private void Start()
  {
    if (!_slowSpawn)
      AddChild(_childAmount);
    if (_randomPositionTimer > 0.0)
      InvokeRepeating("SetFlockRandomPosition", _randomPositionTimer, _randomPositionTimer);
    SetFlockRandomPosition();
    UpdatePlayer();
  }

  private void OnDestroy()
  {
    InstanceByRequest<UpdateService>.Instance.FlockSpawnUpdater.RemoveUpdatable(this);
  }

  private void AddChild(int amount)
  {
    for (int index = 0; index < amount; ++index)
    {
      FlockChild flockChild = Instantiate(_childPrefab);
      flockChild.transform.SetParent(transform, false);
      flockChild.Initialise(this);
      childs.Add(flockChild);
    }
  }

  private void RemoveChild(int amount)
  {
    for (int index = 0; index < amount; ++index)
    {
      FlockChild child = childs[childs.Count - 1];
      childs.RemoveAt(childs.Count - 1);
      Destroy(child.gameObject);
    }
  }

  private void UpdateChildAmount()
  {
    if (_childAmount >= 0 && _childAmount < childs.Count)
    {
      RemoveChild(1);
    }
    else
    {
      if (_childAmount <= childs.Count)
        return;
      AddChild(1);
    }
  }

  public void SetFlockRandomPosition()
  {
    targetPosition.x = Random.Range(-_positionSphereWidth, _positionSphereWidth);
    targetPosition.y = Random.Range(-_positionSphereHeight, _positionSphereHeight);
    targetPosition.z = Random.Range(-_positionSphereDepth, _positionSphereDepth);
    targetPosition += playerOffset;
    if (!_forceChildWaypoints)
      return;
    for (int index = 0; index < childs.Count; ++index)
      childs[index].Wander(Random.value * _forcedRandomDelay);
  }

  public void DestroyBirds()
  {
    for (int index = 0; index < childs.Count; ++index)
      Destroy(childs[index].gameObject);
    _childAmount = 0;
    childs.Clear();
  }

  public void ComputeUpdate()
  {
    if (transform.rotation != Quaternion.identity)
      transform.rotation = Quaternion.identity;
    UpdateChildAmount();
    UpdatePlayer();
  }

  private void UpdatePlayer()
  {
    if (!_moveToPlayer)
      return;
    Vector3 playerPosition = EngineApplication.PlayerPosition;
    playerOffset = new Vector3(playerPosition.x - transform.position.x, playerYOffset, playerPosition.z - transform.position.z);
  }

  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.blue;
    Gizmos.DrawWireCube(transform.position + targetPosition, new Vector3(_spawnSphereWidth * 2f, _spawnSphereHeight * 2f, _spawnSphereDepth * 2f));
    Gizmos.color = Color.cyan;
    Gizmos.DrawWireCube(transform.position + playerOffset, new Vector3(_positionSphereWidth * 2f, _positionSphereHeight * 2f, _positionSphereDepth * 2f));
  }
}
