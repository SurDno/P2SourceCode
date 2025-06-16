using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using Engine.Behaviours.Components;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components.Parameters;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using UnityEngine;

namespace Engine.Source.Services;

[RuntimeService(typeof(CombatService), typeof(ICombatService))]
public class CombatService : IInitialisable, IUpdatable, ICombatService {
	private List<CombatServiceCharacterInfo> characters = new();
	private List<CombatServiceCharacterInfo> activeCharacters = new();
	private List<CombatServiceCombatInfo> currentCombats = new();
	private List<CombatServiceCombatInfo> combatsToUpdate = new();
	private List<CombatCry> currentCries = new();
	private List<CombatServiceCommonOrder> waitingAttackOrders = new();
	private CombatServiceCharacterInfo playerCharacter;
	private Dictionary<IEntity, CombatServiceCharacterInfo> charactersDictionary = new();
	private float enemiesSearchRadius = 30f;
	private float minimumSurrenderTime = 2f;
	private float escapedIgnoreTimeout = 12f;
	private static List<CombatCry> tmp = new();
	private static List<CombatServiceCommonOrder> tmp2 = new();
	private bool playerIsFighting;

	public bool PlayerIsFighting {
		get => playerIsFighting;
		private set {
			playerIsFighting = value;
			if (playerCharacter == null || !(playerCharacter.Character != null))
				return;
			playerCharacter.Character.IsFighting = value;
		}
	}

	public void Initialise() {
		InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable(this);
		PlayerIsFighting = false;
	}

	public void Terminate() {
		characters.Clear();
		activeCharacters.Clear();
		currentCombats.Clear();
		combatsToUpdate.Clear();
		currentCries.Clear();
		waitingAttackOrders.Clear();
		charactersDictionary.Clear();
		InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable(this);
	}

	public void RegisterCharacter(EnemyBase character) {
		if (character == null || !character.isActiveAndEnabled)
			return;
		if (character.Owner != null) {
			var byName = character.Owner.GetComponent<ParametersComponent>()
				.GetByName<CombatStyleEnum>(ParameterNameEnum.CombatStyle);
			if (byName != null && byName.Value == CombatStyleEnum.Default) {
				var component = character.GetComponent<Pivot>();
				if (component != null && component.DefaultCombatStyle != 0)
					byName.Value = component.DefaultCombatStyle;
			}
		}

		var serviceCharacterInfo = characters.Find(x => x.Character == character);
		if (serviceCharacterInfo == null) {
			serviceCharacterInfo = new CombatServiceCharacterInfo(character, this);
			characters.Add(serviceCharacterInfo);
			serviceCharacterInfo.State = CombatServiceCharacterStateEnum.Free;
		}

		if (!serviceCharacterInfo.IsPlayer)
			return;
		playerCharacter = serviceCharacterInfo;
	}

	public void AddCharacterToDictionary(IEntity entity, CombatServiceCharacterInfo character) {
		if (entity == null)
			return;
		charactersDictionary[entity] = character;
	}

	public void UnregisterCharacter(EnemyBase character) {
		if (character == null)
			return;
		var character1 = characters.Find(x => x.Character == character);
		foreach (var characterCombat in GetCharacterCombats(character1))
			RemoveCharacterFromCombat(characterCombat, character1);
		UpdateCharacterCombats(character1);
		if (character1 == null)
			return;
		if (character1.Entity != null)
			charactersDictionary.Remove(character1.Entity);
		characters.Remove(character1);
		character1.Clear();
	}

	public void AddPersonalEnemy(IEntity attacker, IEntity enemy) {
		var characterInfo1 = GetCharacterInfo(attacker);
		var characterInfo2 = GetCharacterInfo(enemy);
		if (characterInfo1 != null && characterInfo2 != null) {
			characterInfo1.PersonalAttackEnemies.Add(characterInfo2);
			characterInfo1.RecountEnemies();
		} else
			waitingAttackOrders.Add(new CombatServiceCommonOrder {
				attacker = attacker,
				enemy = enemy
			});
	}

	public void RemovePersonalEnemy(IEntity attacker, IEntity enemy) {
		var attackerInfo = GetCharacterInfo(attacker);
		var enemyInfo = GetCharacterInfo(enemy);
		if (attackerInfo == null || enemyInfo == null || !attackerInfo.PersonalAttackEnemies.Contains(enemyInfo))
			return;
		attackerInfo.PersonalAttackEnemies.Remove(enemyInfo);
		attackerInfo.RecountEnemies();
		var combat = currentCombats.Find(x => x.Characters.Contains(attackerInfo) && x.Characters.Contains(enemyInfo));
		if (combat != null) {
			attackerInfo.State = CombatServiceCharacterStateEnum.Free;
			var combatFractionInfo = combat.Fractions.Find(x => x.Characters.Contains(attackerInfo));
			if (combatFractionInfo != null && combatFractionInfo.AttackFractions != null &&
			    combatFractionInfo.AttackFractions.Contains(enemyInfo.Fraction))
				combatFractionInfo.AttackFractions.Remove(enemyInfo.Fraction);
			AddCombatToUpdate(combat);
		}
	}

	private void CheckPOIExit(CombatServiceCharacterInfo character) {
		if (character.IsPlayer || character.IsDead || !character.Character.GetComponent<NpcState>().NeedExtraExitPOI)
			return;
		BehaviorSubtreeUtility.SetCharacterSubtree(
			BehaviorSubtreeUtility.GetCharacterSubtree(character.Character.gameObject),
			ScriptableObjectInstance<ResourceFromCodeData>.Instance.POIExtraExit);
		character.State = CombatServiceCharacterStateEnum.ExitingPOI;
	}

	public void EnterCombat(EnemyBase attacker, EnemyBase enemy, bool watch = false) {
		RegisterCharacter(attacker);
		RegisterCharacter(enemy);
		var character = GetCharacterInfo(attacker);
		var target = GetCharacterInfo(enemy);
		if (character == null || target == null)
			return;
		if (!watch) {
			character.FireControllerCombatAction(CombatActionEnum.EnterCombat, enemy.Owner);
			target.FireControllerCombatAction(CombatActionEnum.EnterCombat, attacker.Owner);
		}

		if (!activeCharacters.Contains(character))
			activeCharacters.Add(character);
		if (character.State != CombatServiceCharacterStateEnum.ExitingPOI)
			character.State = CombatServiceCharacterStateEnum.Free;
		CheckPOIExit(character);
		CheckPOIExit(target);
		AddCharactersToCombat(character, target);
		MergeAllCombats(character, target);
		var combat = currentCombats.Find(x => x.Characters.Contains(character) && x.Characters.Contains(target));
		if (watch)
			character.Orders.Add(new CombatServiceCharacterOrder {
				OrderType = CombatServiceCharacterOrderEnum.Watch,
				OrderTarget = target
			});
		else {
			HostileAction(combat, character, target);
			HostileAction(combat, target, character);
			character.LastGotHitPosition = character.Character.transform.position;
		}

		if (combat.EscapedCharacters.Exists(x => x.EscapedCharacter == character))
			combat.EscapedCharacters.RemoveAll(x => x.EscapedCharacter == character);
		AddCombatToUpdate(combat);
	}

	public void ExitCombat(EnemyBase attacker) {
		var character = GetCharacterInfo(attacker);
		if (activeCharacters.Contains(character))
			activeCharacters.Remove(character);
		foreach (var combat in currentCombats.FindAll(x => x.Characters.Contains(character)))
			AddCombatToUpdate(combat);
		SetInactive(character);
		if (character?.Character?.gameObject == null)
			return;
		var component = character?.Character?.gameObject?.GetComponent<WeaponServiceBase>();
		if (!(component != null))
			return;
		component.Weapon = WeaponEnum.Unknown;
	}

	public void HitNpc(IEntity actor, IEntity target) {
		var character = GetCharacterInfo(actor);
		var enemy = GetCharacterInfo(target);
		if (character == null || enemy == null || character.IsCombatIgnored || enemy.IsCombatIgnored)
			return;
		character.LastGotHitPosition = character.Character.transform.position;
		var serviceCombatInfo =
			currentCombats.Find(x => x.Characters.Contains(character) || x.Characters.Contains(enemy));
		if (serviceCombatInfo == null || !serviceCombatInfo.Characters.Contains(character) ||
		    !serviceCombatInfo.Characters.Contains(enemy))
			AddCharactersToCombat(character, enemy);
		MergeAllCombats(character, enemy);
		var combat = currentCombats.Find(x => x.Characters.Contains(character) && x.Characters.Contains(enemy));
		if (combat != null) {
			if (enemy.State == CombatServiceCharacterStateEnum.WatchFighting)
				enemy.State = CombatServiceCharacterStateEnum.Free;
			if (enemy.State == CombatServiceCharacterStateEnum.Escape || character.IsPlayer)
				enemy.CurrentEnemy = character;
			HostileAction(combat, character, enemy);
		}

		if (combat.EscapedCharacters.Exists(x => x.EscapedCharacter == character)) {
			combat.EscapedCharacters.RemoveAll(x => x.EscapedCharacter == character);
			AddCombatToUpdate(combat);
		}

		if (!enemy.IsIndoors && enemy.State == CombatServiceCharacterStateEnum.Surrender)
			CheckNpcEscapeFromSurrender(enemy, combat);
		CharacterCryHelp(enemy, character);
	}

	private void CheckNpcEscapeFromSurrender(
		CombatServiceCharacterInfo character,
		CombatServiceCombatInfo combat) {
		if (character.GetTimeFromLastOrder() < (double)minimumSurrenderTime)
			return;
		character.Orders.Add(new CombatServiceCharacterOrder {
			OrderType = CombatServiceCharacterOrderEnum.Escape,
			OrderTarget = character
		});
		AddCombatToUpdate(combat);
	}

	public void CharacterCryHelp(
		CombatServiceCharacterInfo character,
		CombatServiceCharacterInfo enemy) {
		var hearingCharacters = FindHearingCharacters(character);
		if (hearingCharacters.Count == 0)
			return;
		var combat = currentCombats.Find(x => x.Characters.Contains(character));
		hearingCharacters.RemoveAll(x => combat.Characters.Contains(x));
		if (hearingCharacters.Count == 0)
			return;
		var fraction = combat.Fractions.Find(x => x.Characters.Contains(character));
		hearingCharacters.RemoveAll(x => !fraction.AskForHelpFractions.Contains(x.Fraction));
		if (hearingCharacters.Count == 0)
			return;
		CharacterCry(character, CombatCryEnum.NeedHelp, enemy, 0.0f);
	}

	public void CharacterCry(EnemyBase character, CombatCryEnum cryType) {
		CharacterCry(GetCharacterInfo(character), cryType);
	}

	private void HearCry(CombatServiceCharacterInfo character, CombatCry cry) {
		var combat = currentCombats.Find(x => x.Characters.Contains(character) || x.Characters.Contains(cry.Character));
		if (combat == null || !combat.Characters.Contains(character) || !combat.Characters.Contains(cry.Character)) {
			AddCharactersToCombat(character, cry.Character);
			MergeAllCombats(character, cry.Character);
			combat = currentCombats.Find(x => x.Characters.Contains(character) && x.Characters.Contains(cry.Character));
		}

		character.HearedCries.Add(cry);
		if (cry.CryType != CombatCryEnum.NeedHelp || combat == null)
			return;
		HostileAction(combat, cry.CryTarget, cry.Character);
		AddCombatToUpdate(combat);
	}

	public void LostSight(EnemyBase actor, EnemyBase target) {
		var character = GetCharacterInfo(actor);
		var enemy = GetCharacterInfo(target);
		var serviceCombatInfo = currentCombats.Find(x => x.Characters.Contains(character));
		if (character == null)
			return;
		character.CurrentEnemy = null;
		character.State = CombatServiceCharacterStateEnum.Free;
		if (enemy != null && !enemy.IsDead && serviceCombatInfo != null &&
		    serviceCombatInfo.EscapedCharacters != null &&
		    !serviceCombatInfo.EscapedCharacters.Exists(x =>
			    x.EscapedCharacter == enemy && x.FollowingCharacter == character))
			serviceCombatInfo.EscapedCharacters.Add(new CombatServiceEscapedCharacterInfo {
				EscapedCharacter = enemy,
				FollowingCharacter = character,
				Time = ServiceLocator.GetService<TimeService>().RealTime.TotalSeconds
			});
		character.Orders.Add(new CombatServiceCharacterOrder {
			OrderType = CombatServiceCharacterOrderEnum.GoToPoint,
			OrderPoint = character.FightStartPosition
		});
		UpdateCharacterCombats(character);
	}

	public void IndividualEscape(EnemyBase actor, EnemyBase target) {
		var characterInfo1 = GetCharacterInfo(actor);
		var characterInfo2 = GetCharacterInfo(target);
		if (characterInfo1 == null || (characterInfo1.State == CombatServiceCharacterStateEnum.Surrender &&
		                               !characterInfo2.IsPlayer))
			return;
		characterInfo1.Orders.Add(new CombatServiceCharacterOrder {
			OrderType = characterInfo1.IsIndoors
				? CombatServiceCharacterOrderEnum.Surrender
				: CombatServiceCharacterOrderEnum.Escape,
			OrderTarget = characterInfo2
		});
		UpdateCharacterCombats(characterInfo1);
	}

	public void IndividualSurrender(EnemyBase actor, EnemyBase target) {
		var characterInfo1 = GetCharacterInfo(actor);
		var characterInfo2 = GetCharacterInfo(target);
		if (characterInfo1 == null)
			return;
		characterInfo1.WasBeaten = true;
		characterInfo1.Orders.Add(new CombatServiceCharacterOrder {
			OrderType = CombatServiceCharacterOrderEnum.Surrender,
			OrderTarget = characterInfo2
		});
		UpdateCharacterCombats(characterInfo1);
	}

	public void IndividualFinishOrder(EnemyBase actor) {
		var character = GetCharacterInfo(actor);
		if (character == null)
			return;
		if (character.State == CombatServiceCharacterStateEnum.ExitingPOI)
			character.State = CombatServiceCharacterStateEnum.Free;
		if (character.State == CombatServiceCharacterStateEnum.Fight)
			character.State = CombatServiceCharacterStateEnum.Free;
		if (character.State == CombatServiceCharacterStateEnum.Surrender && !character.CurrentEnemy.IsDead &&
		    !character.IsIndoors && character.CurrentEnemy.IsPlayer)
			character.Orders.Add(new CombatServiceCharacterOrder {
				OrderType = CombatServiceCharacterOrderEnum.Escape,
				OrderTarget = character.CurrentEnemy
			});
		if (character.State == CombatServiceCharacterStateEnum.Loot) {
			if (character.CurrentEnemy != null)
				character.CurrentEnemy.WasLooted = true;
			character.State = CombatServiceCharacterStateEnum.Free;
		}

		if (character.State == CombatServiceCharacterStateEnum.GoToPoint)
			character.State = CombatServiceCharacterStateEnum.Free;
		if (character.State == CombatServiceCharacterStateEnum.Escape) {
			character.State = CombatServiceCharacterStateEnum.Free;
			var serviceCombatInfo = currentCombats.Find(x => x.Characters.Contains(character));
			if (serviceCombatInfo != null && !serviceCombatInfo.EscapedCharacters.Exists(x =>
				    x.EscapedCharacter == character && x.FollowingCharacter == character.CurrentEnemy))
				serviceCombatInfo.EscapedCharacters.Add(new CombatServiceEscapedCharacterInfo {
					EscapedCharacter = character,
					FollowingCharacter = character.CurrentEnemy,
					Time = ServiceLocator.GetService<TimeService>().RealTime.TotalSeconds
				});
		}

		if (character.State == CombatServiceCharacterStateEnum.WatchFighting)
			character.State = CombatServiceCharacterStateEnum.Free;
		UpdateCharacterCombats(character);
	}

	public void ChangedLocation(CombatServiceCharacterInfo character) {
		var combat = currentCombats.Find(x => x.Characters.Contains(character));
		if (combat == null)
			return;
		RemoveCharacterFromCombat(combat, character);
	}

	private void HostileAction(
		CombatServiceCombatInfo combat,
		CombatServiceCharacterInfo agressor,
		CombatServiceCharacterInfo reporter) {
		var flag = false;
		foreach (var fraction in combat.Fractions)
			if (fraction.ChangeOnHostileAction(agressor, reporter))
				flag = true;
		if (!flag)
			return;
		AddCombatToUpdate(combat);
	}

	public void Died(CombatServiceCharacterInfo actor) {
		if (!actor.IsPlayer)
			actor.FireControllerCombatAction(CombatActionEnum.Death, actor.NpcController.LastAttacker);
		CharacterCry(actor, CombatCryEnum.Death);
		actor.CurrentEnemy = null;
		actor.HearedCries.Clear();
		actor.State = CombatServiceCharacterStateEnum.Dead;
		UpdateCharacterCombats(actor);
	}

	private void MergeAllCombats(
		CombatServiceCharacterInfo actor,
		CombatServiceCharacterInfo target) {
		var all = currentCombats.FindAll(x => x.Characters.Contains(actor));
		foreach (var serviceCombatInfo in currentCombats.FindAll(x => x.Characters.Contains(target)))
			if (!all.Contains(serviceCombatInfo))
				all.Add(serviceCombatInfo);
		if (all.Count <= 1)
			return;
		var firstCombat = all[0];
		for (var index = 1; index < all.Count; ++index) {
			var secondCombat = all[index];
			MergeCombats(firstCombat, secondCombat);
			if (currentCombats.Contains(secondCombat))
				currentCombats.Remove(secondCombat);
			if (combatsToUpdate.Contains(secondCombat))
				combatsToUpdate.Remove(secondCombat);
		}
	}

	private void MergeCombats(
		CombatServiceCombatInfo firstCombat,
		CombatServiceCombatInfo secondCombat) {
		foreach (var character in secondCombat.Characters)
			if (!firstCombat.Characters.Contains(character))
				firstCombat.Characters.Add(character);
		foreach (var escapedCharacter in secondCombat.EscapedCharacters)
			firstCombat.EscapedCharacters.Add(escapedCharacter);
		foreach (var fraction in secondCombat.Fractions) {
			var secondCombatFraction = fraction;
			var combatFractionInfo = firstCombat.Fractions.Find(x => x.Fraction == secondCombatFraction.Fraction);
			if (combatFractionInfo != null) {
				foreach (var character in secondCombatFraction.Characters)
					if (!combatFractionInfo.Characters.Contains(character))
						combatFractionInfo.Characters.Add(character);
				foreach (var attackFraction in secondCombatFraction.AttackFractions)
					if (!combatFractionInfo.AttackFractions.Contains(attackFraction))
						combatFractionInfo.AttackFractions.Add(attackFraction);
				foreach (var fearFraction in secondCombatFraction.FearFractions)
					if (!combatFractionInfo.FearFractions.Contains(fearFraction))
						combatFractionInfo.FearFractions.Add(fearFraction);
			} else
				firstCombat.Fractions.Add(secondCombatFraction);
		}
	}

	private void UpdateCharacterCombats(CombatServiceCharacterInfo character) {
		foreach (var combat in currentCombats.FindAll(x => x.Characters.Contains(character)))
			AddCombatToUpdate(combat);
	}

	private void AddCharactersToCombat(
		CombatServiceCharacterInfo attacker,
		CombatServiceCharacterInfo enemy) {
		var combat = currentCombats.Find(x => x.Characters.Contains(attacker) || x.Characters.Contains(enemy)) ??
		             CreateCombat();
		if (!combat.Characters.Contains(attacker)) {
			combat.Characters.Add(attacker);
			InsertCharacterInCombatFractions(combat, attacker);
		}

		if (!combat.Characters.Contains(enemy)) {
			combat.Characters.Add(enemy);
			InsertCharacterInCombatFractions(combat, enemy);
		}

		AddCombatToUpdate(combat);
	}

	private CombatServiceCombatInfo CreateCombat() {
		var combat = new CombatServiceCombatInfo();
		combat.Characters = new List<CombatServiceCharacterInfo>();
		combat.Fractions = new List<CombatServiceCombatFractionInfo>();
		combat.EscapedCharacters = new List<CombatServiceEscapedCharacterInfo>();
		currentCombats.Add(combat);
		return combat;
	}

	private void InsertCharacterInCombatFractions(
		CombatServiceCombatInfo combat,
		CombatServiceCharacterInfo character) {
		if (character == null)
			return;
		var combatFractionInfo = combat.Fractions.Find(x => x.Fraction == character.Fraction);
		if (combatFractionInfo == null) {
			combatFractionInfo = new CombatServiceCombatFractionInfo(character.Fraction);
			combatFractionInfo.Characters = new List<CombatServiceCharacterInfo>();
			combat.Fractions.Add(combatFractionInfo);
		}

		combatFractionInfo.Characters.Add(character);
	}

	private void RemoveCharacterFromCombat(
		CombatServiceCombatInfo combat,
		CombatServiceCharacterInfo character) {
		combat.Characters.Remove(character);
		var combatFractionInfo = combat.Fractions.Find(x => x.Characters.Contains(character));
		if (combatFractionInfo != null) {
			combatFractionInfo.Characters.Remove(character);
			if (combatFractionInfo.Characters.Count == 0)
				combat.Fractions.Remove(combatFractionInfo);
		}

		if (GetCharacterCombats(character).Count == 0 && activeCharacters.Contains(character))
			character.ClearCombatInfo();
		AddCombatToUpdate(combat);
	}

	private void AddCombatToUpdate(CombatServiceCombatInfo combat) {
		if (combatsToUpdate.Contains(combat))
			return;
		combatsToUpdate.Add(combat);
	}

	private void UpdateCombatSituation(CombatServiceCombatInfo combat) {
		combat.EscapedCharacters.RemoveAll(x =>
			x.EscapedCharacter == null || x.EscapedCharacter.Character == null || x.FollowingCharacter == null ||
			x.FollowingCharacter.Character == null);
		foreach (var character in combat.Characters)
			if (!character.IsPlayer && !character.IsDead &&
			    character.State != CombatServiceCharacterStateEnum.ExitingPOI) {
				if (CanCancelCurrentAction(combat, character)) {
					if (character.CurrentEnemy != null && character.CurrentEnemy.IsDead)
						CharacterCry(character, CombatCryEnum.FightWin);
					GenerateCombatOrders(combat, character);
				}

				if (activeCharacters.Contains(character) && character.Orders.Count > 0) {
					TryExecuteCombatOrders(combat, character);
					character.Orders.Clear();
				}
			}

		ClearCombatFromFreeCharacters(combat);
		if (combat.Characters.Exists(x => CharacterIsCombatActive(x)))
			return;
		EndCombat(combat);
	}

	private void ClearCombatFromFreeCharacters(CombatServiceCombatInfo combat) {
		var serviceCharacterInfoList = new List<CombatServiceCharacterInfo>();
		foreach (var character in combat.Characters) {
			if (character.Character == null)
				serviceCharacterInfoList.Add(character);
			if (!character.IsPlayer && character.State == CombatServiceCharacterStateEnum.Free && character.Orders
				    .FindAll(x => x.OrderType != CombatServiceCharacterOrderEnum.GoInactive).Count == 0)
				serviceCharacterInfoList.Add(character);
		}

		foreach (var character in serviceCharacterInfoList)
			RemoveCharacterFromCombat(combat, character);
	}

	private bool TryExecuteCombatOrders(
		CombatServiceCombatInfo combat,
		CombatServiceCharacterInfo character) {
		var serviceCharacterOrder1 = character.Orders.Find(x => x.OrderType == CombatServiceCharacterOrderEnum.Escape);
		if (serviceCharacterOrder1 != null) {
			SetEscape(character, serviceCharacterOrder1.OrderTarget);
			return true;
		}

		var serviceCharacterOrder2 =
			character.Orders.Find(x => x.OrderType == CombatServiceCharacterOrderEnum.Surrender);
		if (serviceCharacterOrder2 != null) {
			SetSurrender(character, serviceCharacterOrder2.OrderTarget);
			return true;
		}

		var serviceCharacterOrder3 = character.Orders.Find(x => x.OrderType == CombatServiceCharacterOrderEnum.Attack);
		if (serviceCharacterOrder3 != null) {
			SetAttack(character, serviceCharacterOrder3.OrderTarget);
			return true;
		}

		var serviceCharacterOrder4 = character.Orders.Find(x => x.OrderType == CombatServiceCharacterOrderEnum.Loot);
		if (serviceCharacterOrder4 != null) {
			SetLoot(character, serviceCharacterOrder4.OrderTarget);
			return true;
		}

		var serviceCharacterOrder5 =
			character.Orders.Find(x => x.OrderType == CombatServiceCharacterOrderEnum.GoToPoint);
		if (serviceCharacterOrder5 != null) {
			SetGoToPoint(character, serviceCharacterOrder5.OrderPoint);
			return true;
		}

		var serviceCharacterOrder6 = character.Orders.Find(x => x.OrderType == CombatServiceCharacterOrderEnum.Watch);
		if (serviceCharacterOrder6 != null) {
			SetWatch(character, serviceCharacterOrder6.OrderTarget);
			return true;
		}

		if (character.Orders.Find(x => x.OrderType == CombatServiceCharacterOrderEnum.GoInactive) == null)
			return false;
		SetInactive(character);
		return true;
	}

	private bool CanCancelCurrentAction(
		CombatServiceCombatInfo combat,
		CombatServiceCharacterInfo character) {
		return
			(character.State == CombatServiceCharacterStateEnum.Fight && (character.CurrentEnemy.Character == null ||
			                                                              character.CurrentEnemy.IsDead ||
			                                                              character.CurrentEnemy.State ==
			                                                              CombatServiceCharacterStateEnum.Surrender ||
			                                                              character.IsIndoors !=
			                                                              character.CurrentEnemy.IsIndoors)) ||
			(character.State == CombatServiceCharacterStateEnum.Escape && (character.CurrentEnemy.Character == null ||
			                                                               character.CurrentEnemy.IsDead ||
			                                                               character.CurrentEnemy.State ==
			                                                               CombatServiceCharacterStateEnum.Escape ||
			                                                               character.CurrentEnemy.State ==
			                                                               CombatServiceCharacterStateEnum.Surrender ||
			                                                               character.CurrentEnemy.Orders.Exists(x =>
				                                                               x.OrderType ==
				                                                               CombatServiceCharacterOrderEnum.Escape ||
				                                                               x.OrderType ==
				                                                               CombatServiceCharacterOrderEnum
					                                                               .Surrender))) ||
			character.State == CombatServiceCharacterStateEnum.Free ||
			character.State == CombatServiceCharacterStateEnum.WatchFighting ||
			(character.State == CombatServiceCharacterStateEnum.Loot &&
			 (character.PersonalAttackEnemies.Exists(x => CharacterCanBeAttacked(x) && combat.Characters.Contains(x)) ||
			  character.PersonalFearEnemies.Exists(x => CharacterCanFight(x) && combat.Characters.Contains(x)))) ||
			character.State == CombatServiceCharacterStateEnum.GoToPoint ||
			!combat.Characters.Contains(character.CurrentEnemy);
	}

	private void GenerateCombatOrders(
		CombatServiceCombatInfo combat,
		CombatServiceCharacterInfo character) {
		var combatFractionInfo = combat.Fractions.Find(x => x.Characters.Contains(character));
		var fearFractions = combatFractionInfo.FearFractions;
		var attackFractions = combatFractionInfo.AttackFractions;
		var flag1 = false;
		var fearList = new List<CombatServiceCharacterInfo>();
		if (fearFractions.Count > 0 || character.PersonalFearEnemies.Count > 0 || character.WasBeaten ||
		    !character.CanFight) {
			fearList.AddRange(
				combat.Characters.FindAll(x => fearFractions.Contains(x.Fraction) && CharacterCanFight(x)));
			if (character.WasBeaten || !character.CanFight)
				fearList.AddRange(combat.Characters.FindAll(x =>
					(attackFractions.Contains(x.Fraction) || character.PersonalAttackEnemies.Contains(x)) &&
					CharacterCanFight(x)));
			var all = character.PersonalFearEnemies.FindAll(x =>
				!fearList.Contains(x) && CharacterCanFight(x) && combat.Characters.Contains(x));
			fearList.AddRange(all);
			fearList.RemoveAll(x =>
				combat.EscapedCharacters.Exists(y => y.EscapedCharacter == character && y.FollowingCharacter == x));
			fearList.RemoveAll(x => character.IsIndoors != x.IsIndoors);
			FilterAimsByRadius(character, fearList, enemiesSearchRadius);
			if (fearList.Count > 0) {
				character.Orders.Add(new CombatServiceCharacterOrder {
					OrderType = CombatServiceCharacterOrderEnum.Escape,
					OrderTarget = FindClosestAim(character, fearList)
				});
				flag1 = true;
			}
		}

		if (!flag1 && !character.WasBeaten && character.CanFight &&
		    (attackFractions.Count > 0 || character.PersonalAttackEnemies.Count > 0)) {
			var attackList =
				combat.Characters.FindAll(x => attackFractions.Contains(x.Fraction) && CharacterCanBeAttacked(x));
			var all1 = character.PersonalAttackEnemies.FindAll(x =>
				!attackList.Contains(x) && CharacterCanBeAttacked(x) && combat.Characters.Contains(x));
			attackList.AddRange(all1);
			attackList.RemoveAll(x => fearList.Contains(x));
			attackList.RemoveAll(x =>
				combat.EscapedCharacters.Exists(y => y.EscapedCharacter == x && y.FollowingCharacter == character));
			attackList.RemoveAll(x =>
				combat.EscapedCharacters.Exists(y => y.EscapedCharacter == character && y.FollowingCharacter == x));
			attackList.RemoveAll(x => character.IsIndoors != x.IsIndoors);
			FilterAimsByRadius(character, attackList, enemiesSearchRadius);
			var all2 = attackList.FindAll(x => CharacterAttackers(combat, x).Count == 0);
			var flag2 = false;
			if (all2.Count > 0) {
				attackList = all2;
				flag2 = true;
			}

			if (attackList.Count > 0) {
				var closestAim = FindClosestAim(character, attackList);
				character.Orders.Add(new CombatServiceCharacterOrder {
					OrderType = CombatServiceCharacterOrderEnum.Attack,
					OrderTarget = closestAim
				});
				if (flag2 && !closestAim.IsPlayer) {
					if (closestAim.State == CombatServiceCharacterStateEnum.Fight ||
					    closestAim.State == CombatServiceCharacterStateEnum.Escape)
						closestAim.CurrentEnemy = character;
					else
						closestAim.Orders.Add(new CombatServiceCharacterOrder {
							OrderType = CombatServiceCharacterOrderEnum.Attack,
							OrderTarget = character
						});
				}
			}
		}

		if (character.Orders.Count != 0)
			return;
		if (character.CanLoot) {
			var lootList =
				combat.Characters.FindAll(x => attackFractions.Contains(x.Fraction) && x.IsDead && !x.WasLooted);
			var all = character.PersonalAttackEnemies.FindAll(x => !lootList.Contains(x) && x.IsDead && !x.WasLooted);
			lootList.AddRange(all);
			lootList.RemoveAll(x => CharacterLooters(combat, x).Count > 0);
			if (lootList.Count > 0)
				character.Orders.Add(new CombatServiceCharacterOrder {
					OrderType = CombatServiceCharacterOrderEnum.Loot,
					OrderTarget = FindClosestAim(character, lootList)
				});
		}

		if (character.State == CombatServiceCharacterStateEnum.GoToPoint ||
		    (character.State == CombatServiceCharacterStateEnum.WatchFighting &&
		     combat.Characters.Exists(x => x.State == CombatServiceCharacterStateEnum.Fight)))
			return;
		character.Orders.Add(new CombatServiceCharacterOrder {
			OrderType = CombatServiceCharacterOrderEnum.GoInactive
		});
	}

	private bool CharacterCanFight(CombatServiceCharacterInfo character) {
		return !character.IsDead && (character.IsPlayer || (!character.IsCombatIgnored && !character.WasBeaten &&
		                                                    character.CanFight &&
		                                                    character.State != CombatServiceCharacterStateEnum
			                                                    .Surrender &&
		                                                    character.State != CombatServiceCharacterStateEnum.Escape &&
		                                                    !character.Orders.Exists(x =>
			                                                    x.OrderType == CombatServiceCharacterOrderEnum.Escape ||
			                                                    x.OrderType == CombatServiceCharacterOrderEnum
				                                                    .Surrender)));
	}

	private bool CharacterCanBeAttacked(CombatServiceCharacterInfo character) {
		return !character.IsDead && (character.IsPlayer || (!character.IsCombatIgnored && !character.IsImmortal &&
		                                                    character.State != CombatServiceCharacterStateEnum
			                                                    .Surrender));
	}

	private bool CharacterIsCombatActive(CombatServiceCharacterInfo character) {
		return !character.IsPlayer && !character.IsDead && (character.State == CombatServiceCharacterStateEnum.Fight ||
		                                                    character.State == CombatServiceCharacterStateEnum.Escape ||
		                                                    character.State == CombatServiceCharacterStateEnum
			                                                    .Surrender ||
		                                                    character.State == CombatServiceCharacterStateEnum.Loot ||
		                                                    character.State == CombatServiceCharacterStateEnum
			                                                    .GoToPoint ||
		                                                    character.State == CombatServiceCharacterStateEnum
			                                                    .ExitingPOI || character.Orders.Exists(x =>
			                                                    x.OrderType == CombatServiceCharacterOrderEnum.Attack ||
			                                                    x.OrderType == CombatServiceCharacterOrderEnum.Escape ||
			                                                    x.OrderType == CombatServiceCharacterOrderEnum
				                                                    .Surrender ||
			                                                    x.OrderType == CombatServiceCharacterOrderEnum.Loot ||
			                                                    x.OrderType == CombatServiceCharacterOrderEnum
				                                                    .GoToPoint));
	}

	private List<CombatServiceCharacterInfo> CharacterAttackers(
		CombatServiceCombatInfo combat,
		CombatServiceCharacterInfo character) {
		var serviceCharacterInfoList = new List<CombatServiceCharacterInfo>();
		foreach (var character1 in combat.Characters)
			if (character1.State == CombatServiceCharacterStateEnum.Fight && character1.CurrentEnemy == character)
				serviceCharacterInfoList.Add(character1);
			else if (character1.Orders.Exists(x =>
				         x.OrderType == CombatServiceCharacterOrderEnum.Attack && x.OrderTarget == character))
				serviceCharacterInfoList.Add(character1);
		return serviceCharacterInfoList;
	}

	private List<CombatServiceCharacterInfo> CharacterLooters(
		CombatServiceCombatInfo combat,
		CombatServiceCharacterInfo character) {
		var serviceCharacterInfoList = new List<CombatServiceCharacterInfo>();
		foreach (var character1 in combat.Characters)
			if (character1.State == CombatServiceCharacterStateEnum.Loot && character1.CurrentEnemy == character)
				serviceCharacterInfoList.Add(character1);
			else if (character1.Orders.Exists(x =>
				         x.OrderType == CombatServiceCharacterOrderEnum.Loot && x.OrderTarget == character))
				serviceCharacterInfoList.Add(character1);
		return serviceCharacterInfoList;
	}

	private void EndCombat(CombatServiceCombatInfo combat) {
		currentCombats.Remove(combat);
		foreach (var character in combat.Characters)
			if (GetCharacterCombats(character).Count == 0 && !character.IsPlayer &&
			    activeCharacters.Contains(character)) {
				SetInactive(character);
				if (!character.IsDead) {
					var component = character.Character.GetComponent<NPCWeaponService>();
					if (component != null)
						component.Weapon = WeaponEnum.Unknown;
				}

				activeCharacters.Remove(character);
			}
	}

	private void TraceCombat(CombatServiceCombatInfo combat) {
		Debug.Log("characters: " + combat.Characters.Count);
		foreach (var character in combat.Characters)
			Debug.Log(character.Character + " / " + character.Fraction + " / " + character.State);
		Debug.Log("escaped characters: ");
		foreach (var escapedCharacter in combat.EscapedCharacters)
			Debug.Log(escapedCharacter.EscapedCharacter.Character + " / " +
			          escapedCharacter.FollowingCharacter.Character);
		Debug.Log("/////////////////////");
	}

	private void SetWatch(CombatServiceCharacterInfo actor, CombatServiceCharacterInfo target) {
		if (actor.IsDead)
			return;
		currentCombats.Find(x => x.Characters.Contains(actor) || x.Characters.Contains(target));
		actor.CurrentEnemy = target;
		actor.State = CombatServiceCharacterStateEnum.WatchFighting;
		BehaviorSubtreeUtility.SetCharacterSubtree(
			BehaviorSubtreeUtility.GetCharacterSubtree(actor.Character.gameObject),
			actor.CombatSettings.WatchFightingAI);
	}

	private void SetAttack(CombatServiceCharacterInfo actor, CombatServiceCharacterInfo target) {
		if (actor == null)
			Debug.LogError("actor is null!");
		else if (target == null)
			Debug.LogError("target is null!");
		else {
			if (actor.IsDead)
				return;
			var combatFractionInfo = currentCombats
				.Find(x => x.Characters.Contains(actor) || x.Characters.Contains(target)).Fractions
				.Find(x => x.Characters.Contains(actor));
			if (combatFractionInfo == null)
				Debug.LogError("ownFraction == null");
			else if (combatFractionInfo.FearFractions == null)
				Debug.LogError("ownFraction.FearFractions == null");
			else if (combatFractionInfo.FearFractions.Contains(target.Fraction))
				SetEscape(actor, target);
			else {
				actor.CurrentEnemy = target;
				actor.State = CombatServiceCharacterStateEnum.Fight;
				actor.FightStartPosition = actor.Character.transform.position;
				actor.LastGotHitPosition = actor.Character.transform.position;
				if (actor.Character == null)
					Debug.LogError("actor character is null!");
				else
					BehaviorSubtreeUtility.SetCharacterSubtree(
						BehaviorSubtreeUtility.GetCharacterSubtree(actor.Character.gameObject),
						actor.CombatSettings.FightAI);
			}
		}
	}

	private void SetEscape(CombatServiceCharacterInfo actor, CombatServiceCharacterInfo target) {
		if (actor.IsDead)
			return;
		if (actor.IsIndoors)
			SetSurrender(actor, target);
		else {
			if (!actor.IsPlayer)
				actor.FireControllerCombatAction(CombatActionEnum.Surrender, target.Entity);
			actor.CurrentEnemy = target;
			actor.State = CombatServiceCharacterStateEnum.Escape;
			BehaviorSubtreeUtility.SetCharacterSubtree(
				BehaviorSubtreeUtility.GetCharacterSubtree(actor.Character.gameObject), actor.CombatSettings.EscapeAI);
		}
	}

	private void SetSurrender(CombatServiceCharacterInfo actor, CombatServiceCharacterInfo target) {
		if (actor.IsDead)
			return;
		if (!actor.IsPlayer && target != null)
			actor.FireControllerCombatAction(CombatActionEnum.Surrender, target.Entity);
		actor.CurrentEnemy = target;
		actor.State = CombatServiceCharacterStateEnum.Surrender;
		BehaviorSubtreeUtility.SetCharacterSubtree(
			BehaviorSubtreeUtility.GetCharacterSubtree(actor.Character.gameObject), actor.CombatSettings.SurrenderAI);
	}

	private void SetLoot(CombatServiceCharacterInfo actor, CombatServiceCharacterInfo target) {
		if (actor.IsDead)
			return;
		actor.CurrentEnemy = target;
		actor.State = CombatServiceCharacterStateEnum.Loot;
		BehaviorSubtreeUtility.SetCharacterSubtree(
			BehaviorSubtreeUtility.GetCharacterSubtree(actor.Character.gameObject), actor.CombatSettings.LootAI);
	}

	private void SetGoToPoint(CombatServiceCharacterInfo actor, Vector3 target) {
		if (actor.IsDead)
			return;
		actor.CurrentEnemy = null;
		actor.State = CombatServiceCharacterStateEnum.GoToPoint;
		var characterSubtree = BehaviorSubtreeUtility.GetCharacterSubtree(actor.Character.gameObject);
		BehaviorSubtreeUtility.SetCharacterSubtree(characterSubtree, actor.CombatSettings.GoToPointAI);
		if (characterSubtree.GetVariable("Target") == null)
			return;
		characterSubtree.SetVariableValue("Target", actor.FightStartPosition);
	}

	private void SetInactive(CombatServiceCharacterInfo actor) {
		if (actor == null || actor.IsPlayer)
			return;
		actor.CurrentEnemy = null;
		actor.State = CombatServiceCharacterStateEnum.Free;
		actor.HearedCries.Clear();
		BehaviorSubtreeUtility.SetCharacterSubtree(
			BehaviorSubtreeUtility.GetCharacterSubtree(actor.Character.gameObject), null);
	}

	public void CharacterCry(
		CombatServiceCharacterInfo character,
		CombatCryEnum cryType,
		CombatServiceCharacterInfo cryTarget = null,
		float timeout = 1f) {
		if (character != null && character.Character != null)
			character.Character.PlayLipSync(cryType);
		var cry = new CombatCry(cryType, character);
		cry.Timeout = timeout;
		cry.Radius = 0.0f;
		cry.CryTarget = cryTarget;
		if (timeout == 0.0)
			ActCry(cry);
		else
			currentCries.Add(cry);
	}

	private void ActCry(CombatCry cry) {
		if (cry.Character == null || cry.Character.Character == null)
			return;
		var hearingCharacters = FindHearingCharacters(cry.Character);
		if (hearingCharacters == null)
			return;
		foreach (var character in hearingCharacters)
			if (!character.IsPlayer && !character.IsDead)
				HearCry(character, cry);
	}

	private void UpdateCries() {
		tmp.Clear();
		foreach (var currentCry in currentCries) {
			currentCry.Timeout -= Time.deltaTime;
			if (currentCry.Timeout <= 0.0) {
				ActCry(currentCry);
				tmp.Add(currentCry);
			}
		}

		foreach (var combatCry in tmp)
			currentCries.Remove(combatCry);
	}

	private void UpdateWaitingOrders() {
		tmp2.Clear();
		foreach (var waitingAttackOrder in waitingAttackOrders) {
			var characterInfo1 = GetCharacterInfo(waitingAttackOrder.attacker);
			var characterInfo2 = GetCharacterInfo(waitingAttackOrder.enemy);
			if (characterInfo1 != null && characterInfo2 != null) {
				tmp2.Add(waitingAttackOrder);
				characterInfo1.PersonalAttackEnemies.Add(characterInfo2);
				characterInfo1.RecountEnemies();
			}
		}

		foreach (var serviceCommonOrder in tmp2)
			waitingAttackOrders.Remove(serviceCommonOrder);
	}

	public CombatServiceCharacterInfo GetCharacterInfo(EnemyBase character) {
		foreach (var character1 in characters)
			if (character1 != null && character1.Character == character)
				return character1;
		return null;
	}

	public CombatServiceCharacterInfo GetCharacterInfo(IEntity entity) {
		CombatServiceCharacterInfo characterInfo;
		if (charactersDictionary.TryGetValue(entity, out characterInfo))
			return characterInfo;
		foreach (var character in characters)
			if (character != null && character.Character != null && character.Entity == entity)
				return character;
		return null;
	}

	public void ComputeUpdate() {
		if (!InstanceByRequest<EngineApplication>.Instance.IsPaused) {
			foreach (var character in characters)
				character.Update();
			var totalSeconds = ServiceLocator.GetService<TimeService>().RealTime.TotalSeconds;
			foreach (var currentCombat in currentCombats) {
				var flag = false;
				var index = 0;
				while (index < currentCombat.EscapedCharacters.Count) {
					var escapedCharacter = currentCombat.EscapedCharacters[index];
					if (totalSeconds <= escapedCharacter.Time + escapedIgnoreTimeout &&
					    escapedCharacter.FollowingCharacter.CanHear(escapedCharacter.EscapedCharacter)) {
						currentCombat.EscapedCharacters.RemoveAt(index);
						flag = true;
					} else
						++index;
				}

				if (flag && !combatsToUpdate.Contains(currentCombat))
					combatsToUpdate.Add(currentCombat);
			}
		}

		if (combatsToUpdate.Count > 0) {
			foreach (var combat in combatsToUpdate)
				UpdateCombatSituation(combat);
			combatsToUpdate.Clear();
			RecountPlayerDanger();
		}

		UpdateCries();
		UpdateWaitingOrders();
	}

	private void RecountPlayerDanger() {
		PlayerIsFighting = false;
		if (currentCombats == null)
			return;
		var all = currentCombats.FindAll(x => x.Characters != null && x.Characters.Exists(y => y.IsPlayer));
		if (all != null)
			foreach (var serviceCombatInfo in all)
				if (serviceCombatInfo.Characters != null)
					foreach (var character in serviceCombatInfo.Characters) {
						if (character.State == CombatServiceCharacterStateEnum.Fight &&
						    character.CurrentEnemy != null && character.CurrentEnemy.IsPlayer) {
							PlayerIsFighting = true;
							return;
						}

						if (character.Orders != null && character.Orders.Exists(x =>
							    x.OrderType == CombatServiceCharacterOrderEnum.Attack && x.OrderTarget != null &&
							    x.OrderTarget.IsPlayer)) {
							PlayerIsFighting = true;
							return;
						}
					}
	}

	private List<CombatServiceCharacterInfo> FindCharactersInRadius(
		CombatServiceCharacterInfo character,
		float radius) {
		if (character.Character == null || character.Character.gameObject == null)
			return null;
		var position = character.Character.gameObject.transform.position;
		var charactersInRadius = new List<CombatServiceCharacterInfo>();
		foreach (var character1 in characters)
			if (character1 != character && character1.IsIndoors == character.IsIndoors &&
			    !(character1.Character == null) && !(character1.Character.gameObject == null) &&
			    (character1.Character.gameObject.transform.position - position).magnitude <= (double)radius)
				charactersInRadius.Add(character1);
		return charactersInRadius;
	}

	private List<CombatServiceCharacterInfo> FindHearingCharacters(
		CombatServiceCharacterInfo character) {
		if (character.Character == null || character.Character.gameObject == null)
			return null;
		var position = character.Character.gameObject.transform.position;
		var hearingCharacters = new List<CombatServiceCharacterInfo>();
		foreach (var character1 in characters)
			if (character1 != character && character1.IsIndoors == character.IsIndoors && character1.CanHear(character))
				hearingCharacters.Add(character1);
		return hearingCharacters;
	}

	private void FilterAimsByRadius(
		CombatServiceCharacterInfo character,
		List<CombatServiceCharacterInfo> aimsList,
		float radius) {
		if (character.Character == null || character.Character.gameObject == null)
			return;
		var position = character.Character.gameObject.transform.position;
		var serviceCharacterInfoList = new List<CombatServiceCharacterInfo>();
		foreach (var aims in aimsList)
			if (!(aims.Character == null) && !(aims.Character.gameObject == null) &&
			    (aims.Character.gameObject.transform.position - position).magnitude <= (double)radius)
				serviceCharacterInfoList.Add(aims);
		aimsList = serviceCharacterInfoList;
	}

	public CombatServiceCharacterInfo FindClosestAim(
		CombatServiceCharacterInfo character,
		List<CombatServiceCharacterInfo> aimsList) {
		if (character.Character == null || character.Character.gameObject == null)
			return null;
		var nullable1 = new float?();
		CombatServiceCharacterInfo closestAim = null;
		var position = character.Character.gameObject.transform.position;
		foreach (var aims in aimsList)
			if (!(aims.Character == null) && !(aims.Character.gameObject == null)) {
				var magnitude = (aims.Character.gameObject.transform.position - position).magnitude;
				int num1;
				if (nullable1.HasValue) {
					var nullable2 = nullable1;
					var num2 = magnitude;
					num1 = (nullable2.GetValueOrDefault() > (double)num2) & nullable2.HasValue ? 1 : 0;
				} else
					num1 = 1;

				if (num1 != 0) {
					nullable1 = magnitude;
					closestAim = aims;
				}
			}

		return closestAim;
	}

	public bool CharacterIsInCombat(EnemyBase character) {
		var actor = GetCharacterInfo(character);
		return actor != null && currentCombats.FindAll(x => x.Characters.Contains(actor)).Count != 0;
	}

	public List<CombatServiceCombatInfo> GetCharacterCombats(CombatServiceCharacterInfo character) {
		return currentCombats.FindAll(x => x.Characters.Contains(character));
	}

	public bool CharacterIsActivated(CombatServiceCharacterInfo character) {
		return activeCharacters.Contains(character);
	}
}