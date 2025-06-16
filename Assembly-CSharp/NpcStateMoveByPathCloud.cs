using Engine.Behaviours.Components;
using Engine.Source.Commons;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NpcStateMoveByPathCloud : INpcState
{
  private NpcStateMoveByPathCloud.StateEnum state;
  private bool failed = false;
  private NpcState npcState;
  private Pivot pivot;
  private NavMeshAgent agent;
  private bool agentWasEnabled;
  private List<Vector3> path;
  private int currentIndex = 0;
  private Vector3 currentPoint;
  private float remainingPointsDistance;
  private float speed = 2f;
  private bool inited;

  public GameObject GameObject { get; private set; }

  public NpcStateStatusEnum Status { get; private set; }

  private bool TryInit()
  {
    if (this.inited)
      return true;
    this.agent = this.pivot.GetAgent();
    if ((Object) this.agent != (Object) null)
      this.speed = this.agent.speed;
    this.inited = true;
    return true;
  }

  public NpcStateMoveByPathCloud(NpcState npcState, Pivot pivot)
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
    this.agentWasEnabled = this.agent.enabled;
    this.agent.enabled = false;
    this.path = path;
    this.GoToNextPoint();
  }

  private void GoToNextPoint()
  {
    this.currentPoint = this.path[this.currentIndex];
    ++this.currentIndex;
    this.state = NpcStateMoveByPathCloud.StateEnum.Moving;
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
  }

  public void OnAnimatorMove()
  {
  }

  public void OnAnimatorEventEvent(string obj)
  {
    if (!this.failed)
      ;
  }

  public void Update()
  {
    if (InstanceByRequest<EngineApplication>.Instance.IsPaused || !this.inited)
      return;
    if (this.state == NpcStateMoveByPathCloud.StateEnum.Moving)
      this.OnUpdateMove();
    if (this.state != NpcStateMoveByPathCloud.StateEnum.Done)
      return;
    this.Status = this.failed ? NpcStateStatusEnum.Failed : NpcStateStatusEnum.Success;
  }

  public void OnUpdateMove()
  {
    Vector3 vector3 = this.currentPoint - this.GameObject.transform.position;
    float magnitude = vector3.magnitude;
    float num = 0.5f;
    this.GameObject.transform.position += vector3.normalized * this.speed * Time.deltaTime;
    if ((double) magnitude >= (double) num)
      return;
    this.PointReached();
  }

  private void PointReached()
  {
    if (this.currentIndex == this.path.Count)
      this.state = NpcStateMoveByPathCloud.StateEnum.Done;
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
