using Engine.Behaviours.Components;
using System;

internal class NpcStateFactory
{
  public static INpcState Create(NpcStateEnum npcStateEnum, NpcState state, Pivot pivot)
  {
    switch (npcStateEnum)
    {
      case NpcStateEnum.Unknown:
        return (INpcState) new NpcStateUnknown(state, pivot);
      case NpcStateEnum.Idle:
        return (INpcState) new NpcStateIdle(state, pivot);
      case NpcStateEnum.IdleKinematic:
        return (INpcState) new NpcStateIdleKinematic(state, pivot);
      case NpcStateEnum.IdlePlagueCloud:
        return (INpcState) new NpcStateIdlePlagueCloud(state, pivot);
      case NpcStateEnum.Rotate:
        return (INpcState) new NpcStateRotate(state, pivot);
      case NpcStateEnum.Move:
        return (INpcState) new NpcStateMove(state, pivot);
      case NpcStateEnum.MoveCloud:
        return (INpcState) new NpcStateMoveCloud(state, pivot);
      case NpcStateEnum.MoveRetreat:
        return (INpcState) new NpcStateMoveRetreat(state, pivot);
      case NpcStateEnum.MoveFollow:
        return (INpcState) new NpcStateMoveFollow(state, pivot);
      case NpcStateEnum.MoveByPath:
        return (INpcState) new NpcStateMoveByPath(state, pivot);
      case NpcStateEnum.MoveByPathCloud:
        return (INpcState) new NpcStateMoveByPathCloud(state, pivot);
      case NpcStateEnum.MoveFollowTeleport:
        return (INpcState) new NpcStateMoveFollowTeleport(state, pivot);
      case NpcStateEnum.MoveFollowTeleportStationary:
        return (INpcState) new NpcStateMoveFollowTeleportStationary(state, pivot);
      case NpcStateEnum.PointOfInterest:
        return (INpcState) new NpcStatePointOfInterest(state, pivot);
      case NpcStateEnum.PresetIdle:
        return (INpcState) new NpcStateIdlePreset(state, pivot);
      case NpcStateEnum.PresetIdleTest:
        return (INpcState) new NpcStateIdlePresetTest(state, pivot);
      case NpcStateEnum.POIExtraExit:
        return (INpcState) new NPCStatePOIExtraExit(state, pivot);
      case NpcStateEnum.FightIdle:
        return (INpcState) new NpcStateFightIdle(state, pivot);
      case NpcStateEnum.FightFollow:
        return (INpcState) new NpcStateFightFollow(state, pivot);
      case NpcStateEnum.FightKeepDistance:
        return (INpcState) new NpcStateFightKeepDistance(state, pivot);
      case NpcStateEnum.FightSurrender:
        return (INpcState) new NpcStateFightSurrender(state, pivot);
      case NpcStateEnum.FightSurrenderLoot:
        return (INpcState) new NpcStateFightSurrenderLoot(state, pivot);
      case NpcStateEnum.FightEscape:
        return (INpcState) new NpcStateFightEscape(state, pivot);
      case NpcStateEnum.Ragdoll:
        return (INpcState) new NpcStateRagdoll(state, pivot);
      case NpcStateEnum.RagdollRessurect:
        return (INpcState) new NpcStateRagdollRessurect(state, pivot);
      case NpcStateEnum.InFire:
        return (INpcState) new NpcStateInFire(state, pivot);
      case NpcStateEnum.DialogNpc:
        return (INpcState) new NpcStateDialogNpc(state, pivot);
      case NpcStateEnum.FightFollowTarget:
        return (INpcState) new NpcStateFightFollowTarget(state, pivot);
      default:
        throw new NotImplementedException();
    }
  }
}
