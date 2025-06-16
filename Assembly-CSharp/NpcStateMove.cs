// Decompiled with JetBrains decompiler
// Type: NpcStateMove
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
public class NpcStateMove : INpcState
{
  private NpcState npcState;
  private Pivot pivot;
  private Animator animator;
  private NPCWeaponService weaponService;
  private Rigidbody rigidbody;
  private AnimatorUpdateMode initialAnimatorUpdateMode;
  private AnimatorCullingMode initialAnimatorCullingMode;
  private bool agentWasEnabled;
  private bool rigidbodyWasKinematic;
  private bool rigidbodyWasGravity;
  [Inspected]
  private NpcStateMove.StateEnum state = NpcStateMove.StateEnum.WaitingPath;
  [Inspected]
  private bool failed = false;
  [Inspected]
  private NavMeshAgent agent;
  [Inspected]
  private bool failOnPartialPath;
  [Inspected]
  private bool pathIsPartial;
  [Inspected]
  private Vector3 destination;
  private EngineBehavior behavior;
  private int prevAreaMask;
  private bool inited;

  public GameObject GameObject { get; private set; }

  [Inspected]
  private NavMeshAgentWrapper agentWrapper => new NavMeshAgentWrapper(this.agent);

  [Inspected]
  public NpcStateStatusEnum Status { get; private set; }

  private bool TryInit()
  {
    if (this.inited)
      return true;
    this.behavior = this.pivot.GetBehavior();
    this.agent = this.pivot.GetAgent();
    this.animator = this.pivot.GetAnimator();
    this.weaponService = this.pivot.GetNpcWeaponService();
    this.rigidbody = this.pivot.GetRigidbody();
    this.inited = true;
    return true;
  }

  public NpcStateMove(NpcState npcState, Pivot pivot)
  {
    this.GameObject = npcState.gameObject;
    this.pivot = pivot;
    this.npcState = npcState;
  }

  public void Activate(Vector3 destination, bool failOnPartialPath = false)
  {
    this.failed = false;
    this.Status = NpcStateStatusEnum.Running;
    if (!this.TryInit())
      return;
    this.failOnPartialPath = failOnPartialPath;
    this.destination = destination;
    this.state = NpcStateMove.StateEnum.WaitingPath;
    this.prevAreaMask = this.agent.areaMask;
    this.agentWasEnabled = this.agent.enabled;
    this.pathIsPartial = false;
    bool indoor = true;
    if (this.npcState.Owner != null)
    {
      LocationItemComponent component = (LocationItemComponent) this.npcState.Owner.GetComponent<ILocationItemComponent>();
      if (component == null)
      {
        Debug.LogWarning((object) (this.GameObject.name + ": location component not found"));
        this.Status = NpcStateStatusEnum.Failed;
        return;
      }
      if (component != null)
        indoor = component.IsIndoor;
    }
    if ((Object) this.rigidbody != (Object) null)
    {
      this.rigidbodyWasKinematic = this.rigidbody.isKinematic;
      this.rigidbodyWasGravity = this.rigidbody.useGravity;
      this.rigidbody.useGravity = false;
      this.rigidbody.isKinematic = false;
    }
    NPCStateHelper.SetAgentAreaMask(this.agent, indoor);
    this.agent.enabled = true;
    if (!this.agent.isOnNavMesh)
    {
      Vector3 position = this.GameObject.transform.position;
      if (NavMeshUtility.SampleRaycastPosition(ref position, indoor ? 1f : 5f, indoor ? 2f : 10f, this.agent.areaMask))
      {
        this.agent.Warp(position);
      }
      else
      {
        this.Status = NpcStateStatusEnum.Failed;
        return;
      }
    }
    this.agent.SetDestination(destination);
    if ((Object) this.animator != (Object) null)
    {
      this.initialAnimatorUpdateMode = this.animator.updateMode;
      this.animator.updateMode = AnimatorUpdateMode.AnimatePhysics;
      this.initialAnimatorCullingMode = this.animator.cullingMode;
      this.animator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
    }
    if (!((Object) this.weaponService != (Object) null))
      return;
    this.weaponService.Weapon = WeaponEnum.Unknown;
  }

  public void Shutdown()
  {
    if (this.failed)
      return;
    if ((Object) this.animator != (Object) null)
    {
      this.animator.updateMode = this.initialAnimatorUpdateMode;
      this.animator.cullingMode = this.initialAnimatorCullingMode;
    }
    if ((Object) this.rigidbody != (Object) null)
    {
      this.rigidbody.isKinematic = this.rigidbodyWasKinematic;
      this.rigidbody.useGravity = this.rigidbodyWasGravity;
    }
    this.agent.areaMask = this.prevAreaMask;
    this.agent.enabled = this.agentWasEnabled;
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
    if (!this.inited)
      return;
    if (InstanceByRequest<EngineApplication>.Instance.IsPaused)
    {
      if (!((Object) this.agent != (Object) null))
        return;
      this.agent.velocity = Vector3.zero;
    }
    else
    {
      if (this.state == NpcStateMove.StateEnum.WaitingPath)
        this.OnUpdateWaitPath();
      if (this.state == NpcStateMove.StateEnum.Moving)
        this.OnUpdateMove();
      if (this.state == NpcStateMove.StateEnum.Stopping)
        this.OnUpdateStopMovement();
      if (this.state == NpcStateMove.StateEnum.StopAndRestartPath)
        this.OnUpdateStopAndRestartPath();
      if (this.state != NpcStateMove.StateEnum.Done)
        return;
      this.Status = this.failed ? NpcStateStatusEnum.Failed : NpcStateStatusEnum.Success;
    }
  }

  public void OnUpdateWaitPath()
  {
    if (this.agent.pathPending)
      return;
    if ((double) Random.value < (double) Time.deltaTime / 0.5 && NavMeshUtility.IsBrokenPath(this.agent))
    {
      Debug.Log((object) ObjectInfoUtility.GetStream().Append("[Navigation]").Append("  broken path detected, trying to reset: ").GetInfo((object) this.npcState.Owner), (Object) this.GameObject);
      Vector3 destination = this.agent.destination;
      this.agent.ResetPath();
      this.agent.SetDestination(destination);
      this.state = NpcStateMove.StateEnum.WaitingPath;
    }
    else
    {
      this.pathIsPartial = this.agent.pathStatus == NavMeshPathStatus.PathPartial;
      if (this.pathIsPartial && this.failOnPartialPath)
        this.CompleteTask(true);
      else if (this.agent.pathStatus == NavMeshPathStatus.PathInvalid)
      {
        Vector3 destination = this.agent.destination;
        this.agent.ResetPath();
        this.agent.SetDestination(destination);
        this.state = NpcStateMove.StateEnum.WaitingPath;
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
          this.agent.ResetPath();
          this.agent.SetDestination(this.destination);
        }
        else
        {
          this.state = NpcStateMove.StateEnum.Moving;
          this.behavior.StartMovement(this.agent.desiredVelocity.normalized);
        }
      }
    }
  }

  public void OnUpdateMove()
  {
    if ((double) Random.value < (double) Time.deltaTime / 0.5 && NavMeshUtility.IsBrokenPath(this.agent))
    {
      Debug.Log((object) ObjectInfoUtility.GetStream().Append("[Navigation]").Append("  broken path detected, trying to reset: ").GetInfo((object) this.npcState.Owner), (Object) this.GameObject);
      this.state = NpcStateMove.StateEnum.StopAndRestartPath;
    }
    else
    {
      float num = this.agent.stoppingDistance * 3f;
      if (!this.agent.hasPath || !this.behavior.Move(this.agent.desiredVelocity, this.agent.remainingDistance) || (double) this.agent.remainingDistance >= (double) num)
        return;
      this.agent.ResetPath();
      this.state = NpcStateMove.StateEnum.Stopping;
    }
  }

  public void OnUpdateStopAndRestartPath()
  {
    if (!this.behavior.Move(this.agent.desiredVelocity, 0.0f))
      return;
    Vector3 destination = this.agent.destination;
    this.agent.ResetPath();
    this.agent.SetDestination(destination);
    this.state = NpcStateMove.StateEnum.WaitingPath;
  }

  public void OnUpdateStopMovement()
  {
    if (!this.behavior.Move(this.agent.desiredVelocity, 0.0f))
      return;
    this.state = NpcStateMove.StateEnum.Done;
  }

  private void CompleteTask(bool failed)
  {
    if (!this.failed)
      this.failed = failed;
    if (this.state == NpcStateMove.StateEnum.Moving)
    {
      this.behavior.Move(this.agent.desiredVelocity, 0.0f);
      this.state = NpcStateMove.StateEnum.Stopping;
    }
    else
      this.state = NpcStateMove.StateEnum.Done;
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
