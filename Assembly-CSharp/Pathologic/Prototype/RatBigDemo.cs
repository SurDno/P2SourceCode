using System.Collections.Generic;
using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components.Utilities;
using UnityEngine;
using UnityEngine.AI;

namespace Pathologic.Prototype
{
  [RequireComponent(typeof (NavMeshAgent))]
  public class RatBigDemo : MonoBehaviour
  {
    private NavMeshAgent _agent;
    private Animator _animator;
    private float _idleTime;
    public State _state;
    public SkinnedMeshRenderer Renderer;
    public float CullDistance = 20f;
    [Tooltip("Probability to go to point of interest (0-1)")]
    [Range(0.0f, 1f)]
    public float POIProbability = 0.3f;
    public float WanderRadius = 15f;
    public float Speed = 2f;
    private List<Vector3> _pathCorners = new List<Vector3>();
    private float putOnNavmeshTimeLeft;
    private int _cornerPathIndex;
    private float _cornerPathDistance;

    private bool IsPlayerNear()
    {
      IEntity player = ServiceLocator.GetService<ISimulation>().Player;
      if (player == null)
        return false;
      GameObject gameObject = ((IEntityView) player).GameObject;
      return !(gameObject == null) && (gameObject.transform.position - transform.position).magnitude < (double) CullDistance;
    }

    private Vector3 PointOfInterest(float distance)
    {
      if (Random.value > (double) POIProbability)
        return RandomNavSphere(gameObject.transform.position, distance, _agent.areaMask);
      GameObject[] gameObjectsWithTag;
      try
      {
        gameObjectsWithTag = GameObject.FindGameObjectsWithTag("RAT_POI");
      }
      catch (UnityException ex)
      {
        return RandomNavSphere(gameObject.transform.position, distance, _agent.areaMask);
      }
      if (gameObjectsWithTag.Length == 0)
        return RandomNavSphere(gameObject.transform.position, distance, _agent.areaMask);
      int index1 = Random.Range(0, gameObjectsWithTag.Length);
      for (int index2 = 0; index2 < gameObjectsWithTag.Length; ++index2)
      {
        int index3 = (index2 + index1) % gameObjectsWithTag.Length;
        if ((gameObjectsWithTag[index3].transform.position - gameObject.transform.position).magnitude < 2.0 * WanderRadius)
          return SampleNavPoint(gameObjectsWithTag[index3].transform.position, _agent.areaMask);
      }
      return SampleNavPoint(gameObjectsWithTag[index1].transform.position, _agent.areaMask);
    }

    private Vector3 RandomNavSphere(Vector3 origin, float distance, int layermask)
    {
      Vector3 vector3 = Random.insideUnitSphere * distance;
      return SampleNavPoint(origin + vector3, layermask);
    }

    private Vector3 SampleNavPoint(Vector3 origin, int layermask)
    {
      NavMeshHit hit;
      NavMesh.SamplePosition(origin, out hit, 100f, layermask);
      return hit.position;
    }

    private void Start()
    {
      _agent = GetComponent<NavMeshAgent>();
      _animator = GetComponent<Animator>();
      _state = State.IDLING;
      _idleTime = 0.0f;
      float maxDistance = 1f;
      NavMeshHit hit;
      if (NavMesh.SamplePosition(gameObject.transform.position, out hit, maxDistance, -1))
      {
        gameObject.transform.position = hit.position;
      }
      else
      {
        Debug.LogWarningFormat("Can't find NavMesh for rat in radius of {0} meter ({1}), x = {2}, y = {3}, z = {4}. The rat will be deleted.", maxDistance, name, transform.position.x, transform.position.y, transform.position.z);
        Destroy(gameObject);
      }
      _agent.updatePosition = true;
      _agent.updateRotation = true;
      _agent.enabled = true;
    }

    private void UpdateLOD()
    {
      if (Random.value > Time.deltaTime / 0.20000000298023224)
        return;
      Renderer.enabled = IsPlayerNear();
    }

    private void Update()
    {
      UpdateLOD();
      if (_state == State.WANDERING_CORNERS)
      {
        _animator.SetFloat("Idle", 0.0f);
        _cornerPathDistance += Speed * Time.deltaTime;
        Vector3 forward;
        while (true)
        {
          Vector3 vector3 = _pathCorners[_cornerPathIndex + 1] - _pathCorners[_cornerPathIndex];
          float magnitude = vector3.magnitude;
          forward = vector3 / magnitude;
          if (_cornerPathDistance > (double) magnitude)
          {
            ++_cornerPathIndex;
            if (_cornerPathIndex + 1 < _pathCorners.Count)
              _cornerPathDistance -= magnitude;
            else
              break;
          }
          else
            goto label_7;
        }
        _idleTime = Random.Range(0.5f, 1f);
        _state = State.IDLING;
        return;
label_7:
        Vector3 vector3_1 = _pathCorners[_cornerPathIndex] + forward * _cornerPathDistance;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(forward), _agent.angularSpeed * Time.deltaTime);
        transform.position = vector3_1;
      }
      else if (_state == State.WANDERING)
      {
        _animator.SetFloat("Idle", 0.0f);
        if (_agent.pathPending || _agent.hasPath && _agent.remainingDistance >= (double) _agent.stoppingDistance)
          return;
        _agent.ResetPath();
        _state = State.IDLING;
        _idleTime = Random.Range(1f, 3f);
      }
      else if (_state == State.IDLING)
      {
        _animator.SetFloat("Idle", 1f);
        _idleTime -= Time.deltaTime;
        if (_idleTime >= 0.0)
          return;
        _agent.enabled = true;
        if (!_agent.isOnNavMesh)
        {
          _state = State.PUT_ON_NAVMESH;
          putOnNavmeshTimeLeft = Random.Range(1f, 2f);
        }
        else
        {
          _agent.destination = PointOfInterest(WanderRadius);
          _state = State.WAITING_PATH;
        }
      }
      else if (_state == State.WAITING_PATH)
      {
        _animator.SetFloat("Idle", 1f);
        if (_agent.pathPending)
          return;
        if (!_agent.isOnNavMesh)
        {
          _state = State.PUT_ON_NAVMESH;
          putOnNavmeshTimeLeft = Random.Range(1f, 2f);
        }
        else if (_agent.pathStatus == NavMeshPathStatus.PathInvalid)
        {
          _agent.destination = PointOfInterest(WanderRadius);
        }
        else
        {
          if (_agent.path == null || _agent.pathStatus != NavMeshPathStatus.PathComplete && _agent.pathStatus != NavMeshPathStatus.PathPartial)
            return;
          if (!NavMeshUtility.HasPathNoGarbage(_agent) || Random.value < Time.deltaTime / 0.5 && !NavMeshUtility.HasPathWithGarbage(_agent))
          {
            _agent.destination = PointOfInterest(WanderRadius);
          }
          else
          {
            NavMeshUtility.GetCornersNonAlloc(_agent, _pathCorners);
            _cornerPathIndex = 0;
            _cornerPathDistance = 0.0f;
            _state = State.WANDERING_CORNERS;
            _agent.enabled = false;
          }
        }
      }
      else
      {
        if (_state != State.PUT_ON_NAVMESH)
          return;
        putOnNavmeshTimeLeft -= Time.deltaTime;
        if (putOnNavmeshTimeLeft < 0.0)
          return;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(gameObject.transform.position, out hit, 1f, -1))
        {
          gameObject.transform.position = hit.position;
          _agent.Warp(hit.position);
          _state = State.WAITING_PATH;
        }
        else if (NavMesh.SamplePosition(gameObject.transform.position, out hit, 10f, -1))
        {
          gameObject.transform.position = hit.position;
          _agent.Warp(hit.position);
          _state = State.WAITING_PATH;
        }
        else
          putOnNavmeshTimeLeft = Random.Range(1f, 2f);
      }
    }

    public enum State
    {
      WAITING_PATH,
      WANDERING,
      WANDERING_CORNERS,
      IDLING,
      PUT_ON_NAVMESH,
    }
  }
}
