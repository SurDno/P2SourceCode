using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components.Utilities;
using System.Collections.Generic;
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
    public RatBigDemo.State _state;
    public SkinnedMeshRenderer Renderer;
    public float CullDistance = 20f;
    [Tooltip("Probability to go to point of interest (0-1)")]
    [Range(0.0f, 1f)]
    public float POIProbability = 0.3f;
    public float WanderRadius = 15f;
    public float Speed = 2f;
    private List<Vector3> _pathCorners = new List<Vector3>();
    private float putOnNavmeshTimeLeft;
    private int _cornerPathIndex = 0;
    private float _cornerPathDistance;

    private bool IsPlayerNear()
    {
      IEntity player = ServiceLocator.GetService<ISimulation>().Player;
      if (player == null)
        return false;
      GameObject gameObject = ((IEntityView) player).GameObject;
      return !((Object) gameObject == (Object) null) && (double) (gameObject.transform.position - this.transform.position).magnitude < (double) this.CullDistance;
    }

    private Vector3 PointOfInterest(float distance)
    {
      if ((double) Random.value > (double) this.POIProbability)
        return this.RandomNavSphere(this.gameObject.transform.position, distance, this._agent.areaMask);
      GameObject[] gameObjectsWithTag;
      try
      {
        gameObjectsWithTag = GameObject.FindGameObjectsWithTag("RAT_POI");
      }
      catch (UnityException ex)
      {
        return this.RandomNavSphere(this.gameObject.transform.position, distance, this._agent.areaMask);
      }
      if (gameObjectsWithTag.Length == 0)
        return this.RandomNavSphere(this.gameObject.transform.position, distance, this._agent.areaMask);
      int index1 = Random.Range(0, gameObjectsWithTag.Length);
      for (int index2 = 0; index2 < gameObjectsWithTag.Length; ++index2)
      {
        int index3 = (index2 + index1) % gameObjectsWithTag.Length;
        if ((double) (gameObjectsWithTag[index3].transform.position - this.gameObject.transform.position).magnitude < 2.0 * (double) this.WanderRadius)
          return this.SampleNavPoint(gameObjectsWithTag[index3].transform.position, this._agent.areaMask);
      }
      return this.SampleNavPoint(gameObjectsWithTag[index1].transform.position, this._agent.areaMask);
    }

    private Vector3 RandomNavSphere(Vector3 origin, float distance, int layermask)
    {
      Vector3 vector3 = Random.insideUnitSphere * distance;
      return this.SampleNavPoint(origin + vector3, layermask);
    }

    private Vector3 SampleNavPoint(Vector3 origin, int layermask)
    {
      NavMeshHit hit;
      NavMesh.SamplePosition(origin, out hit, 100f, layermask);
      return hit.position;
    }

    private void Start()
    {
      this._agent = this.GetComponent<NavMeshAgent>();
      this._animator = this.GetComponent<Animator>();
      this._state = RatBigDemo.State.IDLING;
      this._idleTime = 0.0f;
      float maxDistance = 1f;
      NavMeshHit hit;
      if (NavMesh.SamplePosition(this.gameObject.transform.position, out hit, maxDistance, -1))
      {
        this.gameObject.transform.position = hit.position;
      }
      else
      {
        Debug.LogWarningFormat("Can't find NavMesh for rat in radius of {0} meter ({1}), x = {2}, y = {3}, z = {4}. The rat will be deleted.", (object) maxDistance, (object) this.name, (object) this.transform.position.x, (object) this.transform.position.y, (object) this.transform.position.z);
        Object.Destroy((Object) this.gameObject);
      }
      this._agent.updatePosition = true;
      this._agent.updateRotation = true;
      this._agent.enabled = true;
    }

    private void UpdateLOD()
    {
      if ((double) Random.value > (double) Time.deltaTime / 0.20000000298023224)
        return;
      this.Renderer.enabled = this.IsPlayerNear();
    }

    private void Update()
    {
      this.UpdateLOD();
      if (this._state == RatBigDemo.State.WANDERING_CORNERS)
      {
        this._animator.SetFloat("Idle", 0.0f);
        this._cornerPathDistance += this.Speed * Time.deltaTime;
        Vector3 forward;
        while (true)
        {
          Vector3 vector3 = this._pathCorners[this._cornerPathIndex + 1] - this._pathCorners[this._cornerPathIndex];
          float magnitude = vector3.magnitude;
          forward = vector3 / magnitude;
          if ((double) this._cornerPathDistance > (double) magnitude)
          {
            ++this._cornerPathIndex;
            if (this._cornerPathIndex + 1 < this._pathCorners.Count)
              this._cornerPathDistance -= magnitude;
            else
              break;
          }
          else
            goto label_7;
        }
        this._idleTime = Random.Range(0.5f, 1f);
        this._state = RatBigDemo.State.IDLING;
        return;
label_7:
        Vector3 vector3_1 = this._pathCorners[this._cornerPathIndex] + forward * this._cornerPathDistance;
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(forward), this._agent.angularSpeed * Time.deltaTime);
        this.transform.position = vector3_1;
      }
      else if (this._state == RatBigDemo.State.WANDERING)
      {
        this._animator.SetFloat("Idle", 0.0f);
        if (this._agent.pathPending || this._agent.hasPath && (double) this._agent.remainingDistance >= (double) this._agent.stoppingDistance)
          return;
        this._agent.ResetPath();
        this._state = RatBigDemo.State.IDLING;
        this._idleTime = Random.Range(1f, 3f);
      }
      else if (this._state == RatBigDemo.State.IDLING)
      {
        this._animator.SetFloat("Idle", 1f);
        this._idleTime -= Time.deltaTime;
        if ((double) this._idleTime >= 0.0)
          return;
        this._agent.enabled = true;
        if (!this._agent.isOnNavMesh)
        {
          this._state = RatBigDemo.State.PUT_ON_NAVMESH;
          this.putOnNavmeshTimeLeft = Random.Range(1f, 2f);
        }
        else
        {
          this._agent.destination = this.PointOfInterest(this.WanderRadius);
          this._state = RatBigDemo.State.WAITING_PATH;
        }
      }
      else if (this._state == RatBigDemo.State.WAITING_PATH)
      {
        this._animator.SetFloat("Idle", 1f);
        if (this._agent.pathPending)
          return;
        if (!this._agent.isOnNavMesh)
        {
          this._state = RatBigDemo.State.PUT_ON_NAVMESH;
          this.putOnNavmeshTimeLeft = Random.Range(1f, 2f);
        }
        else if (this._agent.pathStatus == NavMeshPathStatus.PathInvalid)
        {
          this._agent.destination = this.PointOfInterest(this.WanderRadius);
        }
        else
        {
          if (this._agent.path == null || this._agent.pathStatus != NavMeshPathStatus.PathComplete && this._agent.pathStatus != NavMeshPathStatus.PathPartial)
            return;
          if (!NavMeshUtility.HasPathNoGarbage(this._agent) || (double) Random.value < (double) Time.deltaTime / 0.5 && !NavMeshUtility.HasPathWithGarbage(this._agent))
          {
            this._agent.destination = this.PointOfInterest(this.WanderRadius);
          }
          else
          {
            NavMeshUtility.GetCornersNonAlloc(this._agent, this._pathCorners);
            this._cornerPathIndex = 0;
            this._cornerPathDistance = 0.0f;
            this._state = RatBigDemo.State.WANDERING_CORNERS;
            this._agent.enabled = false;
          }
        }
      }
      else
      {
        if (this._state != RatBigDemo.State.PUT_ON_NAVMESH)
          return;
        this.putOnNavmeshTimeLeft -= Time.deltaTime;
        if ((double) this.putOnNavmeshTimeLeft < 0.0)
          return;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(this.gameObject.transform.position, out hit, 1f, -1))
        {
          this.gameObject.transform.position = hit.position;
          this._agent.Warp(hit.position);
          this._state = RatBigDemo.State.WAITING_PATH;
        }
        else if (NavMesh.SamplePosition(this.gameObject.transform.position, out hit, 10f, -1))
        {
          this.gameObject.transform.position = hit.position;
          this._agent.Warp(hit.position);
          this._state = RatBigDemo.State.WAITING_PATH;
        }
        else
          this.putOnNavmeshTimeLeft = Random.Range(1f, 2f);
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
