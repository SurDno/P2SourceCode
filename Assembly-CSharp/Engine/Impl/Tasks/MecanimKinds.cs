namespace Engine.Impl.Tasks;

public static class MecanimKinds {
	public const string MecanimSpeed = "Mecanim.Speed";
	public const string MecanimRandom = "Mecanim.Random";
	public const string InfoStateControl = "Info.State.Control";
	public const string InfoStateCurrent = "Info.State.Current";
	public const string AttackerAngleStart = "Attacker.Angle.Start";
	public const string AttackerAngleElapsed = "Attacker.Angle.Elapsed";
	public const string AttackerAttack = "Attacker.Attack";
	public const string AttackerStateControl = "Attacker.State.Control";
	public const string AttackerStateCurrent = "Attacker.State.Current";
	public const string AttackerStateAnimationEnd = "Attacker.State.AnimationEnd";
	public const string AttackerMoveControl = "Attacker.Move.Control";
	public const string AttackerMoveCurrent = "Attacker.Move.Current";
	public const string AttackerIdleControl = "Attacker.Idle.Control";
	public const string AttackerIdleCurrent = "Attacker.Idle.Current";
	public const string AttackerStepsVelocityX = "Attacker.Steps.Velocity.X";
	public const string AttackerStepsVelocityZ = "Attacker.Steps.Velocity.Z";
	public const string AttackerStepsControl = "Attacker.Steps.Control";
	public const string AttackerStepsCurrent = "Attacker.Steps.Current";
	public const string AttackerRecedeControl = "Attacker.Recede.Control";
	public const string AttackerRecedeCurrent = "Attacker.Recede.Current";
	public const string AttackerRecedeVelocity = "Attacker.Recede.Velocity";
	public const string AttackerVelocityX = "Attacker.Velocity.X";
	public const string AttackerVelocityZ = "Attacker.Velocity.Z";
	public const string AttackerIsRightFootLeading = "Attacker.IsRightFootLeading";
	public const string AttackerPlayerStateControl = "AttackerPlayer.State.Control";
	public const string AttackerPlayerStateCurrent = "AttackerPlayer.State.Current";
	public const string AttackerPlayerWeaponStateControl = "AttackerPlayer.Weapon.State.Control";
	public const string AttackerPlayerWeaponStateCurrent = "AttackerPlayer.Weapon.State.Current";
	public const string AttackerPlayerWeaponRevolverStateControl = "AttackerPlayer.Weapon.Revolver.State.Control";
	public const string AttackerPlayerWeaponRevolverStateCurrent = "AttackerPlayer.Weapon.Revolver.State.Current";
	public const string AttackerPlayerWeaponRevolverFire = "AttackerPlayer.Weapon.Revolver.Fire";
	public const string AttackerPlayerWeaponHolsterTrigger = "AttackerPlayer.Weapon.Holster.Trigger";
	public const string AttackerSanitarStateControl = "AttackerSanitar.State.Control";
	public const string AttackerSanitarStateCurrent = "AttackerSanitar.State.Current";
	public const string AttackerSoldierStateControl = "AttackerSoldier.State.Control";
	public const string AttackerSoldierStateCurrent = "AttackerSoldier.State.Current";
	public const string AttackerSoldierAimAngle = "AttackerSoldier.Aim.Angle";
	public const string AttackerSoldierFireTrigger = "AttackerSoldier.Fire.Trigger";
	public const string AttackerSoldierRotateLeft = "AttackerSoldier.Rotate.Left";
	public const string AttackerSoldierRotateRight = "AttackerSoldier.Rotate.Right";
	public const string AttackerDiseasedStateControl = "AttackerDiseased.State.Control";
	public const string AttackerDiseasedStateCurrent = "AttackerDiseased.State.Current";
	public const string AttackerDiseasedPlayerPushKind = "AttackerDiseased.Player.Push.Kind";
	public const string Movable = "Movable";
	public const string MovableAngleStart = "Movable.Angle.Start";
	public const string MovableAngleElapsed = "Movable.Angle.Elapsed";
	public const string MovableStateControl = "Movable.State.Control";
	public const string MovableStateCurrent = "Movable.State.Current";
	public const string MovableMoveControl = "Movable.Move.Control";
	public const string MovableMoveCurrent = "Movable.Move.Current";
	public const string MovableIdleControl = "Movable.Idle.Control";
	public const string MovableIdleCurrent = "Movable.Idle.Current";
	public const string MovableIdleAnimationsCount = "Movable.Idle.AnimationsCount";
	public const string MovableIdleAnimationsLowCount = "Movable.Idle.AnimationsLowCount";
	public const string MovableIdleAnimationsControl = "Movable.Idle.AnimationControl";
	public const string MovableVelocityZ = "Movable.Velocity.Z";
	public const string Speaking = "Speaking";
	public const string MovablePOIControl = "Movable.POI.Control";
	public const string MovablePOICurrent = "Movable.POI.Current";
	public const string MovablePOIRandom = "Movable.POI.Random";
	public const string MovablePOIAnimationIndex = "Movable.POI.AnimationIndex";
	public const string MovablePOIAnimationIndex2 = "Movable.POI.AnimationIndex2";
	public const string MovablePOIStartFromMiddle = "Movable.POI.StartFromMiddle";
	public const string MovablePOIMiddleAnimationsCount = "Movable.POI.MiddleAnimationsCount";
	public const string DialogAnimationsCount = "Dialog.AnimationsCount";
	public const string DialogAnimationControl = "Dialog.AnimationControl";
	public const string FightScale = "Fight.Scale";
	public const string FightBlockStance = "Fight.BlockStance";
	public const string FightRandom = "Fight.Random";
	public const string FightWalkSpeed = "Fight.WalkSpeed";
	public const string FightFaint = "Fight.Faint";
	public const string FightAttackType = "Fight.AttackType";
	public const string FightContrReaction = "Fight.ContrReaction";
	public const string FightContrReaction1 = "Fight.ContrReaction1";
	public const string FightIsPlayerBlock = "Fight.IsPlayerBlock";
	public const string FightTriggersWeaponOn = "Fight.Triggers/WeaponOn";
	public const string FightTriggersWeaponPrepare = "Fight.Triggers/WeaponPrepare";
	public const string FightTriggersCancelWeaponPrepare = "Fight.Triggers/CancelWeaponPrepare";
	public const string FightTriggersReset = "Fight.Triggers/Reset";
	public const string FightIsFaint = "Fight.IsFaint";
	public const string FightReactionX = "Fight.ReactionX";
	public const string FightReactionY = "Fight.ReactionY";
	public const string FightAttackAfterCheat = "Fight.AttackAfterCheat";
	public const string FightPushAngle = "Fight.PushAngle";
	public const string FightThrowRange = "Fight.ThrowRange";
	public const string FightRifleHold = "Fight.RifleHold";
	public const string LipsyncLayer0 = "Lipsync 0";
	public const string LipsyncLayer1 = "Lipsync 1";
	public const string LipsyncLayer2 = "Lipsync 2";
	public const string LipsyncPhonemeIndex0 = "LipSync/PhonemeIndex0";
	public const string LipsyncPhonemeIndex1 = "LipSync/PhonemeIndex1";
	public const string LipsyncPhonemeIndex2 = "LipSync/PhonemeIndex2";
	public const string TriggersRagdoll = "Triggers/Ragdoll";
	public const string TriggersRagdollGetUpFromBelly = "Triggers/RagdollGetUpFromBelly";
	public const string TriggersRagdollGetUpFromBack = "Triggers/RagdollGetUpFromBack";
	public const string FightTriggersReaction = "Fight.Triggers/Reaction";
	public const string FightTriggersCancelAttack = "Fight.Triggers/CancelAttack";
	public const string FightTriggersPush = "Fight.Triggers/Push";
	public const string FightTriggersAttack = "Fight.Triggers/Attack";
	public const string FightTriggersFire = "Fight.Triggers/Fire";
	public const string FightTriggersRessurect = "Fight.Triggers/Ressurect";
	public const string FightTriggersRessurect2 = "Fight.Triggers/Ressurect2";
	public const string FightTriggersCancelReaction = "Fight.Triggers/CancelReaction";
	public const string FightTriggersFaint = "Fight.Triggers/Faint";
	public const string FightTriggersFaint2 = "Fight.Triggers/Faint";
	public const string FightTriggersDie = "Fight.Triggers/Die";
	public const string FightTriggersDie2 = "Fight.Triggers/Die2";
	public const string FightTriggersQuickBlock = "Fight.Triggers/QuickBlock";
	public const string FightTriggersDodgeBack = "Fight.Triggers/DodgeBack";
	public const string FightTriggersDodgeLeft = "Fight.Triggers/DodgeLeft";
	public const string FightTriggersDodgeRight = "Fight.Triggers/DodgeRight";
	public const string FightTriggersEscape = "Fight.Triggers/Escape";
	public const string FightTriggersEscapeImmediate = "Fight.Triggers/EscapeImmediate";
	public const string FightTriggersCancelWalk = "Fight.Triggers/CancelWalk";
	public const string FightTriggersCancelEscape = "Fight.Triggers/CancelEscape";
	public const string FightTriggersSurrender = "Fight.Triggers/Surrender";
	public const string FightTriggersTakeMyMoney = "Fight.Triggers/TakeMyMoney";
	public const string FightTriggersTakeMyMoneyImmediate = "Fight.Triggers/TakeMyMoneyImmediate";
	public const string FightTriggersDieInFire = "Fight.Triggers/DieInFire";
	public const string FightTriggersLoot = "Fight.Triggers/Loot";
	public const string FightTriggersCounterattack = "Fight.Triggers/Counterattack";
	public const string FightTriggersThrowBomb = "Fight.Triggers/ThrowBomb";
	public const string FightTriggersStagger = "Fight.Triggers/Stagger";
	public const string FightTriggersCancelStagger = "Fight.Triggers/CancelStagger";
	public const string FightTriggersRunAttack = "Fight.Triggers/RunPunch";
	public const string FightTriggersStepLeft = "Fight.Triggers/StepLeft";
	public const string FightTriggersStepRight = "Fight.Triggers/StepRight";
	public const string FightTriggersAimSamopal = "Fight.Triggers/AimSamopal";
	public const string FightTriggersFireSamopal = "Fight.Triggers/FireSamopal";
	public const string FightTriggersDropSamopal = "Fight.Triggers/DropSamopal";
	public const string FightTriggersAimRifle = "Fight.Triggers/AimRifle";
	public const string FightTriggersFireRifle = "Fight.Triggers/FireRifle";
	public const string FightTriggersPunchRifle = "Fight.Triggers/PunchRifle";
	public const string TriggersPOIExtraExit = "Triggers/POIExtraExit";
	public const string TriggersPOIExtraExitNoAnimation = "Triggers/POIExtraExitNoAnimation";

	public enum StateKind {
		Unknown = 0,
		Attacker = 1,
		Audio = 2,
		Controller = 3,
		Detectable = 4,
		Detector = 5,
		GameObject = 6,
		Gate = 7,
		Info = 8,
		Market = 10,
		Milestone = 11,
		Model = 12,
		Movable = 13,
		Region = 14,
		Speaking = 15,
		Storable = 16,
		Storage = 17,
		TaskManager = 18,
		Trade = 19,
		AttackerPlayer = 20,
		Klubok = 21,
		AttackerSanitar = 22,
		AttackerSoldier = 23,
		AttackerDiseased = 24
	}

	public enum InfoStateKind {
		Unknown,
		Cutscene
	}

	public enum AttackerStateKind {
		Unknown,
		Idle,
		Rotate,
		Move,
		Steps,
		Recede,
		Run,
		Attack,
		Reaction,
		Surrender,
		Retreat,
		Fatality,
		FatalityReaction,
		FinishRecover,
		Steps2,
		BodySearch,
		AttackLeadsToFinishPose,
		ReactionLeadsToFinishPose,
		FinishDieYourself,
		AttackPlayer,
		ReactionToPlayer
	}

	public enum AttackerPlayerStateKind {
		Unknown,
		Holstered,
		Unholstered,
		AttackFalse,
		AttackNPC,
		ReactionToNPC,
		PushDiseased
	}

	public enum AttackerPlayerWeaponStateKind {
		Unknown,
		Hands,
		Knife,
		Revolver,
		Rifle
	}

	public enum AttackerPlayerWeaponRevolverStateKind {
		Unknown,
		Idle,
		Steady,
		Aim,
		Unholster
	}

	public enum AttackerSanitarStateKind {
		Unknown,
		Attack
	}

	public enum AttackerSoldierStateKind {
		Unknown,
		Idle,
		Fire
	}

	public enum AttackerDiseasedStateKind {
		Unknown,
		PushedByPlayer
	}

	public enum AttackerMoveStateKind {
		Unknown,
		Start,
		Gait,
		Stop
	}

	public enum AttackerIdleStateKind {
		Unknown,
		Primary,
		Secondary
	}

	public enum AttackerStepsStateKind8 {
		Unknown
	}

	public enum AttackerRecedeStateKind {
		Unknown
	}

	public enum MovableMoveStateKind {
		Unknown,
		Start,
		Gait,
		Stop
	}

	public enum MovableIdleStateKind {
		Unknown,
		Primary,
		Secondary
	}

	public enum MovablePOIStateKind {
		Unknown,
		S_SitAtDesk,
		S_SitOnBench,
		S_LeanOnWall,
		S_LeanOnTable,
		S_SitNearWall,
		S_LieOnBed,
		S_NearFire,
		Q_ViewPoster,
		Q_LookOutOfTheWindow,
		Q_LookUnder,
		Q_LookIntoTheWindow,
		Q_ActionWithWall,
		Q_ActionWithTable,
		Q_ActionWithWardrobe,
		Q_ActionWithShelves,
		Q_ActionWithNightstand,
		Q_ActionOnFloor,
		S_ActionOnFloor,
		Q_Idle,
		Q_NearFire,
		S_Dialog,
		S_Loot,
		Q_PlaygroundPlay,
		S_PlaygroundSandbox,
		S_PlaygroundCooperative,
		Q_PlaygroundCooperative,
		S_SitAtDeskRight
	}

	public enum MovableStateKind {
		Unknown,
		Idle,
		Rotate,
		Move,
		Attack,
		SoldierStop,
		POI
	}

	public enum SpeakaingStateKind {
		Unknown,
		Cutscene
	}
}