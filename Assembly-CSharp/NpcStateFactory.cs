using System;
using Engine.Behaviours.Components;

internal class NpcStateFactory
{
  public static INpcState Create(NpcStateEnum npcStateEnum, NpcState state, Pivot pivot)
  {
    switch (npcStateEnum)
    {
      case NpcStateEnum.Unknown:
        return new NpcStateUnknown(state, pivot);
      case NpcStateEnum.Idle:
        return new NpcStateIdle(state, pivot);
      case NpcStateEnum.IdleKinematic:
        return new NpcStateIdleKinematic(state, pivot);
      case NpcStateEnum.IdlePlagueCloud:
        return new NpcStateIdlePlagueCloud(state, pivot);
      case NpcStateEnum.Rotate:
        return new NpcStateRotate(state, pivot);
      case NpcStateEnum.Move:
        return new NpcStateMove(state, pivot);
      case NpcStateEnum.MoveCloud:
        return new NpcStateMoveCloud(state, pivot);
      case NpcStateEnum.MoveRetreat:
        return new NpcStateMoveRetreat(state, pivot);
      case NpcStateEnum.MoveFollow:
        return new NpcStateMoveFollow(state, pivot);
      case NpcStateEnum.MoveByPath:
        return new NpcStateMoveByPath(state, pivot);
      case NpcStateEnum.MoveByPathCloud:
        return new NpcStateMoveByPathCloud(state, pivot);
      case NpcStateEnum.MoveFollowTeleport:
        return new NpcStateMoveFollowTeleport(state, pivot);
      case NpcStateEnum.MoveFollowTeleportStationary:
        return new NpcStateMoveFollowTeleportStationary(state, pivot);
      case NpcStateEnum.PointOfInterest:
        return new NpcStatePointOfInterest(state, pivot);
      case NpcStateEnum.PresetIdle:
        return new NpcStateIdlePreset(state, pivot);
      case NpcStateEnum.PresetIdleTest:
        return new NpcStateIdlePresetTest(state, pivot);
      case NpcStateEnum.POIExtraExit:
        return new NPCStatePOIExtraExit(state, pivot);
      case NpcStateEnum.FightIdle:
        return new NpcStateFightIdle(state, pivot);
      case NpcStateEnum.FightFollow:
        return new NpcStateFightFollow(state, pivot);
      case NpcStateEnum.FightKeepDistance:
        return new NpcStateFightKeepDistance(state, pivot);
      case NpcStateEnum.FightSurrender:
        return new NpcStateFightSurrender(state, pivot);
      case NpcStateEnum.FightSurrenderLoot:
        return new NpcStateFightSurrenderLoot(state, pivot);
      case NpcStateEnum.FightEscape:
        return new NpcStateFightEscape(state, pivot);
      case NpcStateEnum.Ragdoll:
        return new NpcStateRagdoll(state, pivot);
      case NpcStateEnum.RagdollRessurect:
        return new NpcStateRagdollRessurect(state, pivot);
      case NpcStateEnum.InFire:
        return new NpcStateInFire(state, pivot);
      case NpcStateEnum.DialogNpc:
        return new NpcStateDialogNpc(state, pivot);
      case NpcStateEnum.FightFollowTarget:
        return new NpcStateFightFollowTarget(state, pivot);
      default:
        throw new NotImplementedException();
    }
  }
}
