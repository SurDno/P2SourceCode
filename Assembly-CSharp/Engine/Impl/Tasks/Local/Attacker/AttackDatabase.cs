using System;
using Engine.Common.Components.Attacker;
using Engine.Common.Components.AttackerDiseased;
using Engine.Source.Commons;

namespace Engine.Impl.Tasks.Local.Attacker
{
  public static class AttackDatabase
  {
    public static int GetRandomAttackSubkind(AttackKind attackKind)
    {
      return attackKind == AttackKind.FrontPunch ? UnityEngine.Random.Range(0, 2) + 1 : 1;
    }

    public static int GetRandomPlayerAttackSubkind(PlayerAttackKind attackKind)
    {
      return attackKind == PlayerAttackKind.FrontPush ? UnityEngine.Random.Range(0, 2) + 1 : 1;
    }

    public static int GetRandomNPCAttackSubkind(NPCAttackKind attackKind)
    {
      return attackKind == NPCAttackKind.FrontPunch ? UnityEngine.Random.Range(0, 2) + 1 : 1;
    }

    public static float GetAttackImportantPhaseTime(PlayerAttackKind attackKind)
    {
      return attackKind == PlayerAttackKind.FrontPush ? 2f : 1.05f;
    }

    public static float GetAttackImportantPhaseTime(NPCAttackKind attackKind) => 1.05f;

    public static float GetReactionImportantPhaseTime(NPCAttackKind attackKind) => 1.05f;

    public static void GetAttackTarget(
      AttackKind attackKind,
      int attackSubkind,
      GameObject target,
      out Vector3 position,
      out Quaternion rotation)
    {
      if (attackKind == AttackKind.FrontPush)
      {
        position = target.transform.position + target.transform.rotation * new Vector3(-0.143f, 0.0f, 0.828f);
        rotation = target.transform.rotation * Quaternion.AngleAxis(180f, Vector3.up);
      }
      else if (attackKind == AttackKind.FrontPunch)
      {
        position = target.transform.position + target.transform.rotation * new Vector3(-0.04f, 0.0f, 1.318f);
        rotation = target.transform.rotation * Quaternion.AngleAxis(180f, Vector3.up);
      }
      else
      {
        if (attackKind != AttackKind.FrontDodgeCounterPunch)
          throw new NotImplementedException();
        position = target.transform.position + target.transform.rotation * new Vector3(-0.084f, 0.0f, 0.985f);
        rotation = target.transform.rotation * Quaternion.AngleAxis(180f, Vector3.up);
      }
    }

    public static void GetPlayerAttackTarget(
      PlayerAttackKind playerAttackKind,
      int attackSubkind,
      IEntityView targetView,
      out Vector3 position,
      out Quaternion rotation)
    {
      switch (playerAttackKind)
      {
        case PlayerAttackKind.FrontPunch:
          position = targetView.Position + targetView.Rotation * new Vector3(0.03619f, 0.0f, 1.18181f);
          rotation = targetView.Rotation * Quaternion.AngleAxis(180f, Vector3.up);
          break;
        case PlayerAttackKind.FrontDodgeCounterPunch:
          position = targetView.Position + targetView.Rotation * new Vector3(-0.084f, 0.0f, 0.985f);
          rotation = targetView.Rotation * Quaternion.AngleAxis(180f, Vector3.up);
          break;
        case PlayerAttackKind.FrontPush:
          position = targetView.Position + targetView.Rotation * new Vector3(0.075f, 0.0f, 0.987f);
          rotation = targetView.Rotation * Quaternion.AngleAxis(180f, Vector3.up);
          break;
        case PlayerAttackKind.FrontDodge:
          position = targetView.Position + targetView.Rotation * new Vector3(0.136f, 0.0f, 1.0651f);
          rotation = targetView.Rotation * Quaternion.AngleAxis(180f, Vector3.up);
          break;
        default:
          throw new NotImplementedException();
      }
    }

    public static void GetPlayerAttackTarget(
      PlayerPushesDiseasedKind playerAttackKind,
      IEntityView targetView,
      out Vector3 position,
      out Quaternion rotation)
    {
      if (playerAttackKind == PlayerPushesDiseasedKind.FrontalPush)
      {
        position = targetView.Position + targetView.Rotation * new Vector3(0.136f, 0.0f, 1.0651f);
        rotation = targetView.Rotation * Quaternion.AngleAxis(180f, Vector3.up);
      }
      else if (playerAttackKind == PlayerPushesDiseasedKind.PushToLeft)
      {
        position = targetView.Position + targetView.Rotation * new Vector3(0.136f, 0.0f, 1.0651f);
        rotation = targetView.Rotation * Quaternion.AngleAxis(180f, Vector3.up);
      }
      else
      {
        if (playerAttackKind != PlayerPushesDiseasedKind.PushToRight)
          throw new NotImplementedException();
        position = targetView.Position + targetView.Rotation * new Vector3(0.136f, 0.0f, 1.0651f);
        rotation = targetView.Rotation * Quaternion.AngleAxis(180f, Vector3.up);
      }
    }

    public static void GetNPCAttackTarget(
      NPCAttackKind playerAttackKind,
      int attackSubkind,
      IEntityView targetView,
      out Vector3 position,
      out Quaternion rotation)
    {
      switch (playerAttackKind)
      {
        case NPCAttackKind.FrontPunch:
          position = targetView.Position + targetView.Rotation * new Vector3(0.05f, 0.0f, 1.236f);
          rotation = targetView.Rotation * Quaternion.AngleAxis(180f, Vector3.up);
          break;
        case NPCAttackKind.FrontPush:
          position = targetView.Position + targetView.Rotation * new Vector3(0.05f, 0.0f, 1.236f);
          rotation = targetView.Rotation * Quaternion.AngleAxis(180f, Vector3.up);
          break;
        case NPCAttackKind.FrontPunchBlocked:
          position = targetView.Position + targetView.Rotation * new Vector3(0.136f, 0.0f, 1.0651f);
          rotation = targetView.Rotation * Quaternion.AngleAxis(180f, Vector3.up);
          break;
        case NPCAttackKind.FrontPunchBlockPassed:
          position = targetView.Position + targetView.Rotation * new Vector3(0.136f, 0.0f, 1.0651f);
          rotation = targetView.Rotation * Quaternion.AngleAxis(180f, Vector3.up);
          break;
        default:
          throw new NotImplementedException();
      }
    }

    public static void GetFinishTarget(
      FinishKind attackKind,
      IEntityView targetView,
      out Vector3 position,
      out Quaternion rotation)
    {
      if (attackKind != FinishKind.FrontPunchFinish)
        throw new NotImplementedException();
      position = targetView.Position + targetView.Rotation * new Vector3(0.035f, 0.0f, 1.227f);
      rotation = targetView.Rotation * Quaternion.AngleAxis(180f, Vector3.up);
    }

    public static void GetFatalityTarget(
      FinishKind attackKind,
      IEntityView targetView,
      out Vector3 position,
      out Quaternion rotation)
    {
      if (attackKind != FinishKind.FrontPunchFinish)
        throw new NotImplementedException();
      position = targetView.Position + targetView.Rotation * new Vector3(2.613f, 0.0f, -0.742f);
      rotation = targetView.Rotation * Quaternion.AngleAxis(-90f, Vector3.up);
    }

    public static float GetSyncTime(AttackKind attackKind, bool fatalityStage)
    {
      if (attackKind == AttackKind.FrontPush || attackKind == AttackKind.FrontPunch || attackKind == AttackKind.FrontDodgeCounterPunch)
        return 0.3f;
      throw new NotImplementedException();
    }

    public static float GetSyncTime(PlayerAttackKind attackKind) => 0.3f;

    public static float GetSyncTime(NPCAttackKind attackKind) => 0.3f;

    public static float GetSyncTime(PlayerPushesDiseasedKind attackKind) => 0.3f;

    public static float GetSyncTime(FinishKind attackKind, bool fatalityStage)
    {
      if (attackKind != FinishKind.FrontPunchFinish)
        throw new NotImplementedException();
      return fatalityStage ? 0.75f : 0.5f;
    }

    public static bool CanReactionMove(AttackKind attackKind) => true;

    public static bool CanReactionMove(NPCAttackKind attackKind) => true;

    public static bool CanReactionToPlayerMove(PlayerAttackKind attackKind) => true;

    public static bool CanReactionMove(FinishKind attackKind, bool fatalityStage)
    {
      return !(attackKind == FinishKind.FrontPunchFinish & fatalityStage);
    }

    public static bool CanReactionToPlayerMove(PlayerPushesDiseasedKind attackKind) => true;
  }
}
