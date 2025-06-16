using Engine.Behaviours.Components;
using Engine.Common.Components;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Components.Utilities;
using Inspectors;
using UnityEngine;
using UnityEngine.AI;

public class NpcStateMoveBase : INpcState
{
  protected NpcState npcState;
  protected Pivot pivot;
  protected EngineBehavior behavior;
  protected Animator animator;
  protected NavMeshAgent agent;
  protected Rigidbody rigidbody;
  private NPCWeaponService weaponService;
  private bool infinite;
  private int prevAreaMask;
  private bool agentWasEnabled;
  private bool rigidbodyWasKinematic;
  private bool rigidbodyWasGravity;
  private StateEnum state = StateEnum.WaitingPath;
  private bool movementFailed;
  private bool inited;
  private bool failed;

  public GameObject GameObject { get; private set; }

  [Inspected]
  private NavMeshAgentWrapper agentWrapper => new NavMeshAgentWrapper(agent);

  public virtual bool MovementPaused => false;

  public virtual void DoUpdate()
  {
  }

  public virtual void DoShutdown()
  {
  }

  [Inspected]
  public NpcStateStatusEnum Status { get; protected set; }

  protected void CompleteTask(bool failed)
  {
    if (!movementFailed)
      movementFailed = failed;
    if (state == StateEnum.Moving)
    {
      behavior.Move(agent.desiredVelocity, 0.0f);
      state = StateEnum.Stopping;
    }
    else
      state = StateEnum.Done;
  }

  protected void RestartMovement(Vector3 destination)
  {
    agent.ResetPath();
    agent.SetDestination(destination);
    state = StateEnum.WaitingPath;
  }

  private bool TryInit()
  {
    if (inited)
      return true;
    behavior = pivot.GetBehavior();
    agent = pivot.GetAgent();
    animator = pivot.GetAnimator();
    rigidbody = pivot.GetRigidbody();
    weaponService = pivot.GetNpcWeaponService();
    failed = false;
    inited = true;
    return true;
  }

  public NpcStateMoveBase(NpcState npcState, Pivot pivot, bool infinite)
  {
    GameObject = npcState.gameObject;
    this.infinite = infinite;
    this.pivot = pivot;
    this.npcState = npcState;
  }

  public bool Activate()
  {
    if (!TryInit())
      return false;
    prevAreaMask = agent.areaMask;
    agentWasEnabled = agent.enabled;
    if (rigidbody != null)
    {
      rigidbodyWasKinematic = rigidbody.isKinematic;
      rigidbody.isKinematic = false;
      rigidbodyWasGravity = rigidbody.useGravity;
      rigidbody.useGravity = false;
    }
    Status = NpcStateStatusEnum.Running;
    if (npcState.Owner == null)
    {
      Debug.LogWarning(GameObject.name + " : entity not found");
      Status = NpcStateStatusEnum.Failed;
      return false;
    }
    LocationItemComponent component = (LocationItemComponent) npcState.Owner.GetComponent<ILocationItemComponent>();
    if (component == null)
    {
      Debug.LogWarning(GameObject.name + ": location component not found");
      Status = NpcStateStatusEnum.Failed;
      return false;
    }
    bool isIndoor = component.IsIndoor;
    NPCStateHelper.SetAgentAreaMask(agent, isIndoor);
    agent.enabled = true;
    state = StateEnum.WaitingPath;
    if (!agent.isOnNavMesh)
    {
      Vector3 position = GameObject.transform.position;
      if (NavMeshUtility.SampleRaycastPosition(ref position, isIndoor ? 1f : 5f, isIndoor ? 2f : 10f, agent.areaMask))
        agent.Warp(position);
      else
        Status = NpcStateStatusEnum.Failed;
    }
    if (weaponService != null)
      weaponService.Weapon = WeaponEnum.Unknown;
    return true;
  }

  public void Shutdown()
  {
    if (failed)
      return;
    agent.areaMask = prevAreaMask;
    agent.enabled = agentWasEnabled;
    if (rigidbody != null)
    {
      rigidbody.isKinematic = rigidbodyWasKinematic;
      rigidbody.useGravity = rigidbodyWasGravity;
    }
    DoShutdown();
    if (!(weaponService != null))
      return;
    weaponService.Weapon = npcState.Weapon;
  }

  public void OnAnimatorMove()
  {
    if (failed)
      return;
    behavior.OnExternalAnimatorMove();
  }

  public void OnAnimatorEventEvent(string obj)
  {
    if (!failed)
      ;
  }

  public void Update()
  {
    if (failed)
      return;
    if (InstanceByRequest<EngineApplication>.Instance.IsPaused)
    {
      if (!(agent != null))
        return;
      agent.velocity = Vector3.zero;
    }
    else
    {
      if (Status != 0 || MovementPaused)
        return;
      DoUpdate();
      if (state == StateEnum.WaitingPath)
        OnUpdateWaitPath();
      if (state == StateEnum.Moving)
        OnUpdateMove();
      if (state == StateEnum.Stopping)
        OnUpdateStopMovement();
      if (state == StateEnum.StopAndRestartPath)
        OnUpdateStopAndRestartPath();
      if (state != StateEnum.Done)
        return;
      if (movementFailed)
        Status = NpcStateStatusEnum.Failed;
      else if (!infinite)
        Status = NpcStateStatusEnum.Success;
    }
  }

  private void OnUpdateWaitPath()
  {
    if (agent.pathPending)
      return;
    if (Random.value < Time.deltaTime / 0.5 && NavMeshUtility.IsBrokenPath(agent))
    {
      Debug.Log(ObjectInfoUtility.GetStream().Append("[Navigation]").Append("  broken path detected, trying to reset: ").GetInfo(npcState.Owner), GameObject);
      Vector3 destination = agent.destination;
      agent.ResetPath();
      agent.SetDestination(destination);
      state = StateEnum.WaitingPath;
    }
    else if (agent.pathStatus == NavMeshPathStatus.PathInvalid)
    {
      Vector3 destination = agent.destination;
      agent.ResetPath();
      agent.SetDestination(destination);
      state = StateEnum.WaitingPath;
    }
    else if (!NavMeshUtility.HasPathNoGarbage(agent) || Random.value < Time.deltaTime / 0.5 && !NavMeshUtility.HasPathWithGarbage(agent))
    {
      Debug.LogWarningFormat("{0} : agent.path.corners.Length == 0, distance to destination = {1}", GameObject.name, (GameObject.transform.position - agent.destination).magnitude);
      CompleteTask(false);
    }
    else
    {
      float stoppingDistance = agent.stoppingDistance;
      if (agent.desiredVelocity.magnitude < 0.0099999997764825821)
      {
        if (agent.remainingDistance < (double) stoppingDistance)
          CompleteTask(false);
        Vector3 destination = agent.destination;
        agent.ResetPath();
        agent.SetDestination(destination);
      }
      else
      {
        state = StateEnum.Moving;
        behavior.StartMovement(agent.desiredVelocity.normalized);
      }
    }
  }

  private void OnUpdateMove()
  {
    if (Random.value < Time.deltaTime / 0.5 && NavMeshUtility.IsBrokenPath(agent))
    {
      Debug.Log(ObjectInfoUtility.GetStream().Append("[Navigation]").Append("  broken path detected, trying to reset: ").GetInfo(npcState.Owner), GameObject);
      state = StateEnum.StopAndRestartPath;
    }
    else
    {
      float num = agent.stoppingDistance * 3f;
      if (!agent.hasPath || !behavior.Move(agent.desiredVelocity, agent.remainingDistance) || agent.remainingDistance >= (double) num)
        return;
      agent.ResetPath();
      state = StateEnum.Done;
    }
  }

  public void OnUpdateStopAndRestartPath()
  {
    if (!behavior.Move(agent.desiredVelocity, 0.0f))
      return;
    Vector3 destination = agent.destination;
    agent.ResetPath();
    agent.SetDestination(destination);
    state = StateEnum.WaitingPath;
  }

  private void OnUpdateStopMovement()
  {
    if (!behavior.Move(agent.desiredVelocity, 0.0f))
      return;
    state = StateEnum.Done;
  }

  public void OnLodStateChanged(bool inLodState)
  {
    npcState.AnimatorEnabled = !inLodState;
    EffectsComponent component = npcState.Owner?.GetComponent<EffectsComponent>();
    if (component != null)
      component.Disabled = inLodState;
    agent.updatePosition = inLodState;
    agent.updateRotation = inLodState;
    agent.speed = behavior.Gait == EngineBehavior.GaitType.Walk ? 1.5f : 3f;
    agent.acceleration = 10f;
  }

  private enum StateEnum
  {
    WaitingPath,
    Moving,
    Stopping,
    StopAndRestartPath,
    Done,
  }
}
