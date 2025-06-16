using Engine.Behaviours.Components;
using Engine.Source.Commons;
using Engine.Source.Components.Utilities;
using Inspectors;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NpcStateMoveByPath : INpcState
{
  private NpcStateMoveByPath.StateEnum state;
  private bool failed = false;
  private NpcState npcState;
  private Pivot pivot;
  private EngineBehavior behavior;
  private NavMeshAgent agent;
  private NPCWeaponService weaponService;
  private bool agentWasEnabled;
  [Inspected]
  private List<Vector3> path;
  [Inspected]
  private int currentIndex = 0;
  [Inspected]
  private Vector3 currentPoint;
  [Inspected]
  private float remainingPointsDistance;
  private bool movementStarted;
  private bool inited;

  public GameObject GameObject { get; private set; }

  public NpcStateStatusEnum Status { get; private set; }

  private bool TryInit()
  {
    if (this.inited)
      return true;
    this.behavior = this.pivot.GetBehavior();
    this.agent = this.pivot.GetAgent();
    this.weaponService = this.pivot.GetNpcWeaponService();
    this.inited = true;
    return true;
  }

  public NpcStateMoveByPath(NpcState npcState, Pivot pivot)
  {
    this.GameObject = npcState.gameObject;
    this.pivot = pivot;
    this.npcState = npcState;
  }

  public void Activate(List<Vector3> path)
  {
    this.failed = false;
    this.currentIndex = 0;
    this.Status = NpcStateStatusEnum.Running;
    if (!this.TryInit())
      return;
    this.path = path;
    this.movementStarted = false;
    this.agentWasEnabled = this.agent.enabled;
    this.agent.enabled = true;
    if (!this.agent.isOnNavMesh)
    {
      Vector3 position = this.GameObject.transform.position;
      bool flag = false;
      if (NavMeshUtility.SampleRaycastPosition(ref position, flag ? 1f : 5f, flag ? 2f : 10f, this.agent.areaMask))
      {
        this.agent.Warp(position);
      }
      else
      {
        Debug.Log((object) "Can't sample navmesh", (Object) this.GameObject);
        this.Status = NpcStateStatusEnum.Failed;
        return;
      }
    }
    this.GoToNextPoint();
    if (!((Object) this.weaponService != (Object) null))
      return;
    this.weaponService.Weapon = WeaponEnum.Unknown;
  }

  private void GoToNextPoint()
  {
    this.currentPoint = this.path[this.currentIndex];
    ++this.currentIndex;
    this.state = NpcStateMoveByPath.StateEnum.Moving;
    this.RecountRemainingDistance();
  }

  private void RecountRemainingDistance()
  {
    this.remainingPointsDistance = 0.0f;
    for (int currentIndex = this.currentIndex; currentIndex < this.path.Count; ++currentIndex)
      this.remainingPointsDistance += (this.path[currentIndex - 1] - this.path[currentIndex]).magnitude;
  }

  public void Shutdown()
  {
    if (this.failed)
      return;
    this.agent.enabled = this.agentWasEnabled;
    if (!((Object) this.weaponService != (Object) null))
      return;
    this.weaponService.Weapon = this.npcState.Weapon;
  }

  public void OnAnimatorMove() => this.behavior.OnExternalAnimatorMove();

  public void OnAnimatorEventEvent(string obj)
  {
    if (!this.failed)
      ;
  }

  public void Update()
  {
    if (InstanceByRequest<EngineApplication>.Instance.IsPaused || !this.inited)
      return;
    if (this.state == NpcStateMoveByPath.StateEnum.Moving)
      this.OnUpdateMove();
    if (this.state == NpcStateMoveByPath.StateEnum.Stopping)
      this.OnUpdateStopMovement();
    if (this.state != NpcStateMoveByPath.StateEnum.Done)
      return;
    this.Status = this.failed ? NpcStateStatusEnum.Failed : NpcStateStatusEnum.Success;
  }

  public void OnUpdateMove()
  {
    Vector3 direction = this.currentPoint - this.GameObject.transform.position;
    float magnitude = direction.magnitude;
    float stoppingDistance = this.agent.stoppingDistance;
    if (this.currentIndex != this.path.Count)
      stoppingDistance *= 1.5f;
    if (!this.movementStarted)
    {
      this.behavior.StartMovement(direction.normalized);
      this.movementStarted = true;
    }
    else
    {
      bool flag = this.behavior.Move(direction, magnitude + this.remainingPointsDistance);
      if (!((double) magnitude < (double) stoppingDistance | flag))
        return;
      this.PointReached();
    }
  }

  public void OnUpdateStopMovement()
  {
    if (!this.behavior.Move(this.currentPoint - this.GameObject.transform.position, 0.0f))
      return;
    this.state = NpcStateMoveByPath.StateEnum.Done;
  }

  private void PointReached()
  {
    if (this.currentIndex == this.path.Count)
      this.state = NpcStateMoveByPath.StateEnum.Stopping;
    else
      this.GoToNextPoint();
  }

  public void OnLodStateChanged(bool enabled)
  {
  }

  private enum StateEnum
  {
    Moving,
    Stopping,
    Done,
  }
}
