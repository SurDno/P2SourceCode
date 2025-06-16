// Decompiled with JetBrains decompiler
// Type: NpcStateMoveBase
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Behaviours.Components;
using Engine.Common.Components;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Components.Utilities;
using Inspectors;
using UnityEngine;
using UnityEngine.AI;

#nullable disable
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
  private NpcStateMoveBase.StateEnum state = NpcStateMoveBase.StateEnum.WaitingPath;
  private bool movementFailed = false;
  private bool inited;
  private bool failed;

  public GameObject GameObject { get; private set; }

  [Inspected]
  private NavMeshAgentWrapper agentWrapper => new NavMeshAgentWrapper(this.agent);

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
    if (!this.movementFailed)
      this.movementFailed = failed;
    if (this.state == NpcStateMoveBase.StateEnum.Moving)
    {
      this.behavior.Move(this.agent.desiredVelocity, 0.0f);
      this.state = NpcStateMoveBase.StateEnum.Stopping;
    }
    else
      this.state = NpcStateMoveBase.StateEnum.Done;
  }

  protected void RestartMovement(Vector3 destination)
  {
    this.agent.ResetPath();
    this.agent.SetDestination(destination);
    this.state = NpcStateMoveBase.StateEnum.WaitingPath;
  }

  private bool TryInit()
  {
    if (this.inited)
      return true;
    this.behavior = this.pivot.GetBehavior();
    this.agent = this.pivot.GetAgent();
    this.animator = this.pivot.GetAnimator();
    this.rigidbody = this.pivot.GetRigidbody();
    this.weaponService = this.pivot.GetNpcWeaponService();
    this.failed = false;
    this.inited = true;
    return true;
  }

  public NpcStateMoveBase(NpcState npcState, Pivot pivot, bool infinite)
  {
    this.GameObject = npcState.gameObject;
    this.infinite = infinite;
    this.pivot = pivot;
    this.npcState = npcState;
  }

  public bool Activate()
  {
    if (!this.TryInit())
      return false;
    this.prevAreaMask = this.agent.areaMask;
    this.agentWasEnabled = this.agent.enabled;
    if ((Object) this.rigidbody != (Object) null)
    {
      this.rigidbodyWasKinematic = this.rigidbody.isKinematic;
      this.rigidbody.isKinematic = false;
      this.rigidbodyWasGravity = this.rigidbody.useGravity;
      this.rigidbody.useGravity = false;
    }
    this.Status = NpcStateStatusEnum.Running;
    if (this.npcState.Owner == null)
    {
      Debug.LogWarning((object) (this.GameObject.name + " : entity not found"));
      this.Status = NpcStateStatusEnum.Failed;
      return false;
    }
    LocationItemComponent component = (LocationItemComponent) this.npcState.Owner.GetComponent<ILocationItemComponent>();
    if (component == null)
    {
      Debug.LogWarning((object) (this.GameObject.name + ": location component not found"));
      this.Status = NpcStateStatusEnum.Failed;
      return false;
    }
    bool isIndoor = component.IsIndoor;
    NPCStateHelper.SetAgentAreaMask(this.agent, isIndoor);
    this.agent.enabled = true;
    this.state = NpcStateMoveBase.StateEnum.WaitingPath;
    if (!this.agent.isOnNavMesh)
    {
      Vector3 position = this.GameObject.transform.position;
      if (NavMeshUtility.SampleRaycastPosition(ref position, isIndoor ? 1f : 5f, isIndoor ? 2f : 10f, this.agent.areaMask))
        this.agent.Warp(position);
      else
        this.Status = NpcStateStatusEnum.Failed;
    }
    if ((Object) this.weaponService != (Object) null)
      this.weaponService.Weapon = WeaponEnum.Unknown;
    return true;
  }

  public void Shutdown()
  {
    if (this.failed)
      return;
    this.agent.areaMask = this.prevAreaMask;
    this.agent.enabled = this.agentWasEnabled;
    if ((Object) this.rigidbody != (Object) null)
    {
      this.rigidbody.isKinematic = this.rigidbodyWasKinematic;
      this.rigidbody.useGravity = this.rigidbodyWasGravity;
    }
    this.DoShutdown();
    if (!((Object) this.weaponService != (Object) null))
      return;
    this.weaponService.Weapon = this.npcState.Weapon;
  }

  public void OnAnimatorMove()
  {
    if (this.failed)
      return;
    this.behavior.OnExternalAnimatorMove();
  }

  public void OnAnimatorEventEvent(string obj)
  {
    if (!this.failed)
      ;
  }

  public void Update()
  {
    if (this.failed)
      return;
    if (InstanceByRequest<EngineApplication>.Instance.IsPaused)
    {
      if (!((Object) this.agent != (Object) null))
        return;
      this.agent.velocity = Vector3.zero;
    }
    else
    {
      if (this.Status != 0 || this.MovementPaused)
        return;
      this.DoUpdate();
      if (this.state == NpcStateMoveBase.StateEnum.WaitingPath)
        this.OnUpdateWaitPath();
      if (this.state == NpcStateMoveBase.StateEnum.Moving)
        this.OnUpdateMove();
      if (this.state == NpcStateMoveBase.StateEnum.Stopping)
        this.OnUpdateStopMovement();
      if (this.state == NpcStateMoveBase.StateEnum.StopAndRestartPath)
        this.OnUpdateStopAndRestartPath();
      if (this.state != NpcStateMoveBase.StateEnum.Done)
        return;
      if (this.movementFailed)
        this.Status = NpcStateStatusEnum.Failed;
      else if (!this.infinite)
        this.Status = NpcStateStatusEnum.Success;
    }
  }

  private void OnUpdateWaitPath()
  {
    if (this.agent.pathPending)
      return;
    if ((double) Random.value < (double) Time.deltaTime / 0.5 && NavMeshUtility.IsBrokenPath(this.agent))
    {
      Debug.Log((object) ObjectInfoUtility.GetStream().Append("[Navigation]").Append("  broken path detected, trying to reset: ").GetInfo((object) this.npcState.Owner), (Object) this.GameObject);
      Vector3 destination = this.agent.destination;
      this.agent.ResetPath();
      this.agent.SetDestination(destination);
      this.state = NpcStateMoveBase.StateEnum.WaitingPath;
    }
    else if (this.agent.pathStatus == NavMeshPathStatus.PathInvalid)
    {
      Vector3 destination = this.agent.destination;
      this.agent.ResetPath();
      this.agent.SetDestination(destination);
      this.state = NpcStateMoveBase.StateEnum.WaitingPath;
    }
    else if (!NavMeshUtility.HasPathNoGarbage(this.agent) || (double) Random.value < (double) Time.deltaTime / 0.5 && !NavMeshUtility.HasPathWithGarbage(this.agent))
    {
      Debug.LogWarningFormat("{0} : agent.path.corners.Length == 0, distance to destination = {1}", (object) this.GameObject.name, (object) (this.GameObject.transform.position - this.agent.destination).magnitude);
      this.CompleteTask(false);
    }
    else
    {
      float stoppingDistance = this.agent.stoppingDistance;
      if ((double) this.agent.desiredVelocity.magnitude < 0.0099999997764825821)
      {
        if ((double) this.agent.remainingDistance < (double) stoppingDistance)
          this.CompleteTask(false);
        Vector3 destination = this.agent.destination;
        this.agent.ResetPath();
        this.agent.SetDestination(destination);
      }
      else
      {
        this.state = NpcStateMoveBase.StateEnum.Moving;
        this.behavior.StartMovement(this.agent.desiredVelocity.normalized);
      }
    }
  }

  private void OnUpdateMove()
  {
    if ((double) Random.value < (double) Time.deltaTime / 0.5 && NavMeshUtility.IsBrokenPath(this.agent))
    {
      Debug.Log((object) ObjectInfoUtility.GetStream().Append("[Navigation]").Append("  broken path detected, trying to reset: ").GetInfo((object) this.npcState.Owner), (Object) this.GameObject);
      this.state = NpcStateMoveBase.StateEnum.StopAndRestartPath;
    }
    else
    {
      float num = this.agent.stoppingDistance * 3f;
      if (!this.agent.hasPath || !this.behavior.Move(this.agent.desiredVelocity, this.agent.remainingDistance) || (double) this.agent.remainingDistance >= (double) num)
        return;
      this.agent.ResetPath();
      this.state = NpcStateMoveBase.StateEnum.Done;
    }
  }

  public void OnUpdateStopAndRestartPath()
  {
    if (!this.behavior.Move(this.agent.desiredVelocity, 0.0f))
      return;
    Vector3 destination = this.agent.destination;
    this.agent.ResetPath();
    this.agent.SetDestination(destination);
    this.state = NpcStateMoveBase.StateEnum.WaitingPath;
  }

  private void OnUpdateStopMovement()
  {
    if (!this.behavior.Move(this.agent.desiredVelocity, 0.0f))
      return;
    this.state = NpcStateMoveBase.StateEnum.Done;
  }

  public void OnLodStateChanged(bool inLodState)
  {
    this.npcState.AnimatorEnabled = !inLodState;
    EffectsComponent component = this.npcState.Owner?.GetComponent<EffectsComponent>();
    if (component != null)
      component.Disabled = inLodState;
    this.agent.updatePosition = inLodState;
    this.agent.updateRotation = inLodState;
    this.agent.speed = this.behavior.Gait == EngineBehavior.GaitType.Walk ? 1.5f : 3f;
    this.agent.acceleration = 10f;
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
