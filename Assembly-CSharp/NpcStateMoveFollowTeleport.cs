using Engine.Behaviours.Components;
using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Services;
using UnityEngine;
using UnityEngine.AI;

public class NpcStateMoveFollowTeleport(NpcState npcState, Pivot pivot) : NpcStateMoveBase(npcState, pivot, true) {
  private float followDistance;
  private bool waitForTargetSeesMe;
  private Vector3 lastDestination;
  private bool wasRestartBehaviourAfterTeleport;

  public void Activate(float followDistance, float trialTime, bool waitForTargetSeesMe)
  {
    if (!Activate())
      return;
    this.waitForTargetSeesMe = waitForTargetSeesMe;
    wasRestartBehaviourAfterTeleport = npcState.RestartBehaviourAfterTeleport;
    this.followDistance = followDistance;
    lastDestination = Vector3.one * 100000f;
  }

  public override void DoShutdown()
  {
    npcState.RestartBehaviourAfterTeleport = wasRestartBehaviourAfterTeleport;
  }

  private GameObject GetPlayer()
  {
    IEntity player = ServiceLocator.GetService<ISimulation>().Player;
    if (player == null)
      return null;
    LocationItemComponent component1 = player.GetComponent<LocationItemComponent>();
    if (component1 == null || component1.IsIndoor)
      return null;
    PlayerControllerComponent component2 = player.GetComponent<PlayerControllerComponent>();
    return component2 != null && !component2.CanReceiveMail.Value ? null : ((IEntityView) player).GameObject;
  }

  public override bool MovementPaused => false;

  public override void DoUpdate()
  {
    GameObject player = GetPlayer();
    if (player == null || Status != 0)
      return;
    PostmanTeleportService service = ServiceLocator.GetService<PostmanTeleportService>();
    Vector3 vector3 = player.transform.position - GameObject.transform.position;
    float magnitude1 = vector3.magnitude;
    vector3 = player.transform.position - GameObject.transform.position;
    if (vector3.magnitude < (double) followDistance && TargetIsVisible(player))
    {
      if (!waitForTargetSeesMe || TargetSeesMe(player))
        Status = NpcStateStatusEnum.Success;
      else
        service.ReportPostmanIsOK(npcState.Owner, true);
    }
    else
    {
      vector3 = lastDestination - player.transform.position;
      float magnitude2 = vector3.magnitude;
      vector3 = player.transform.position - GameObject.transform.position;
      float magnitude3 = vector3.magnitude;
      if (behavior.Gait == EngineBehavior.GaitType.Run)
      {
        if (magnitude3 < followDistance * 3.0)
          behavior.Gait = EngineBehavior.GaitType.Walk;
        if (magnitude2 > (double) followDistance)
        {
          lastDestination = player.transform.position;
          RestartMovement(lastDestination);
        }
      }
      else
      {
        if (magnitude3 > followDistance * 4.0)
          behavior.Gait = EngineBehavior.GaitType.Run;
        if (magnitude2 > 0.30000001192092896 * followDistance)
        {
          lastDestination = player.transform.position;
          RestartMovement(lastDestination);
        }
      }
      if (agent.hasPath && agent.remainingDistance < (double) followDistance && magnitude3 < (double) followDistance)
      {
        if (!waitForTargetSeesMe || TargetSeesMe(player))
          Status = NpcStateStatusEnum.Success;
        CompleteTask(false);
        service.ReportPostmanIsOK(npcState.Owner, true);
      }
      else
      {
        if (magnitude1 >= 50.0)
          return;
        service.ReportPostmanIsOK(npcState.Owner, true);
      }
    }
  }

  private bool TargetSeesMe(GameObject target)
  {
    return Vector3.Angle((GameObject.transform.position - target.transform.position).normalized, target.transform.forward) < 30.0;
  }

  private bool TargetIsVisible(GameObject target)
  {
    return !NavMesh.Raycast(GameObject.transform.position, target.transform.position, out NavMeshHit _, -1);
  }
}
