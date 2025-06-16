// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.CombatService
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using BehaviorDesigner.Runtime;
using Engine.Behaviours.Components;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components.Parameters;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace Engine.Source.Services
{
  [RuntimeService(new System.Type[] {typeof (CombatService), typeof (ICombatService)})]
  public class CombatService : IInitialisable, IUpdatable, ICombatService
  {
    private List<CombatServiceCharacterInfo> characters = new List<CombatServiceCharacterInfo>();
    private List<CombatServiceCharacterInfo> activeCharacters = new List<CombatServiceCharacterInfo>();
    private List<CombatServiceCombatInfo> currentCombats = new List<CombatServiceCombatInfo>();
    private List<CombatServiceCombatInfo> combatsToUpdate = new List<CombatServiceCombatInfo>();
    private List<CombatCry> currentCries = new List<CombatCry>();
    private List<CombatServiceCommonOrder> waitingAttackOrders = new List<CombatServiceCommonOrder>();
    private CombatServiceCharacterInfo playerCharacter;
    private Dictionary<IEntity, CombatServiceCharacterInfo> charactersDictionary = new Dictionary<IEntity, CombatServiceCharacterInfo>();
    private float enemiesSearchRadius = 30f;
    private float minimumSurrenderTime = 2f;
    private float escapedIgnoreTimeout = 12f;
    private static List<CombatCry> tmp = new List<CombatCry>();
    private static List<CombatServiceCommonOrder> tmp2 = new List<CombatServiceCommonOrder>();
    private bool playerIsFighting;

    public bool PlayerIsFighting
    {
      get => this.playerIsFighting;
      private set
      {
        this.playerIsFighting = value;
        if (this.playerCharacter == null || !((UnityEngine.Object) this.playerCharacter.Character != (UnityEngine.Object) null))
          return;
        this.playerCharacter.Character.IsFighting = value;
      }
    }

    public void Initialise()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable((IUpdatable) this);
      this.PlayerIsFighting = false;
    }

    public void Terminate()
    {
      this.characters.Clear();
      this.activeCharacters.Clear();
      this.currentCombats.Clear();
      this.combatsToUpdate.Clear();
      this.currentCries.Clear();
      this.waitingAttackOrders.Clear();
      this.charactersDictionary.Clear();
      InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable((IUpdatable) this);
    }

    public void RegisterCharacter(EnemyBase character)
    {
      if ((UnityEngine.Object) character == (UnityEngine.Object) null || !character.isActiveAndEnabled)
        return;
      if (character.Owner != null)
      {
        IParameter<CombatStyleEnum> byName = character.Owner.GetComponent<ParametersComponent>().GetByName<CombatStyleEnum>(ParameterNameEnum.CombatStyle);
        if (byName != null && byName.Value == CombatStyleEnum.Default)
        {
          Pivot component = character.GetComponent<Pivot>();
          if ((UnityEngine.Object) component != (UnityEngine.Object) null && component.DefaultCombatStyle != 0)
            byName.Value = component.DefaultCombatStyle;
        }
      }
      CombatServiceCharacterInfo serviceCharacterInfo = this.characters.Find((Predicate<CombatServiceCharacterInfo>) (x => (UnityEngine.Object) x.Character == (UnityEngine.Object) character));
      if (serviceCharacterInfo == null)
      {
        serviceCharacterInfo = new CombatServiceCharacterInfo(character, this);
        this.characters.Add(serviceCharacterInfo);
        serviceCharacterInfo.State = CombatServiceCharacterStateEnum.Free;
      }
      if (!serviceCharacterInfo.IsPlayer)
        return;
      this.playerCharacter = serviceCharacterInfo;
    }

    public void AddCharacterToDictionary(IEntity entity, CombatServiceCharacterInfo character)
    {
      if (entity == null)
        return;
      this.charactersDictionary[entity] = character;
    }

    public void UnregisterCharacter(EnemyBase character)
    {
      if ((UnityEngine.Object) character == (UnityEngine.Object) null)
        return;
      CombatServiceCharacterInfo character1 = this.characters.Find((Predicate<CombatServiceCharacterInfo>) (x => (UnityEngine.Object) x.Character == (UnityEngine.Object) character));
      foreach (CombatServiceCombatInfo characterCombat in this.GetCharacterCombats(character1))
        this.RemoveCharacterFromCombat(characterCombat, character1);
      this.UpdateCharacterCombats(character1);
      if (character1 == null)
        return;
      if (character1.Entity != null)
        this.charactersDictionary.Remove(character1.Entity);
      this.characters.Remove(character1);
      character1.Clear();
    }

    public void AddPersonalEnemy(IEntity attacker, IEntity enemy)
    {
      CombatServiceCharacterInfo characterInfo1 = this.GetCharacterInfo(attacker);
      CombatServiceCharacterInfo characterInfo2 = this.GetCharacterInfo(enemy);
      if (characterInfo1 != null && characterInfo2 != null)
      {
        characterInfo1.PersonalAttackEnemies.Add(characterInfo2);
        characterInfo1.RecountEnemies();
      }
      else
        this.waitingAttackOrders.Add(new CombatServiceCommonOrder()
        {
          attacker = attacker,
          enemy = enemy
        });
    }

    public void RemovePersonalEnemy(IEntity attacker, IEntity enemy)
    {
      CombatServiceCharacterInfo attackerInfo = this.GetCharacterInfo(attacker);
      CombatServiceCharacterInfo enemyInfo = this.GetCharacterInfo(enemy);
      if (attackerInfo == null || enemyInfo == null || !attackerInfo.PersonalAttackEnemies.Contains(enemyInfo))
        return;
      attackerInfo.PersonalAttackEnemies.Remove(enemyInfo);
      attackerInfo.RecountEnemies();
      CombatServiceCombatInfo combat = this.currentCombats.Find((Predicate<CombatServiceCombatInfo>) (x => x.Characters.Contains(attackerInfo) && x.Characters.Contains(enemyInfo)));
      if (combat != null)
      {
        attackerInfo.State = CombatServiceCharacterStateEnum.Free;
        CombatServiceCombatFractionInfo combatFractionInfo = combat.Fractions.Find((Predicate<CombatServiceCombatFractionInfo>) (x => x.Characters.Contains(attackerInfo)));
        if (combatFractionInfo != null && combatFractionInfo.AttackFractions != null && combatFractionInfo.AttackFractions.Contains(enemyInfo.Fraction))
          combatFractionInfo.AttackFractions.Remove(enemyInfo.Fraction);
        this.AddCombatToUpdate(combat);
      }
    }

    private void CheckPOIExit(CombatServiceCharacterInfo character)
    {
      if (character.IsPlayer || character.IsDead || !character.Character.GetComponent<NpcState>().NeedExtraExitPOI)
        return;
      BehaviorSubtreeUtility.SetCharacterSubtree(BehaviorSubtreeUtility.GetCharacterSubtree(character.Character.gameObject), ScriptableObjectInstance<ResourceFromCodeData>.Instance.POIExtraExit);
      character.State = CombatServiceCharacterStateEnum.ExitingPOI;
    }

    public void EnterCombat(EnemyBase attacker, EnemyBase enemy, bool watch = false)
    {
      this.RegisterCharacter(attacker);
      this.RegisterCharacter(enemy);
      CombatServiceCharacterInfo character = this.GetCharacterInfo(attacker);
      CombatServiceCharacterInfo target = this.GetCharacterInfo(enemy);
      if (character == null || target == null)
        return;
      if (!watch)
      {
        character.FireControllerCombatAction(CombatActionEnum.EnterCombat, enemy.Owner);
        target.FireControllerCombatAction(CombatActionEnum.EnterCombat, attacker.Owner);
      }
      if (!this.activeCharacters.Contains(character))
        this.activeCharacters.Add(character);
      if (character.State != CombatServiceCharacterStateEnum.ExitingPOI)
        character.State = CombatServiceCharacterStateEnum.Free;
      this.CheckPOIExit(character);
      this.CheckPOIExit(target);
      this.AddCharactersToCombat(character, target);
      this.MergeAllCombats(character, target);
      CombatServiceCombatInfo combat = this.currentCombats.Find((Predicate<CombatServiceCombatInfo>) (x => x.Characters.Contains(character) && x.Characters.Contains(target)));
      if (watch)
      {
        character.Orders.Add(new CombatServiceCharacterOrder()
        {
          OrderType = CombatServiceCharacterOrderEnum.Watch,
          OrderTarget = target
        });
      }
      else
      {
        this.HostileAction(combat, character, target);
        this.HostileAction(combat, target, character);
        character.LastGotHitPosition = character.Character.transform.position;
      }
      if (combat.EscapedCharacters.Exists((Predicate<CombatServiceEscapedCharacterInfo>) (x => x.EscapedCharacter == character)))
        combat.EscapedCharacters.RemoveAll((Predicate<CombatServiceEscapedCharacterInfo>) (x => x.EscapedCharacter == character));
      this.AddCombatToUpdate(combat);
    }

    public void ExitCombat(EnemyBase attacker)
    {
      CombatServiceCharacterInfo character = this.GetCharacterInfo(attacker);
      if (this.activeCharacters.Contains(character))
        this.activeCharacters.Remove(character);
      foreach (CombatServiceCombatInfo combat in this.currentCombats.FindAll((Predicate<CombatServiceCombatInfo>) (x => x.Characters.Contains(character))))
        this.AddCombatToUpdate(combat);
      this.SetInactive(character);
      if ((UnityEngine.Object) character?.Character?.gameObject == (UnityEngine.Object) null)
        return;
      WeaponServiceBase component = character?.Character?.gameObject?.GetComponent<WeaponServiceBase>();
      if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
        return;
      component.Weapon = WeaponEnum.Unknown;
    }

    public void HitNpc(IEntity actor, IEntity target)
    {
      CombatServiceCharacterInfo character = this.GetCharacterInfo(actor);
      CombatServiceCharacterInfo enemy = this.GetCharacterInfo(target);
      if (character == null || enemy == null || character.IsCombatIgnored || enemy.IsCombatIgnored)
        return;
      character.LastGotHitPosition = character.Character.transform.position;
      CombatServiceCombatInfo serviceCombatInfo = this.currentCombats.Find((Predicate<CombatServiceCombatInfo>) (x => x.Characters.Contains(character) || x.Characters.Contains(enemy)));
      if (serviceCombatInfo == null || !serviceCombatInfo.Characters.Contains(character) || !serviceCombatInfo.Characters.Contains(enemy))
        this.AddCharactersToCombat(character, enemy);
      this.MergeAllCombats(character, enemy);
      CombatServiceCombatInfo combat = this.currentCombats.Find((Predicate<CombatServiceCombatInfo>) (x => x.Characters.Contains(character) && x.Characters.Contains(enemy)));
      if (combat != null)
      {
        if (enemy.State == CombatServiceCharacterStateEnum.WatchFighting)
          enemy.State = CombatServiceCharacterStateEnum.Free;
        if (enemy.State == CombatServiceCharacterStateEnum.Escape || character.IsPlayer)
          enemy.CurrentEnemy = character;
        this.HostileAction(combat, character, enemy);
      }
      if (combat.EscapedCharacters.Exists((Predicate<CombatServiceEscapedCharacterInfo>) (x => x.EscapedCharacter == character)))
      {
        combat.EscapedCharacters.RemoveAll((Predicate<CombatServiceEscapedCharacterInfo>) (x => x.EscapedCharacter == character));
        this.AddCombatToUpdate(combat);
      }
      if (!enemy.IsIndoors && enemy.State == CombatServiceCharacterStateEnum.Surrender)
        this.CheckNpcEscapeFromSurrender(enemy, combat);
      this.CharacterCryHelp(enemy, character);
    }

    private void CheckNpcEscapeFromSurrender(
      CombatServiceCharacterInfo character,
      CombatServiceCombatInfo combat)
    {
      if ((double) character.GetTimeFromLastOrder() < (double) this.minimumSurrenderTime)
        return;
      character.Orders.Add(new CombatServiceCharacterOrder()
      {
        OrderType = CombatServiceCharacterOrderEnum.Escape,
        OrderTarget = character
      });
      this.AddCombatToUpdate(combat);
    }

    public void CharacterCryHelp(
      CombatServiceCharacterInfo character,
      CombatServiceCharacterInfo enemy)
    {
      List<CombatServiceCharacterInfo> hearingCharacters = this.FindHearingCharacters(character);
      if (hearingCharacters.Count == 0)
        return;
      CombatServiceCombatInfo combat = this.currentCombats.Find((Predicate<CombatServiceCombatInfo>) (x => x.Characters.Contains(character)));
      hearingCharacters.RemoveAll((Predicate<CombatServiceCharacterInfo>) (x => combat.Characters.Contains(x)));
      if (hearingCharacters.Count == 0)
        return;
      CombatServiceCombatFractionInfo fraction = combat.Fractions.Find((Predicate<CombatServiceCombatFractionInfo>) (x => x.Characters.Contains(character)));
      hearingCharacters.RemoveAll((Predicate<CombatServiceCharacterInfo>) (x => !fraction.AskForHelpFractions.Contains(x.Fraction)));
      if (hearingCharacters.Count == 0)
        return;
      this.CharacterCry(character, CombatCryEnum.NeedHelp, enemy, 0.0f);
    }

    public void CharacterCry(EnemyBase character, CombatCryEnum cryType)
    {
      this.CharacterCry(this.GetCharacterInfo(character), cryType);
    }

    private void HearCry(CombatServiceCharacterInfo character, CombatCry cry)
    {
      CombatServiceCombatInfo combat = this.currentCombats.Find((Predicate<CombatServiceCombatInfo>) (x => x.Characters.Contains(character) || x.Characters.Contains(cry.Character)));
      if (combat == null || !combat.Characters.Contains(character) || !combat.Characters.Contains(cry.Character))
      {
        this.AddCharactersToCombat(character, cry.Character);
        this.MergeAllCombats(character, cry.Character);
        combat = this.currentCombats.Find((Predicate<CombatServiceCombatInfo>) (x => x.Characters.Contains(character) && x.Characters.Contains(cry.Character)));
      }
      character.HearedCries.Add(cry);
      if (cry.CryType != CombatCryEnum.NeedHelp || combat == null)
        return;
      this.HostileAction(combat, cry.CryTarget, cry.Character);
      this.AddCombatToUpdate(combat);
    }

    public void LostSight(EnemyBase actor, EnemyBase target)
    {
      CombatServiceCharacterInfo character = this.GetCharacterInfo(actor);
      CombatServiceCharacterInfo enemy = this.GetCharacterInfo(target);
      CombatServiceCombatInfo serviceCombatInfo = this.currentCombats.Find((Predicate<CombatServiceCombatInfo>) (x => x.Characters.Contains(character)));
      if (character == null)
        return;
      character.CurrentEnemy = (CombatServiceCharacterInfo) null;
      character.State = CombatServiceCharacterStateEnum.Free;
      if (enemy != null && !enemy.IsDead && serviceCombatInfo != null && serviceCombatInfo.EscapedCharacters != null && !serviceCombatInfo.EscapedCharacters.Exists((Predicate<CombatServiceEscapedCharacterInfo>) (x => x.EscapedCharacter == enemy && x.FollowingCharacter == character)))
        serviceCombatInfo.EscapedCharacters.Add(new CombatServiceEscapedCharacterInfo()
        {
          EscapedCharacter = enemy,
          FollowingCharacter = character,
          Time = ServiceLocator.GetService<TimeService>().RealTime.TotalSeconds
        });
      character.Orders.Add(new CombatServiceCharacterOrder()
      {
        OrderType = CombatServiceCharacterOrderEnum.GoToPoint,
        OrderPoint = character.FightStartPosition
      });
      this.UpdateCharacterCombats(character);
    }

    public void IndividualEscape(EnemyBase actor, EnemyBase target)
    {
      CombatServiceCharacterInfo characterInfo1 = this.GetCharacterInfo(actor);
      CombatServiceCharacterInfo characterInfo2 = this.GetCharacterInfo(target);
      if (characterInfo1 == null || characterInfo1.State == CombatServiceCharacterStateEnum.Surrender && !characterInfo2.IsPlayer)
        return;
      characterInfo1.Orders.Add(new CombatServiceCharacterOrder()
      {
        OrderType = characterInfo1.IsIndoors ? CombatServiceCharacterOrderEnum.Surrender : CombatServiceCharacterOrderEnum.Escape,
        OrderTarget = characterInfo2
      });
      this.UpdateCharacterCombats(characterInfo1);
    }

    public void IndividualSurrender(EnemyBase actor, EnemyBase target)
    {
      CombatServiceCharacterInfo characterInfo1 = this.GetCharacterInfo(actor);
      CombatServiceCharacterInfo characterInfo2 = this.GetCharacterInfo(target);
      if (characterInfo1 == null)
        return;
      characterInfo1.WasBeaten = true;
      characterInfo1.Orders.Add(new CombatServiceCharacterOrder()
      {
        OrderType = CombatServiceCharacterOrderEnum.Surrender,
        OrderTarget = characterInfo2
      });
      this.UpdateCharacterCombats(characterInfo1);
    }

    public void IndividualFinishOrder(EnemyBase actor)
    {
      CombatServiceCharacterInfo character = this.GetCharacterInfo(actor);
      if (character == null)
        return;
      if (character.State == CombatServiceCharacterStateEnum.ExitingPOI)
        character.State = CombatServiceCharacterStateEnum.Free;
      if (character.State == CombatServiceCharacterStateEnum.Fight)
        character.State = CombatServiceCharacterStateEnum.Free;
      if (character.State == CombatServiceCharacterStateEnum.Surrender && !character.CurrentEnemy.IsDead && !character.IsIndoors && character.CurrentEnemy.IsPlayer)
        character.Orders.Add(new CombatServiceCharacterOrder()
        {
          OrderType = CombatServiceCharacterOrderEnum.Escape,
          OrderTarget = character.CurrentEnemy
        });
      if (character.State == CombatServiceCharacterStateEnum.Loot)
      {
        if (character.CurrentEnemy != null)
          character.CurrentEnemy.WasLooted = true;
        character.State = CombatServiceCharacterStateEnum.Free;
      }
      if (character.State == CombatServiceCharacterStateEnum.GoToPoint)
        character.State = CombatServiceCharacterStateEnum.Free;
      if (character.State == CombatServiceCharacterStateEnum.Escape)
      {
        character.State = CombatServiceCharacterStateEnum.Free;
        CombatServiceCombatInfo serviceCombatInfo = this.currentCombats.Find((Predicate<CombatServiceCombatInfo>) (x => x.Characters.Contains(character)));
        if (serviceCombatInfo != null && !serviceCombatInfo.EscapedCharacters.Exists((Predicate<CombatServiceEscapedCharacterInfo>) (x => x.EscapedCharacter == character && x.FollowingCharacter == character.CurrentEnemy)))
          serviceCombatInfo.EscapedCharacters.Add(new CombatServiceEscapedCharacterInfo()
          {
            EscapedCharacter = character,
            FollowingCharacter = character.CurrentEnemy,
            Time = ServiceLocator.GetService<TimeService>().RealTime.TotalSeconds
          });
      }
      if (character.State == CombatServiceCharacterStateEnum.WatchFighting)
        character.State = CombatServiceCharacterStateEnum.Free;
      this.UpdateCharacterCombats(character);
    }

    public void ChangedLocation(CombatServiceCharacterInfo character)
    {
      CombatServiceCombatInfo combat = this.currentCombats.Find((Predicate<CombatServiceCombatInfo>) (x => x.Characters.Contains(character)));
      if (combat == null)
        return;
      this.RemoveCharacterFromCombat(combat, character);
    }

    private void HostileAction(
      CombatServiceCombatInfo combat,
      CombatServiceCharacterInfo agressor,
      CombatServiceCharacterInfo reporter)
    {
      bool flag = false;
      foreach (CombatServiceCombatFractionInfo fraction in combat.Fractions)
      {
        if (fraction.ChangeOnHostileAction(agressor, reporter))
          flag = true;
      }
      if (!flag)
        return;
      this.AddCombatToUpdate(combat);
    }

    public void Died(CombatServiceCharacterInfo actor)
    {
      if (!actor.IsPlayer)
        actor.FireControllerCombatAction(CombatActionEnum.Death, actor.NpcController.LastAttacker);
      this.CharacterCry(actor, CombatCryEnum.Death);
      actor.CurrentEnemy = (CombatServiceCharacterInfo) null;
      actor.HearedCries.Clear();
      actor.State = CombatServiceCharacterStateEnum.Dead;
      this.UpdateCharacterCombats(actor);
    }

    private void MergeAllCombats(
      CombatServiceCharacterInfo actor,
      CombatServiceCharacterInfo target)
    {
      List<CombatServiceCombatInfo> all = this.currentCombats.FindAll((Predicate<CombatServiceCombatInfo>) (x => x.Characters.Contains(actor)));
      foreach (CombatServiceCombatInfo serviceCombatInfo in this.currentCombats.FindAll((Predicate<CombatServiceCombatInfo>) (x => x.Characters.Contains(target))))
      {
        if (!all.Contains(serviceCombatInfo))
          all.Add(serviceCombatInfo);
      }
      if (all.Count <= 1)
        return;
      CombatServiceCombatInfo firstCombat = all[0];
      for (int index = 1; index < all.Count; ++index)
      {
        CombatServiceCombatInfo secondCombat = all[index];
        this.MergeCombats(firstCombat, secondCombat);
        if (this.currentCombats.Contains(secondCombat))
          this.currentCombats.Remove(secondCombat);
        if (this.combatsToUpdate.Contains(secondCombat))
          this.combatsToUpdate.Remove(secondCombat);
      }
    }

    private void MergeCombats(
      CombatServiceCombatInfo firstCombat,
      CombatServiceCombatInfo secondCombat)
    {
      foreach (CombatServiceCharacterInfo character in secondCombat.Characters)
      {
        if (!firstCombat.Characters.Contains(character))
          firstCombat.Characters.Add(character);
      }
      foreach (CombatServiceEscapedCharacterInfo escapedCharacter in secondCombat.EscapedCharacters)
        firstCombat.EscapedCharacters.Add(escapedCharacter);
      foreach (CombatServiceCombatFractionInfo fraction in secondCombat.Fractions)
      {
        CombatServiceCombatFractionInfo secondCombatFraction = fraction;
        CombatServiceCombatFractionInfo combatFractionInfo = firstCombat.Fractions.Find((Predicate<CombatServiceCombatFractionInfo>) (x => x.Fraction == secondCombatFraction.Fraction));
        if (combatFractionInfo != null)
        {
          foreach (CombatServiceCharacterInfo character in secondCombatFraction.Characters)
          {
            if (!combatFractionInfo.Characters.Contains(character))
              combatFractionInfo.Characters.Add(character);
          }
          foreach (FractionEnum attackFraction in secondCombatFraction.AttackFractions)
          {
            if (!combatFractionInfo.AttackFractions.Contains(attackFraction))
              combatFractionInfo.AttackFractions.Add(attackFraction);
          }
          foreach (FractionEnum fearFraction in secondCombatFraction.FearFractions)
          {
            if (!combatFractionInfo.FearFractions.Contains(fearFraction))
              combatFractionInfo.FearFractions.Add(fearFraction);
          }
        }
        else
          firstCombat.Fractions.Add(secondCombatFraction);
      }
    }

    private void UpdateCharacterCombats(CombatServiceCharacterInfo character)
    {
      foreach (CombatServiceCombatInfo combat in this.currentCombats.FindAll((Predicate<CombatServiceCombatInfo>) (x => x.Characters.Contains(character))))
        this.AddCombatToUpdate(combat);
    }

    private void AddCharactersToCombat(
      CombatServiceCharacterInfo attacker,
      CombatServiceCharacterInfo enemy)
    {
      CombatServiceCombatInfo combat = this.currentCombats.Find((Predicate<CombatServiceCombatInfo>) (x => x.Characters.Contains(attacker) || x.Characters.Contains(enemy))) ?? this.CreateCombat();
      if (!combat.Characters.Contains(attacker))
      {
        combat.Characters.Add(attacker);
        this.InsertCharacterInCombatFractions(combat, attacker);
      }
      if (!combat.Characters.Contains(enemy))
      {
        combat.Characters.Add(enemy);
        this.InsertCharacterInCombatFractions(combat, enemy);
      }
      this.AddCombatToUpdate(combat);
    }

    private CombatServiceCombatInfo CreateCombat()
    {
      CombatServiceCombatInfo combat = new CombatServiceCombatInfo();
      combat.Characters = new List<CombatServiceCharacterInfo>();
      combat.Fractions = new List<CombatServiceCombatFractionInfo>();
      combat.EscapedCharacters = new List<CombatServiceEscapedCharacterInfo>();
      this.currentCombats.Add(combat);
      return combat;
    }

    private void InsertCharacterInCombatFractions(
      CombatServiceCombatInfo combat,
      CombatServiceCharacterInfo character)
    {
      if (character == null)
        return;
      CombatServiceCombatFractionInfo combatFractionInfo = combat.Fractions.Find((Predicate<CombatServiceCombatFractionInfo>) (x => x.Fraction == character.Fraction));
      if (combatFractionInfo == null)
      {
        combatFractionInfo = new CombatServiceCombatFractionInfo(character.Fraction);
        combatFractionInfo.Characters = new List<CombatServiceCharacterInfo>();
        combat.Fractions.Add(combatFractionInfo);
      }
      combatFractionInfo.Characters.Add(character);
    }

    private void RemoveCharacterFromCombat(
      CombatServiceCombatInfo combat,
      CombatServiceCharacterInfo character)
    {
      combat.Characters.Remove(character);
      CombatServiceCombatFractionInfo combatFractionInfo = combat.Fractions.Find((Predicate<CombatServiceCombatFractionInfo>) (x => x.Characters.Contains(character)));
      if (combatFractionInfo != null)
      {
        combatFractionInfo.Characters.Remove(character);
        if (combatFractionInfo.Characters.Count == 0)
          combat.Fractions.Remove(combatFractionInfo);
      }
      if (this.GetCharacterCombats(character).Count == 0 && this.activeCharacters.Contains(character))
        character.ClearCombatInfo();
      this.AddCombatToUpdate(combat);
    }

    private void AddCombatToUpdate(CombatServiceCombatInfo combat)
    {
      if (this.combatsToUpdate.Contains(combat))
        return;
      this.combatsToUpdate.Add(combat);
    }

    private void UpdateCombatSituation(CombatServiceCombatInfo combat)
    {
      combat.EscapedCharacters.RemoveAll((Predicate<CombatServiceEscapedCharacterInfo>) (x => x.EscapedCharacter == null || (UnityEngine.Object) x.EscapedCharacter.Character == (UnityEngine.Object) null || x.FollowingCharacter == null || (UnityEngine.Object) x.FollowingCharacter.Character == (UnityEngine.Object) null));
      foreach (CombatServiceCharacterInfo character in combat.Characters)
      {
        if (!character.IsPlayer && !character.IsDead && character.State != CombatServiceCharacterStateEnum.ExitingPOI)
        {
          if (this.CanCancelCurrentAction(combat, character))
          {
            if (character.CurrentEnemy != null && character.CurrentEnemy.IsDead)
              this.CharacterCry(character, CombatCryEnum.FightWin);
            this.GenerateCombatOrders(combat, character);
          }
          if (this.activeCharacters.Contains(character) && character.Orders.Count > 0)
          {
            this.TryExecuteCombatOrders(combat, character);
            character.Orders.Clear();
          }
        }
      }
      this.ClearCombatFromFreeCharacters(combat);
      if (combat.Characters.Exists((Predicate<CombatServiceCharacterInfo>) (x => this.CharacterIsCombatActive(x))))
        return;
      this.EndCombat(combat);
    }

    private void ClearCombatFromFreeCharacters(CombatServiceCombatInfo combat)
    {
      List<CombatServiceCharacterInfo> serviceCharacterInfoList = new List<CombatServiceCharacterInfo>();
      foreach (CombatServiceCharacterInfo character in combat.Characters)
      {
        if ((UnityEngine.Object) character.Character == (UnityEngine.Object) null)
          serviceCharacterInfoList.Add(character);
        if (!character.IsPlayer && character.State == CombatServiceCharacterStateEnum.Free && character.Orders.FindAll((Predicate<CombatServiceCharacterOrder>) (x => x.OrderType != CombatServiceCharacterOrderEnum.GoInactive)).Count == 0)
          serviceCharacterInfoList.Add(character);
      }
      foreach (CombatServiceCharacterInfo character in serviceCharacterInfoList)
        this.RemoveCharacterFromCombat(combat, character);
    }

    private bool TryExecuteCombatOrders(
      CombatServiceCombatInfo combat,
      CombatServiceCharacterInfo character)
    {
      CombatServiceCharacterOrder serviceCharacterOrder1 = character.Orders.Find((Predicate<CombatServiceCharacterOrder>) (x => x.OrderType == CombatServiceCharacterOrderEnum.Escape));
      if (serviceCharacterOrder1 != null)
      {
        this.SetEscape(character, serviceCharacterOrder1.OrderTarget);
        return true;
      }
      CombatServiceCharacterOrder serviceCharacterOrder2 = character.Orders.Find((Predicate<CombatServiceCharacterOrder>) (x => x.OrderType == CombatServiceCharacterOrderEnum.Surrender));
      if (serviceCharacterOrder2 != null)
      {
        this.SetSurrender(character, serviceCharacterOrder2.OrderTarget);
        return true;
      }
      CombatServiceCharacterOrder serviceCharacterOrder3 = character.Orders.Find((Predicate<CombatServiceCharacterOrder>) (x => x.OrderType == CombatServiceCharacterOrderEnum.Attack));
      if (serviceCharacterOrder3 != null)
      {
        this.SetAttack(character, serviceCharacterOrder3.OrderTarget);
        return true;
      }
      CombatServiceCharacterOrder serviceCharacterOrder4 = character.Orders.Find((Predicate<CombatServiceCharacterOrder>) (x => x.OrderType == CombatServiceCharacterOrderEnum.Loot));
      if (serviceCharacterOrder4 != null)
      {
        this.SetLoot(character, serviceCharacterOrder4.OrderTarget);
        return true;
      }
      CombatServiceCharacterOrder serviceCharacterOrder5 = character.Orders.Find((Predicate<CombatServiceCharacterOrder>) (x => x.OrderType == CombatServiceCharacterOrderEnum.GoToPoint));
      if (serviceCharacterOrder5 != null)
      {
        this.SetGoToPoint(character, serviceCharacterOrder5.OrderPoint);
        return true;
      }
      CombatServiceCharacterOrder serviceCharacterOrder6 = character.Orders.Find((Predicate<CombatServiceCharacterOrder>) (x => x.OrderType == CombatServiceCharacterOrderEnum.Watch));
      if (serviceCharacterOrder6 != null)
      {
        this.SetWatch(character, serviceCharacterOrder6.OrderTarget);
        return true;
      }
      if (character.Orders.Find((Predicate<CombatServiceCharacterOrder>) (x => x.OrderType == CombatServiceCharacterOrderEnum.GoInactive)) == null)
        return false;
      this.SetInactive(character);
      return true;
    }

    private bool CanCancelCurrentAction(
      CombatServiceCombatInfo combat,
      CombatServiceCharacterInfo character)
    {
      return character.State == CombatServiceCharacterStateEnum.Fight && ((UnityEngine.Object) character.CurrentEnemy.Character == (UnityEngine.Object) null || character.CurrentEnemy.IsDead || character.CurrentEnemy.State == CombatServiceCharacterStateEnum.Surrender || character.IsIndoors != character.CurrentEnemy.IsIndoors) || character.State == CombatServiceCharacterStateEnum.Escape && ((UnityEngine.Object) character.CurrentEnemy.Character == (UnityEngine.Object) null || character.CurrentEnemy.IsDead || character.CurrentEnemy.State == CombatServiceCharacterStateEnum.Escape || character.CurrentEnemy.State == CombatServiceCharacterStateEnum.Surrender || character.CurrentEnemy.Orders.Exists((Predicate<CombatServiceCharacterOrder>) (x => x.OrderType == CombatServiceCharacterOrderEnum.Escape || x.OrderType == CombatServiceCharacterOrderEnum.Surrender))) || character.State == CombatServiceCharacterStateEnum.Free || character.State == CombatServiceCharacterStateEnum.WatchFighting || character.State == CombatServiceCharacterStateEnum.Loot && (character.PersonalAttackEnemies.Exists((Predicate<CombatServiceCharacterInfo>) (x => this.CharacterCanBeAttacked(x) && combat.Characters.Contains(x))) || character.PersonalFearEnemies.Exists((Predicate<CombatServiceCharacterInfo>) (x => this.CharacterCanFight(x) && combat.Characters.Contains(x)))) || character.State == CombatServiceCharacterStateEnum.GoToPoint || !combat.Characters.Contains(character.CurrentEnemy);
    }

    private void GenerateCombatOrders(
      CombatServiceCombatInfo combat,
      CombatServiceCharacterInfo character)
    {
      CombatServiceCombatFractionInfo combatFractionInfo = combat.Fractions.Find((Predicate<CombatServiceCombatFractionInfo>) (x => x.Characters.Contains(character)));
      List<FractionEnum> fearFractions = combatFractionInfo.FearFractions;
      List<FractionEnum> attackFractions = combatFractionInfo.AttackFractions;
      bool flag1 = false;
      List<CombatServiceCharacterInfo> fearList = new List<CombatServiceCharacterInfo>();
      if (fearFractions.Count > 0 || character.PersonalFearEnemies.Count > 0 || character.WasBeaten || !character.CanFight)
      {
        fearList.AddRange((IEnumerable<CombatServiceCharacterInfo>) combat.Characters.FindAll((Predicate<CombatServiceCharacterInfo>) (x => fearFractions.Contains(x.Fraction) && this.CharacterCanFight(x))));
        if (character.WasBeaten || !character.CanFight)
          fearList.AddRange((IEnumerable<CombatServiceCharacterInfo>) combat.Characters.FindAll((Predicate<CombatServiceCharacterInfo>) (x => (attackFractions.Contains(x.Fraction) || character.PersonalAttackEnemies.Contains(x)) && this.CharacterCanFight(x))));
        List<CombatServiceCharacterInfo> all = character.PersonalFearEnemies.FindAll((Predicate<CombatServiceCharacterInfo>) (x => !fearList.Contains(x) && this.CharacterCanFight(x) && combat.Characters.Contains(x)));
        fearList.AddRange((IEnumerable<CombatServiceCharacterInfo>) all);
        fearList.RemoveAll((Predicate<CombatServiceCharacterInfo>) (x => combat.EscapedCharacters.Exists((Predicate<CombatServiceEscapedCharacterInfo>) (y => y.EscapedCharacter == character && y.FollowingCharacter == x))));
        fearList.RemoveAll((Predicate<CombatServiceCharacterInfo>) (x => character.IsIndoors != x.IsIndoors));
        this.FilterAimsByRadius(character, fearList, this.enemiesSearchRadius);
        if (fearList.Count > 0)
        {
          character.Orders.Add(new CombatServiceCharacterOrder()
          {
            OrderType = CombatServiceCharacterOrderEnum.Escape,
            OrderTarget = this.FindClosestAim(character, fearList)
          });
          flag1 = true;
        }
      }
      if (!flag1 && !character.WasBeaten && character.CanFight && (attackFractions.Count > 0 || character.PersonalAttackEnemies.Count > 0))
      {
        List<CombatServiceCharacterInfo> attackList = combat.Characters.FindAll((Predicate<CombatServiceCharacterInfo>) (x => attackFractions.Contains(x.Fraction) && this.CharacterCanBeAttacked(x)));
        List<CombatServiceCharacterInfo> all1 = character.PersonalAttackEnemies.FindAll((Predicate<CombatServiceCharacterInfo>) (x => !attackList.Contains(x) && this.CharacterCanBeAttacked(x) && combat.Characters.Contains(x)));
        attackList.AddRange((IEnumerable<CombatServiceCharacterInfo>) all1);
        attackList.RemoveAll((Predicate<CombatServiceCharacterInfo>) (x => fearList.Contains(x)));
        attackList.RemoveAll((Predicate<CombatServiceCharacterInfo>) (x => combat.EscapedCharacters.Exists((Predicate<CombatServiceEscapedCharacterInfo>) (y => y.EscapedCharacter == x && y.FollowingCharacter == character))));
        attackList.RemoveAll((Predicate<CombatServiceCharacterInfo>) (x => combat.EscapedCharacters.Exists((Predicate<CombatServiceEscapedCharacterInfo>) (y => y.EscapedCharacter == character && y.FollowingCharacter == x))));
        attackList.RemoveAll((Predicate<CombatServiceCharacterInfo>) (x => character.IsIndoors != x.IsIndoors));
        this.FilterAimsByRadius(character, attackList, this.enemiesSearchRadius);
        List<CombatServiceCharacterInfo> all2 = attackList.FindAll((Predicate<CombatServiceCharacterInfo>) (x => this.CharacterAttackers(combat, x).Count == 0));
        bool flag2 = false;
        if (all2.Count > 0)
        {
          attackList = all2;
          flag2 = true;
        }
        if (attackList.Count > 0)
        {
          CombatServiceCharacterInfo closestAim = this.FindClosestAim(character, attackList);
          character.Orders.Add(new CombatServiceCharacterOrder()
          {
            OrderType = CombatServiceCharacterOrderEnum.Attack,
            OrderTarget = closestAim
          });
          if (flag2 && !closestAim.IsPlayer)
          {
            if (closestAim.State == CombatServiceCharacterStateEnum.Fight || closestAim.State == CombatServiceCharacterStateEnum.Escape)
              closestAim.CurrentEnemy = character;
            else
              closestAim.Orders.Add(new CombatServiceCharacterOrder()
              {
                OrderType = CombatServiceCharacterOrderEnum.Attack,
                OrderTarget = character
              });
          }
        }
      }
      if (character.Orders.Count != 0)
        return;
      if (character.CanLoot)
      {
        List<CombatServiceCharacterInfo> lootList = combat.Characters.FindAll((Predicate<CombatServiceCharacterInfo>) (x => attackFractions.Contains(x.Fraction) && x.IsDead && !x.WasLooted));
        List<CombatServiceCharacterInfo> all = character.PersonalAttackEnemies.FindAll((Predicate<CombatServiceCharacterInfo>) (x => !lootList.Contains(x) && x.IsDead && !x.WasLooted));
        lootList.AddRange((IEnumerable<CombatServiceCharacterInfo>) all);
        lootList.RemoveAll((Predicate<CombatServiceCharacterInfo>) (x => this.CharacterLooters(combat, x).Count > 0));
        if (lootList.Count > 0)
          character.Orders.Add(new CombatServiceCharacterOrder()
          {
            OrderType = CombatServiceCharacterOrderEnum.Loot,
            OrderTarget = this.FindClosestAim(character, lootList)
          });
      }
      if (character.State == CombatServiceCharacterStateEnum.GoToPoint || character.State == CombatServiceCharacterStateEnum.WatchFighting && combat.Characters.Exists((Predicate<CombatServiceCharacterInfo>) (x => x.State == CombatServiceCharacterStateEnum.Fight)))
        return;
      character.Orders.Add(new CombatServiceCharacterOrder()
      {
        OrderType = CombatServiceCharacterOrderEnum.GoInactive
      });
    }

    private bool CharacterCanFight(CombatServiceCharacterInfo character)
    {
      return !character.IsDead && (character.IsPlayer || !character.IsCombatIgnored && !character.WasBeaten && character.CanFight && character.State != CombatServiceCharacterStateEnum.Surrender && character.State != CombatServiceCharacterStateEnum.Escape && !character.Orders.Exists((Predicate<CombatServiceCharacterOrder>) (x => x.OrderType == CombatServiceCharacterOrderEnum.Escape || x.OrderType == CombatServiceCharacterOrderEnum.Surrender)));
    }

    private bool CharacterCanBeAttacked(CombatServiceCharacterInfo character)
    {
      return !character.IsDead && (character.IsPlayer || !character.IsCombatIgnored && !character.IsImmortal && character.State != CombatServiceCharacterStateEnum.Surrender);
    }

    private bool CharacterIsCombatActive(CombatServiceCharacterInfo character)
    {
      return !character.IsPlayer && !character.IsDead && (character.State == CombatServiceCharacterStateEnum.Fight || character.State == CombatServiceCharacterStateEnum.Escape || character.State == CombatServiceCharacterStateEnum.Surrender || character.State == CombatServiceCharacterStateEnum.Loot || character.State == CombatServiceCharacterStateEnum.GoToPoint || character.State == CombatServiceCharacterStateEnum.ExitingPOI || character.Orders.Exists((Predicate<CombatServiceCharacterOrder>) (x => x.OrderType == CombatServiceCharacterOrderEnum.Attack || x.OrderType == CombatServiceCharacterOrderEnum.Escape || x.OrderType == CombatServiceCharacterOrderEnum.Surrender || x.OrderType == CombatServiceCharacterOrderEnum.Loot || x.OrderType == CombatServiceCharacterOrderEnum.GoToPoint)));
    }

    private List<CombatServiceCharacterInfo> CharacterAttackers(
      CombatServiceCombatInfo combat,
      CombatServiceCharacterInfo character)
    {
      List<CombatServiceCharacterInfo> serviceCharacterInfoList = new List<CombatServiceCharacterInfo>();
      foreach (CombatServiceCharacterInfo character1 in combat.Characters)
      {
        if (character1.State == CombatServiceCharacterStateEnum.Fight && character1.CurrentEnemy == character)
          serviceCharacterInfoList.Add(character1);
        else if (character1.Orders.Exists((Predicate<CombatServiceCharacterOrder>) (x => x.OrderType == CombatServiceCharacterOrderEnum.Attack && x.OrderTarget == character)))
          serviceCharacterInfoList.Add(character1);
      }
      return serviceCharacterInfoList;
    }

    private List<CombatServiceCharacterInfo> CharacterLooters(
      CombatServiceCombatInfo combat,
      CombatServiceCharacterInfo character)
    {
      List<CombatServiceCharacterInfo> serviceCharacterInfoList = new List<CombatServiceCharacterInfo>();
      foreach (CombatServiceCharacterInfo character1 in combat.Characters)
      {
        if (character1.State == CombatServiceCharacterStateEnum.Loot && character1.CurrentEnemy == character)
          serviceCharacterInfoList.Add(character1);
        else if (character1.Orders.Exists((Predicate<CombatServiceCharacterOrder>) (x => x.OrderType == CombatServiceCharacterOrderEnum.Loot && x.OrderTarget == character)))
          serviceCharacterInfoList.Add(character1);
      }
      return serviceCharacterInfoList;
    }

    private void EndCombat(CombatServiceCombatInfo combat)
    {
      this.currentCombats.Remove(combat);
      foreach (CombatServiceCharacterInfo character in combat.Characters)
      {
        if (this.GetCharacterCombats(character).Count == 0 && !character.IsPlayer && this.activeCharacters.Contains(character))
        {
          this.SetInactive(character);
          if (!character.IsDead)
          {
            NPCWeaponService component = character.Character.GetComponent<NPCWeaponService>();
            if ((UnityEngine.Object) component != (UnityEngine.Object) null)
              component.Weapon = WeaponEnum.Unknown;
          }
          this.activeCharacters.Remove(character);
        }
      }
    }

    private void TraceCombat(CombatServiceCombatInfo combat)
    {
      Debug.Log((object) ("characters: " + (object) combat.Characters.Count));
      foreach (CombatServiceCharacterInfo character in combat.Characters)
        Debug.Log((object) (character.Character.ToString() + " / " + (object) character.Fraction + " / " + (object) character.State));
      Debug.Log((object) "escaped characters: ");
      foreach (CombatServiceEscapedCharacterInfo escapedCharacter in combat.EscapedCharacters)
        Debug.Log((object) (escapedCharacter.EscapedCharacter.Character.ToString() + " / " + (object) escapedCharacter.FollowingCharacter.Character));
      Debug.Log((object) "/////////////////////");
    }

    private void SetWatch(CombatServiceCharacterInfo actor, CombatServiceCharacterInfo target)
    {
      if (actor.IsDead)
        return;
      this.currentCombats.Find((Predicate<CombatServiceCombatInfo>) (x => x.Characters.Contains(actor) || x.Characters.Contains(target)));
      actor.CurrentEnemy = target;
      actor.State = CombatServiceCharacterStateEnum.WatchFighting;
      BehaviorSubtreeUtility.SetCharacterSubtree(BehaviorSubtreeUtility.GetCharacterSubtree(actor.Character.gameObject), actor.CombatSettings.WatchFightingAI);
    }

    private void SetAttack(CombatServiceCharacterInfo actor, CombatServiceCharacterInfo target)
    {
      if (actor == null)
        Debug.LogError((object) "actor is null!");
      else if (target == null)
      {
        Debug.LogError((object) "target is null!");
      }
      else
      {
        if (actor.IsDead)
          return;
        CombatServiceCombatFractionInfo combatFractionInfo = this.currentCombats.Find((Predicate<CombatServiceCombatInfo>) (x => x.Characters.Contains(actor) || x.Characters.Contains(target))).Fractions.Find((Predicate<CombatServiceCombatFractionInfo>) (x => x.Characters.Contains(actor)));
        if (combatFractionInfo == null)
          Debug.LogError((object) "ownFraction == null");
        else if (combatFractionInfo.FearFractions == null)
          Debug.LogError((object) "ownFraction.FearFractions == null");
        else if (combatFractionInfo.FearFractions.Contains(target.Fraction))
        {
          this.SetEscape(actor, target);
        }
        else
        {
          actor.CurrentEnemy = target;
          actor.State = CombatServiceCharacterStateEnum.Fight;
          actor.FightStartPosition = actor.Character.transform.position;
          actor.LastGotHitPosition = actor.Character.transform.position;
          if ((UnityEngine.Object) actor.Character == (UnityEngine.Object) null)
            Debug.LogError((object) "actor character is null!");
          else
            BehaviorSubtreeUtility.SetCharacterSubtree(BehaviorSubtreeUtility.GetCharacterSubtree(actor.Character.gameObject), actor.CombatSettings.FightAI);
        }
      }
    }

    private void SetEscape(CombatServiceCharacterInfo actor, CombatServiceCharacterInfo target)
    {
      if (actor.IsDead)
        return;
      if (actor.IsIndoors)
      {
        this.SetSurrender(actor, target);
      }
      else
      {
        if (!actor.IsPlayer)
          actor.FireControllerCombatAction(CombatActionEnum.Surrender, target.Entity);
        actor.CurrentEnemy = target;
        actor.State = CombatServiceCharacterStateEnum.Escape;
        BehaviorSubtreeUtility.SetCharacterSubtree(BehaviorSubtreeUtility.GetCharacterSubtree(actor.Character.gameObject), actor.CombatSettings.EscapeAI);
      }
    }

    private void SetSurrender(CombatServiceCharacterInfo actor, CombatServiceCharacterInfo target)
    {
      if (actor.IsDead)
        return;
      if (!actor.IsPlayer && target != null)
        actor.FireControllerCombatAction(CombatActionEnum.Surrender, target.Entity);
      actor.CurrentEnemy = target;
      actor.State = CombatServiceCharacterStateEnum.Surrender;
      BehaviorSubtreeUtility.SetCharacterSubtree(BehaviorSubtreeUtility.GetCharacterSubtree(actor.Character.gameObject), actor.CombatSettings.SurrenderAI);
    }

    private void SetLoot(CombatServiceCharacterInfo actor, CombatServiceCharacterInfo target)
    {
      if (actor.IsDead)
        return;
      actor.CurrentEnemy = target;
      actor.State = CombatServiceCharacterStateEnum.Loot;
      BehaviorSubtreeUtility.SetCharacterSubtree(BehaviorSubtreeUtility.GetCharacterSubtree(actor.Character.gameObject), actor.CombatSettings.LootAI);
    }

    private void SetGoToPoint(CombatServiceCharacterInfo actor, Vector3 target)
    {
      if (actor.IsDead)
        return;
      actor.CurrentEnemy = (CombatServiceCharacterInfo) null;
      actor.State = CombatServiceCharacterStateEnum.GoToPoint;
      BehaviorTree characterSubtree = BehaviorSubtreeUtility.GetCharacterSubtree(actor.Character.gameObject);
      BehaviorSubtreeUtility.SetCharacterSubtree(characterSubtree, actor.CombatSettings.GoToPointAI);
      if (characterSubtree.GetVariable("Target") == null)
        return;
      characterSubtree.SetVariableValue("Target", (object) actor.FightStartPosition);
    }

    private void SetInactive(CombatServiceCharacterInfo actor)
    {
      if (actor == null || actor.IsPlayer)
        return;
      actor.CurrentEnemy = (CombatServiceCharacterInfo) null;
      actor.State = CombatServiceCharacterStateEnum.Free;
      actor.HearedCries.Clear();
      BehaviorSubtreeUtility.SetCharacterSubtree(BehaviorSubtreeUtility.GetCharacterSubtree(actor.Character.gameObject), (ExternalBehaviorTree) null);
    }

    public void CharacterCry(
      CombatServiceCharacterInfo character,
      CombatCryEnum cryType,
      CombatServiceCharacterInfo cryTarget = null,
      float timeout = 1f)
    {
      if (character != null && (UnityEngine.Object) character.Character != (UnityEngine.Object) null)
        character.Character.PlayLipSync(cryType);
      CombatCry cry = new CombatCry(cryType, character);
      cry.Timeout = timeout;
      cry.Radius = 0.0f;
      cry.CryTarget = cryTarget;
      if ((double) timeout == 0.0)
        this.ActCry(cry);
      else
        this.currentCries.Add(cry);
    }

    private void ActCry(CombatCry cry)
    {
      if (cry.Character == null || (UnityEngine.Object) cry.Character.Character == (UnityEngine.Object) null)
        return;
      List<CombatServiceCharacterInfo> hearingCharacters = this.FindHearingCharacters(cry.Character);
      if (hearingCharacters == null)
        return;
      foreach (CombatServiceCharacterInfo character in hearingCharacters)
      {
        if (!character.IsPlayer && !character.IsDead)
          this.HearCry(character, cry);
      }
    }

    private void UpdateCries()
    {
      CombatService.tmp.Clear();
      foreach (CombatCry currentCry in this.currentCries)
      {
        currentCry.Timeout -= Time.deltaTime;
        if ((double) currentCry.Timeout <= 0.0)
        {
          this.ActCry(currentCry);
          CombatService.tmp.Add(currentCry);
        }
      }
      foreach (CombatCry combatCry in CombatService.tmp)
        this.currentCries.Remove(combatCry);
    }

    private void UpdateWaitingOrders()
    {
      CombatService.tmp2.Clear();
      foreach (CombatServiceCommonOrder waitingAttackOrder in this.waitingAttackOrders)
      {
        CombatServiceCharacterInfo characterInfo1 = this.GetCharacterInfo(waitingAttackOrder.attacker);
        CombatServiceCharacterInfo characterInfo2 = this.GetCharacterInfo(waitingAttackOrder.enemy);
        if (characterInfo1 != null && characterInfo2 != null)
        {
          CombatService.tmp2.Add(waitingAttackOrder);
          characterInfo1.PersonalAttackEnemies.Add(characterInfo2);
          characterInfo1.RecountEnemies();
        }
      }
      foreach (CombatServiceCommonOrder serviceCommonOrder in CombatService.tmp2)
        this.waitingAttackOrders.Remove(serviceCommonOrder);
    }

    public CombatServiceCharacterInfo GetCharacterInfo(EnemyBase character)
    {
      foreach (CombatServiceCharacterInfo character1 in this.characters)
      {
        if (character1 != null && (UnityEngine.Object) character1.Character == (UnityEngine.Object) character)
          return character1;
      }
      return (CombatServiceCharacterInfo) null;
    }

    public CombatServiceCharacterInfo GetCharacterInfo(IEntity entity)
    {
      CombatServiceCharacterInfo characterInfo;
      if (this.charactersDictionary.TryGetValue(entity, out characterInfo))
        return characterInfo;
      foreach (CombatServiceCharacterInfo character in this.characters)
      {
        if (character != null && (UnityEngine.Object) character.Character != (UnityEngine.Object) null && character.Entity == entity)
          return character;
      }
      return (CombatServiceCharacterInfo) null;
    }

    public void ComputeUpdate()
    {
      if (!InstanceByRequest<EngineApplication>.Instance.IsPaused)
      {
        foreach (CombatServiceCharacterInfo character in this.characters)
          character.Update();
        double totalSeconds = ServiceLocator.GetService<TimeService>().RealTime.TotalSeconds;
        foreach (CombatServiceCombatInfo currentCombat in this.currentCombats)
        {
          bool flag = false;
          int index = 0;
          while (index < currentCombat.EscapedCharacters.Count)
          {
            CombatServiceEscapedCharacterInfo escapedCharacter = currentCombat.EscapedCharacters[index];
            if (totalSeconds <= escapedCharacter.Time + (double) this.escapedIgnoreTimeout && escapedCharacter.FollowingCharacter.CanHear(escapedCharacter.EscapedCharacter))
            {
              currentCombat.EscapedCharacters.RemoveAt(index);
              flag = true;
            }
            else
              ++index;
          }
          if (flag && !this.combatsToUpdate.Contains(currentCombat))
            this.combatsToUpdate.Add(currentCombat);
        }
      }
      if (this.combatsToUpdate.Count > 0)
      {
        foreach (CombatServiceCombatInfo combat in this.combatsToUpdate)
          this.UpdateCombatSituation(combat);
        this.combatsToUpdate.Clear();
        this.RecountPlayerDanger();
      }
      this.UpdateCries();
      this.UpdateWaitingOrders();
    }

    private void RecountPlayerDanger()
    {
      this.PlayerIsFighting = false;
      if (this.currentCombats == null)
        return;
      List<CombatServiceCombatInfo> all = this.currentCombats.FindAll((Predicate<CombatServiceCombatInfo>) (x => x.Characters != null && x.Characters.Exists((Predicate<CombatServiceCharacterInfo>) (y => y.IsPlayer))));
      if (all != null)
      {
        foreach (CombatServiceCombatInfo serviceCombatInfo in all)
        {
          if (serviceCombatInfo.Characters != null)
          {
            foreach (CombatServiceCharacterInfo character in serviceCombatInfo.Characters)
            {
              if (character.State == CombatServiceCharacterStateEnum.Fight && character.CurrentEnemy != null && character.CurrentEnemy.IsPlayer)
              {
                this.PlayerIsFighting = true;
                return;
              }
              if (character.Orders != null && character.Orders.Exists((Predicate<CombatServiceCharacterOrder>) (x => x.OrderType == CombatServiceCharacterOrderEnum.Attack && x.OrderTarget != null && x.OrderTarget.IsPlayer)))
              {
                this.PlayerIsFighting = true;
                return;
              }
            }
          }
        }
      }
    }

    private List<CombatServiceCharacterInfo> FindCharactersInRadius(
      CombatServiceCharacterInfo character,
      float radius)
    {
      if ((UnityEngine.Object) character.Character == (UnityEngine.Object) null || (UnityEngine.Object) character.Character.gameObject == (UnityEngine.Object) null)
        return (List<CombatServiceCharacterInfo>) null;
      Vector3 position = character.Character.gameObject.transform.position;
      List<CombatServiceCharacterInfo> charactersInRadius = new List<CombatServiceCharacterInfo>();
      foreach (CombatServiceCharacterInfo character1 in this.characters)
      {
        if (character1 != character && character1.IsIndoors == character.IsIndoors && !((UnityEngine.Object) character1.Character == (UnityEngine.Object) null) && !((UnityEngine.Object) character1.Character.gameObject == (UnityEngine.Object) null) && (double) (character1.Character.gameObject.transform.position - position).magnitude <= (double) radius)
          charactersInRadius.Add(character1);
      }
      return charactersInRadius;
    }

    private List<CombatServiceCharacterInfo> FindHearingCharacters(
      CombatServiceCharacterInfo character)
    {
      if ((UnityEngine.Object) character.Character == (UnityEngine.Object) null || (UnityEngine.Object) character.Character.gameObject == (UnityEngine.Object) null)
        return (List<CombatServiceCharacterInfo>) null;
      Vector3 position = character.Character.gameObject.transform.position;
      List<CombatServiceCharacterInfo> hearingCharacters = new List<CombatServiceCharacterInfo>();
      foreach (CombatServiceCharacterInfo character1 in this.characters)
      {
        if (character1 != character && character1.IsIndoors == character.IsIndoors && character1.CanHear(character))
          hearingCharacters.Add(character1);
      }
      return hearingCharacters;
    }

    private void FilterAimsByRadius(
      CombatServiceCharacterInfo character,
      List<CombatServiceCharacterInfo> aimsList,
      float radius)
    {
      if ((UnityEngine.Object) character.Character == (UnityEngine.Object) null || (UnityEngine.Object) character.Character.gameObject == (UnityEngine.Object) null)
        return;
      Vector3 position = character.Character.gameObject.transform.position;
      List<CombatServiceCharacterInfo> serviceCharacterInfoList = new List<CombatServiceCharacterInfo>();
      foreach (CombatServiceCharacterInfo aims in aimsList)
      {
        if (!((UnityEngine.Object) aims.Character == (UnityEngine.Object) null) && !((UnityEngine.Object) aims.Character.gameObject == (UnityEngine.Object) null) && (double) (aims.Character.gameObject.transform.position - position).magnitude <= (double) radius)
          serviceCharacterInfoList.Add(aims);
      }
      aimsList = serviceCharacterInfoList;
    }

    public CombatServiceCharacterInfo FindClosestAim(
      CombatServiceCharacterInfo character,
      List<CombatServiceCharacterInfo> aimsList)
    {
      if ((UnityEngine.Object) character.Character == (UnityEngine.Object) null || (UnityEngine.Object) character.Character.gameObject == (UnityEngine.Object) null)
        return (CombatServiceCharacterInfo) null;
      float? nullable1 = new float?();
      CombatServiceCharacterInfo closestAim = (CombatServiceCharacterInfo) null;
      Vector3 position = character.Character.gameObject.transform.position;
      foreach (CombatServiceCharacterInfo aims in aimsList)
      {
        if (!((UnityEngine.Object) aims.Character == (UnityEngine.Object) null) && !((UnityEngine.Object) aims.Character.gameObject == (UnityEngine.Object) null))
        {
          float magnitude = (aims.Character.gameObject.transform.position - position).magnitude;
          int num1;
          if (nullable1.HasValue)
          {
            float? nullable2 = nullable1;
            float num2 = magnitude;
            num1 = (double) nullable2.GetValueOrDefault() > (double) num2 & nullable2.HasValue ? 1 : 0;
          }
          else
            num1 = 1;
          if (num1 != 0)
          {
            nullable1 = new float?(magnitude);
            closestAim = aims;
          }
        }
      }
      return closestAim;
    }

    public bool CharacterIsInCombat(EnemyBase character)
    {
      CombatServiceCharacterInfo actor = this.GetCharacterInfo(character);
      return actor != null && this.currentCombats.FindAll((Predicate<CombatServiceCombatInfo>) (x => x.Characters.Contains(actor))).Count != 0;
    }

    public List<CombatServiceCombatInfo> GetCharacterCombats(CombatServiceCharacterInfo character)
    {
      return this.currentCombats.FindAll((Predicate<CombatServiceCombatInfo>) (x => x.Characters.Contains(character)));
    }

    public bool CharacterIsActivated(CombatServiceCharacterInfo character)
    {
      return this.activeCharacters.Contains(character);
    }
  }
}
