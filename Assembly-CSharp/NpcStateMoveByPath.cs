using System.Collections.Generic;
using Engine.Behaviours.Components;
using Engine.Source.Commons;
using Engine.Source.Components.Utilities;
using Inspectors;

public class NpcStateMoveByPath : INpcState
{
  private StateEnum state;
  private bool failed;
  private NpcState npcState;
  private Pivot pivot;
  private EngineBehavior behavior;
  private NavMeshAgent agent;
  private NPCWeaponService weaponService;
  private bool agentWasEnabled;
  [Inspected]
  private List<Vector3> path;
  [Inspected]
  private int currentIndex;
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
    if (inited)
      return true;
    behavior = pivot.GetBehavior();
    agent = pivot.GetAgent();
    weaponService = pivot.GetNpcWeaponService();
    inited = true;
    return true;
  }

  public NpcStateMoveByPath(NpcState npcState, Pivot pivot)
  {
    GameObject = npcState.gameObject;
    this.pivot = pivot;
    this.npcState = npcState;
  }

  public void Activate(List<Vector3> path)
  {
    failed = false;
    currentIndex = 0;
    Status = NpcStateStatusEnum.Running;
    if (!TryInit())
      return;
    this.path = path;
    movementStarted = false;
    agentWasEnabled = agent.enabled;
    agent.enabled = true;
    if (!agent.isOnNavMesh)
    {
      Vector3 position = GameObject.transform.position;
      bool flag = false;
      if (NavMeshUtility.SampleRaycastPosition(ref position, flag ? 1f : 5f, flag ? 2f : 10f, agent.areaMask))
      {
        agent.Warp(position);
      }
      else
      {
        Debug.Log((object) "Can't sample navmesh", (Object) GameObject);
        Status = NpcStateStatusEnum.Failed;
        return;
      }
    }
    GoToNextPoint();
    if (!((Object) weaponService != (Object) null))
      return;
    weaponService.Weapon = WeaponEnum.Unknown;
  }

  private void GoToNextPoint()
  {
    currentPoint = path[currentIndex];
    ++currentIndex;
    state = StateEnum.Moving;
    RecountRemainingDistance();
  }

  private void RecountRemainingDistance()
  {
    remainingPointsDistance = 0.0f;
    for (int currentIndex = this.currentIndex; currentIndex < path.Count; ++currentIndex)
      remainingPointsDistance += (path[currentIndex - 1] - path[currentIndex]).magnitude;
  }

  public void Shutdown()
  {
    if (failed)
      return;
    agent.enabled = agentWasEnabled;
    if (!((Object) weaponService != (Object) null))
      return;
    weaponService.Weapon = npcState.Weapon;
  }

  public void OnAnimatorMove() => behavior.OnExternalAnimatorMove();

  public void OnAnimatorEventEvent(string obj)
  {
    if (!failed)
      ;
  }

  public void Update()
  {
    if (InstanceByRequest<EngineApplication>.Instance.IsPaused || !inited)
      return;
    if (state == StateEnum.Moving)
      OnUpdateMove();
    if (state == StateEnum.Stopping)
      OnUpdateStopMovement();
    if (state != StateEnum.Done)
      return;
    Status = failed ? NpcStateStatusEnum.Failed : NpcStateStatusEnum.Success;
  }

  public void OnUpdateMove()
  {
    Vector3 direction = currentPoint - GameObject.transform.position;
    float magnitude = direction.magnitude;
    float stoppingDistance = agent.stoppingDistance;
    if (currentIndex != path.Count)
      stoppingDistance *= 1.5f;
    if (!movementStarted)
    {
      behavior.StartMovement(direction.normalized);
      movementStarted = true;
    }
    else
    {
      bool flag = behavior.Move(direction, magnitude + remainingPointsDistance);
      if (!(magnitude < (double) stoppingDistance | flag))
        return;
      PointReached();
    }
  }

  public void OnUpdateStopMovement()
  {
    if (!behavior.Move(currentPoint - GameObject.transform.position, 0.0f))
      return;
    state = StateEnum.Done;
  }

  private void PointReached()
  {
    if (currentIndex == path.Count)
      state = StateEnum.Stopping;
    else
      GoToNextPoint();
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
