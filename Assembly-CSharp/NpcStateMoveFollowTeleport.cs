using Engine.Behaviours.Components;
using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Services;
using UnityEngine;
using UnityEngine.AI;

public class NpcStateMoveFollowTeleport : NpcStateMoveBase
{
  private float followDistance;
  private bool waitForTargetSeesMe;
  private Vector3 lastDestination;
  private bool wasRestartBehaviourAfterTeleport;

  public NpcStateMoveFollowTeleport(NpcState npcState, Pivot pivot)
    : base(npcState, pivot, true)
  {
  }

  public void Activate(float followDistance, float trialTime, bool waitForTargetSeesMe)
  {
    if (!this.Activate())
      return;
    this.waitForTargetSeesMe = waitForTargetSeesMe;
    this.wasRestartBehaviourAfterTeleport = this.npcState.RestartBehaviourAfterTeleport;
    this.followDistance = followDistance;
    this.lastDestination = Vector3.one * 100000f;
  }

  public override void DoShutdown()
  {
    this.npcState.RestartBehaviourAfterTeleport = this.wasRestartBehaviourAfterTeleport;
  }

  private GameObject GetPlayer()
  {
    IEntity player = ServiceLocator.GetService<ISimulation>().Player;
    if (player == null)
      return (GameObject) null;
    LocationItemComponent component1 = player.GetComponent<LocationItemComponent>();
    if (component1 == null || component1.IsIndoor)
      return (GameObject) null;
    PlayerControllerComponent component2 = player.GetComponent<PlayerControllerComponent>();
    return component2 != null && !component2.CanReceiveMail.Value ? (GameObject) null : ((IEntityView) player).GameObject;
  }

  public override bool MovementPaused => false;

  public override void DoUpdate()
  {
    GameObject player = this.GetPlayer();
    if ((Object) player == (Object) null || this.Status != 0)
      return;
    PostmanTeleportService service = ServiceLocator.GetService<PostmanTeleportService>();
    Vector3 vector3 = player.transform.position - this.GameObject.transform.position;
    float magnitude1 = vector3.magnitude;
    vector3 = player.transform.position - this.GameObject.transform.position;
    if ((double) vector3.magnitude < (double) this.followDistance && this.TargetIsVisible(player))
    {
      if (!this.waitForTargetSeesMe || this.TargetSeesMe(player))
        this.Status = NpcStateStatusEnum.Success;
      else
        service.ReportPostmanIsOK(this.npcState.Owner, true);
    }
    else
    {
      vector3 = this.lastDestination - player.transform.position;
      float magnitude2 = vector3.magnitude;
      vector3 = player.transform.position - this.GameObject.transform.position;
      float magnitude3 = vector3.magnitude;
      if (this.behavior.Gait == EngineBehavior.GaitType.Run)
      {
        if ((double) magnitude3 < (double) this.followDistance * 3.0)
          this.behavior.Gait = EngineBehavior.GaitType.Walk;
        if ((double) magnitude2 > (double) this.followDistance)
        {
          this.lastDestination = player.transform.position;
          this.RestartMovement(this.lastDestination);
        }
      }
      else
      {
        if ((double) magnitude3 > (double) this.followDistance * 4.0)
          this.behavior.Gait = EngineBehavior.GaitType.Run;
        if ((double) magnitude2 > 0.30000001192092896 * (double) this.followDistance)
        {
          this.lastDestination = player.transform.position;
          this.RestartMovement(this.lastDestination);
        }
      }
      if (this.agent.hasPath && (double) this.agent.remainingDistance < (double) this.followDistance && (double) magnitude3 < (double) this.followDistance)
      {
        if (!this.waitForTargetSeesMe || this.TargetSeesMe(player))
          this.Status = NpcStateStatusEnum.Success;
        this.CompleteTask(false);
        service.ReportPostmanIsOK(this.npcState.Owner, true);
      }
      else
      {
        if ((double) magnitude1 >= 50.0)
          return;
        service.ReportPostmanIsOK(this.npcState.Owner, true);
      }
    }
  }

  private bool TargetSeesMe(GameObject target)
  {
    return (double) Vector3.Angle((this.GameObject.transform.position - target.transform.position).normalized, target.transform.forward) < 30.0;
  }

  private bool TargetIsVisible(GameObject target)
  {
    return !NavMesh.Raycast(this.GameObject.transform.position, target.transform.position, out NavMeshHit _, -1);
  }
}
