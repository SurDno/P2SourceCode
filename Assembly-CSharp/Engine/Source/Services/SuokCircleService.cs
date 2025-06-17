using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using BehaviorDesigner.Runtime;
using Cofe.Serializations.Data;
using Engine.Behaviours.Engines.Services;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Common.Components.AttackerPlayer;
using Engine.Common.Components.Parameters;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Saves;
using Engine.Source.Services.Saves;
using FlowCanvas;
using Inspectors;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Engine.Source.Services
{
  [RuntimeService(typeof (SuokCircleService))]
  public class SuokCircleService : 
    IInitialisable,
    IUpdatable,
    ISavesController,
    IChangeParameterListener,
    IEntityEventsListener
  {
    private const int punchesToAvoid = 4;
    private const float yourDamageIsLowTime = 7.5f;
    private const float ruinBlockTime = 5f;
    [Inspected]
    private List<GameObject> suokCombatants = [];
    [Inspected]
    private List<GameObject> suokSpectators = [];
    [Inspected]
    private SuokCircleTutorialStateEnum state;
    private FlowScriptController currentTooltipBlueprint;
    private FlowScriptController voicesBlueprint;
    private IEntity playerEntity;
    private GameObject playerView;
    private ParametersComponent playerParameters;
    private ParametersComponent enemyParameters;
    private NPCEnemy enemyNpc;
    private PlayerEnemy playerNpc;
    private IEntity enemyEntity;
    private GameObject enemyView;
    private float timeLeft;
    private int punchCount;
    private float currentStamina;

    public event Action OnStateChangedEvent;

    public float CurrentStamina => currentStamina;

    public SuokCircleTutorialStateEnum GetState() => state;

    private void SetState(SuokCircleTutorialStateEnum state)
    {
      Debug.Log(string.Format("SetState {0}", state));
      playerParameters.GetByName<float>(ParameterNameEnum.Stamina);
      IParameter<float> byName1 = playerParameters.GetByName<float>(ParameterNameEnum.Health);
      playerParameters.GetByName<float>(ParameterNameEnum.Thirst);
      IParameter<float> byName2 = enemyParameters.GetByName<float>(ParameterNameEnum.Health);
      BehaviorTree characterSubtree = BehaviorSubtreeUtility.GetCharacterSubtree(enemyView);
      Action handlerOnBeginTalking;
      switch (state)
      {
        case SuokCircleTutorialStateEnum.WaitForInitialDialogEnd:
          currentStamina = 1f;
          voicesBlueprint = BlueprintServiceUtility.Start(ScriptableObjectInstance<IntroData>.Instance.SuokVoicesBlueprint, null, null, typeof (SuokCircleService).Name);
          ISpeakingComponent speaking1 = enemyEntity.GetComponent<ISpeakingComponent>();
          handlerOnBeginTalking = null;
          handlerOnBeginTalking = (Action) (() =>
          {
            SetState(SuokCircleTutorialStateEnum.WaitForUnholster);
            speaking1.OnBeginTalking -= handlerOnBeginTalking;
          });
          speaking1.OnBeginTalking += handlerOnBeginTalking;
          BehaviorSubtreeUtility.SetCharacterSubtree(characterSubtree, ScriptableObjectInstance<IntroData>.Instance.SuokSubAI_WaitForUnholster);
          break;
        case SuokCircleTutorialStateEnum.WaitForUnholster:
          currentTooltipBlueprint = BlueprintServiceUtility.Start(ScriptableObjectInstance<IntroData>.Instance.SuokNotificationBlueprint, null, null, typeof (SuokCircleService).Name);
          IAttackerPlayerComponent component = playerEntity.GetComponent<IAttackerPlayerComponent>();
          component.HandsHolster();
          currentTooltipBlueprint.SendEvent("WaitForUnholster");
          BehaviorSubtreeUtility.SetCharacterSubtree(characterSubtree, ScriptableObjectInstance<IntroData>.Instance.SuokSubAI_WaitForUnholster);
          component.WeaponUnholsterEndEvent += Attacker_WeaponUnholsterEndEvent;
          break;
        case SuokCircleTutorialStateEnum.WaitForPunch:
          playerEntity.GetComponent<IAttackerPlayerComponent>().WeaponUnholsterEndEvent -= Attacker_WeaponUnholsterEndEvent;
          voicesBlueprint?.SendEvent("Stop");
          currentStamina = 1f;
          currentTooltipBlueprint?.SendEvent("WaitForPunch");
          BehaviorSubtreeUtility.SetCharacterSubtree(characterSubtree, ScriptableObjectInstance<IntroData>.Instance.SuokSubAI_WaitForPunch);
          enemyNpc.WasPrepunchEvent += EnemyNpc_WasPrepunch;
          break;
        case SuokCircleTutorialStateEnum.WaitForUppercut:
          enemyNpc.WasPrepunchEvent -= EnemyNpc_WasPrepunch;
          currentTooltipBlueprint?.SendEvent("WaitForUppercut");
          BehaviorSubtreeUtility.SetCharacterSubtree(characterSubtree, ScriptableObjectInstance<IntroData>.Instance.SuokSubAI_WaitForUppercut);
          enemyNpc.WasStaggeredEvent += EnemyNpc_WasStaggeredEvent;
          break;
        case SuokCircleTutorialStateEnum.WaitForLowStamina:
          currentStamina = 1f;
          enemyNpc.WasStaggeredEvent -= EnemyNpc_WasStaggeredEvent;
          punchCount = 0;
          timeLeft = 7.5f;
          currentTooltipBlueprint?.SendEvent("WaitForLowStamina");
          BehaviorSubtreeUtility.SetCharacterSubtree(characterSubtree, ScriptableObjectInstance<IntroData>.Instance.SuokSubAI_WaitForLowStamina);
          enemyNpc.WasPunchedToBlockEvent += EnemyNpc_WasPuncheDone;
          enemyNpc.WasPunchedToQuickBlock += EnemyNpc_WasPuncheDone;
          enemyNpc.WasPunchedToDodgeEvent += EnemyNpc_WasPuncheDone;
          enemyNpc.WasPunchedEvent += EnemyNpc_WasPuncheDone;
          enemyNpc.WasStaggeredEvent += EnemyNpc_WasPuncheDone;
          enemyNpc.WasPunchedToStaggerEvent += EnemyNpc_WasPuncheDone;
          playerNpc.PunchEvent += PlayerNpc_PunchFired;
          break;
        case SuokCircleTutorialStateEnum.YourDamageIsLow:
          enemyNpc.WasPunchedToBlockEvent -= EnemyNpc_WasPuncheDone;
          enemyNpc.WasPunchedToQuickBlock -= EnemyNpc_WasPuncheDone;
          enemyNpc.WasPunchedToDodgeEvent -= EnemyNpc_WasPuncheDone;
          enemyNpc.WasPunchedEvent -= EnemyNpc_WasPuncheDone;
          enemyNpc.WasStaggeredEvent -= EnemyNpc_WasPuncheDone;
          enemyNpc.WasPunchedToStaggerEvent -= EnemyNpc_WasPuncheDone;
          playerNpc.PunchEvent -= PlayerNpc_PunchFired;
          timeLeft = 7.5f;
          punchCount = 0;
          currentTooltipBlueprint?.SendEvent("YourDamageIsLow");
          BehaviorSubtreeUtility.SetCharacterSubtree(characterSubtree, ScriptableObjectInstance<IntroData>.Instance.SuokSubAI_WaitForLowStamina);
          playerView.GetComponent<PlayerWeaponServiceNew>().WeaponShootEvent += PlayerWeaponService_WeaponShootEvent;
          break;
        case SuokCircleTutorialStateEnum.BlockExample:
          playerView.GetComponent<PlayerWeaponServiceNew>().WeaponShootEvent -= PlayerWeaponService_WeaponShootEvent;
          currentTooltipBlueprint?.SendEvent("BlockExample");
          BehaviorSubtreeUtility.SetCharacterSubtree(characterSubtree, ScriptableObjectInstance<IntroData>.Instance.SuokSubAI_BlockExample);
          punchCount = 0;
          playerNpc.WasPunchedToBlockEvent += PlayerNpc_WasPuncheToBlockDone;
          break;
        case SuokCircleTutorialStateEnum.RuinBlockExample:
          playerNpc.WasPunchedToBlockEvent -= PlayerNpc_WasPuncheToBlockDone;
          BehaviorSubtreeUtility.SetCharacterSubtree(characterSubtree, ScriptableObjectInstance<IntroData>.Instance.SuokSubAI_RuinBlockExample);
          punchCount = 0;
          playerNpc.WasStaggeredEvent += PlayerNpc_WasStaggered;
          playerNpc.WasPunchedToStaggerEvent += PlayerNpc_WasStaggered;
          timeLeft = 5f;
          break;
        case SuokCircleTutorialStateEnum.StaminaGrowsFasterInBlock:
          playerNpc.WasStaggeredEvent -= PlayerNpc_WasStaggered;
          playerNpc.WasPunchedToStaggerEvent -= PlayerNpc_WasStaggered;
          BehaviorSubtreeUtility.SetCharacterSubtree(characterSubtree, ScriptableObjectInstance<IntroData>.Instance.SuokSubAI_WaitForStaminaGrows);
          currentTooltipBlueprint.SendEvent("StaminaGrowsFasterInBlock");
          break;
        case SuokCircleTutorialStateEnum.WaitForFreeFight:
          currentTooltipBlueprint?.SendEvent("FreeFight");
          BehaviorSubtreeUtility.SetCharacterSubtree(characterSubtree, ScriptableObjectInstance<IntroData>.Instance.SuokSubAI_WaitForFreeFight);
          enemyNpc.WasPunchedEvent += EnemyNpc_WasAttacked;
          enemyNpc.WasPunchedToBlockEvent += EnemyNpc_WasAttacked;
          enemyNpc.WasStaggeredEvent += EnemyNpc_WasAttacked;
          enemyNpc.WasPunchedToDodgeEvent += EnemyNpc_WasAttacked;
          enemyNpc.WasPunchedToQuickBlock += EnemyNpc_WasAttacked;
          break;
        case SuokCircleTutorialStateEnum.FreeFight:
          enemyNpc.WasPunchedEvent -= EnemyNpc_WasAttacked;
          enemyNpc.WasPunchedToBlockEvent -= EnemyNpc_WasAttacked;
          enemyNpc.WasStaggeredEvent -= EnemyNpc_WasAttacked;
          enemyNpc.WasPunchedToDodgeEvent -= EnemyNpc_WasAttacked;
          enemyNpc.WasPunchedToQuickBlock -= EnemyNpc_WasAttacked;
          BehaviorSubtreeUtility.SetCharacterSubtree(characterSubtree, ScriptableObjectInstance<IntroData>.Instance.SuokSubAI_FreeFight);
          byName1.AddListener(this);
          byName2.AddListener(this);
          break;
        case SuokCircleTutorialStateEnum.YouWin:
          byName1.RemoveListener(this);
          byName2.RemoveListener(this);
          currentTooltipBlueprint?.SendEvent("EnemyCanSurrender");
          currentTooltipBlueprint?.SendEvent("Finished");
          BehaviorSubtreeUtility.SetCharacterSubtree(characterSubtree, ScriptableObjectInstance<IntroData>.Instance.SuokSubAI_FinishedYouWin);
          ISpeakingComponent speaking = enemyEntity.GetComponent<ISpeakingComponent>();
          Action handlerOnBeginTalking1 = null;
          handlerOnBeginTalking = (Action) (() =>
          {
            SetState(SuokCircleTutorialStateEnum.AfterWinTalk);
            speaking.OnBeginTalking -= handlerOnBeginTalking1;
          });
          speaking.OnBeginTalking += handlerOnBeginTalking1;
          punchCount = 0;
          enemyNpc.WasPunchedEvent += EnemyHealth_ChangeValueEvent_Ragdoll;
          enemyNpc.WasPunchedToDodgeEvent += EnemyHealth_ChangeValueEvent_Ragdoll;
          enemyNpc.WasPunchedToQuickBlock += EnemyHealth_ChangeValueEvent_Ragdoll;
          enemyNpc.WasPunchedToSurrenderEvent += EnemyHealth_ChangeValueEvent_Ragdoll;
          enemyNpc.WasStaggeredEvent += EnemyHealth_ChangeValueEvent_Ragdoll;
          enemyNpc.WasPunchedToStaggerEvent += EnemyHealth_ChangeValueEvent_Ragdoll;
          break;
        case SuokCircleTutorialStateEnum.YouLoose:
          byName1.RemoveListener(this);
          byName2.RemoveListener(this);
          currentTooltipBlueprint?.SendEvent("FinishedLose");
          BehaviorSubtreeUtility.SetCharacterSubtree(characterSubtree, ScriptableObjectInstance<IntroData>.Instance.SuokSubAI_FinishedYouLoose);
          break;
        case SuokCircleTutorialStateEnum.Ragdoll:
          enemyNpc.WasPunchedEvent += EnemyHealth_ChangeValueEvent_Ragdoll;
          enemyNpc.WasPunchedToDodgeEvent += EnemyHealth_ChangeValueEvent_Ragdoll;
          enemyNpc.WasPunchedToQuickBlock += EnemyHealth_ChangeValueEvent_Ragdoll;
          enemyNpc.WasPunchedToSurrenderEvent += EnemyHealth_ChangeValueEvent_Ragdoll;
          enemyNpc.WasStaggeredEvent += EnemyHealth_ChangeValueEvent_Ragdoll;
          enemyNpc.WasPunchedToStaggerEvent += EnemyHealth_ChangeValueEvent_Ragdoll;
          BehaviorSubtreeUtility.SetCharacterSubtree(BehaviorSubtreeUtility.GetCharacterSubtree(enemyView), ScriptableObjectInstance<IntroData>.Instance.SuokSubAI_Ragdoll);
          FlowScriptController tooltipBlueprint = currentTooltipBlueprint;
          if (tooltipBlueprint != null)
          {
            tooltipBlueprint.SendEvent("YouKilledSurrender");
          }
          break;
      }
      this.state = state;
      Action stateChangedEvent = OnStateChangedEvent;
      if (stateChangedEvent == null)
        return;
      stateChangedEvent();
    }

    private void EnemyHealth_ChangeValueEvent_Ragdoll(EnemyBase enemy)
    {
      Debug.Log(nameof (EnemyHealth_ChangeValueEvent_Ragdoll));
      ++punchCount;
      if (punchCount != 4)
        return;
      Debug.Log("EnemyHealth_ChangeValueEvent_Ragdoll 4");
      SetState(SuokCircleTutorialStateEnum.Ragdoll);
    }

    private void EnemyNpc_WasAttacked(EnemyBase enemy)
    {
      Debug.Log(nameof (EnemyNpc_WasAttacked));
      SetState(SuokCircleTutorialStateEnum.FreeFight);
    }

    private void EnemyNpc_WasStaggeredEvent(EnemyBase enemy)
    {
      Debug.Log(nameof (EnemyNpc_WasStaggeredEvent));
      SetState(SuokCircleTutorialStateEnum.WaitForLowStamina);
    }

    private void EnemyNpc_WasPrepunch(EnemyBase enemy)
    {
      Debug.Log(nameof (EnemyNpc_WasPrepunch));
      ++punchCount;
      if (4 != punchCount)
        return;
      Debug.Log(string.Format("EnemyNpc_WasPrepunch {0}", punchCount));
      SetState(SuokCircleTutorialStateEnum.WaitForUppercut);
    }

    private void PlayerNpc_PunchFired(
      IEntity entity,
      ShotType shotType,
      ReactionType reactionType,
      WeaponEnum weaponEnum,
      ShotSubtypeEnum shotSubtypeEnum)
    {
      Debug.Log(nameof (PlayerNpc_PunchFired));
      if (shotType == ShotType.LowStamina)
      {
        Debug.Log("PlayerNpc_PunchFired low stamina");
        SetState(SuokCircleTutorialStateEnum.YourDamageIsLow);
      }
      else
      {
        Debug.Log("PlayerNpc_PunchFired other");
        ++punchCount;
        currentStamina -= 0.2f;
      }
    }

    private void EnemyNpc_WasPuncheDone(EnemyBase enemy)
    {
      Debug.Log(nameof (EnemyNpc_WasPuncheDone));
      ++punchCount;
    }

    private void PlayerWeaponService_WeaponShootEvent(
      WeaponKind arg1,
      IEntity arg2,
      ShotType arg3,
      ReactionType arg4,
      ShotSubtypeEnum arg5)
    {
      Debug.Log(nameof (PlayerWeaponService_WeaponShootEvent));
      ++punchCount;
      if (punchCount != 1)
        return;
      Debug.Log("PlayerWeaponService_WeaponShootEvent 1");
      SetState(SuokCircleTutorialStateEnum.BlockExample);
    }

    private void PlayerNpc_WasPuncheToBlockDone(EnemyBase enemy)
    {
      Debug.Log(nameof (PlayerNpc_WasPuncheToBlockDone));
      ++punchCount;
      if (punchCount != 2)
        return;
      Debug.Log("PlayerNpc_WasPuncheToBlockDone 2");
      SetState(SuokCircleTutorialStateEnum.RuinBlockExample);
    }

    private void PlayerNpc_WasStaggered(EnemyBase enemy)
    {
      Debug.Log(string.Format("PlayerNpc_WasStaggered {0}", punchCount));
      ++punchCount;
      timeLeft = 5f;
      if (punchCount == 1)
      {
        Debug.Log(string.Format("PlayerNpc_WasStaggered 1 {0}", punchCount));
        currentTooltipBlueprint?.SendEvent("RuinBlockExample");
      }
      else
      {
        if (punchCount != 2)
          return;
        Debug.Log(string.Format("PlayerNpc_WasStaggered 2 {0}", punchCount));
        BehaviorSubtreeUtility.SetCharacterSubtree(BehaviorSubtreeUtility.GetCharacterSubtree(enemyView), ScriptableObjectInstance<IntroData>.Instance.SuokSubAI_WaitForStaminaGrows);
      }
    }

    private void Attacker_WeaponUnholsterEndEvent(WeaponKind weaponKind)
    {
      Debug.Log(nameof (Attacker_WeaponUnholsterEndEvent));
      SetState(SuokCircleTutorialStateEnum.WaitForPunch);
    }

    public void Initialise()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable(this);
    }

    public void Terminate()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable(this);
    }

    public void RegisterCombatant(GameObject character)
    {
      if (suokCombatants.Contains(character))
        Debug.LogError(typeof (SuokCircleService).Name + " - registering combatant already in list");
      else
        suokCombatants.Add(character);
    }

    public void UnregisterCombatant(GameObject character)
    {
      if (!suokCombatants.Remove(character))
        Debug.LogError(typeof (SuokCircleService).Name + " - unregistering combatant not in list");
      BehaviorSubtreeUtility.SetCharacterSubtree(BehaviorSubtreeUtility.GetCharacterSubtree(character), null);
    }

    public void RegisterSpectator(GameObject character)
    {
      if (suokSpectators.Contains(character))
        Debug.LogError(typeof (SuokCircleService).Name + " - registering spectator already in list");
      else
        suokSpectators.Add(character);
    }

    public void UnregisterSpectator(GameObject character)
    {
      if (!suokSpectators.Remove(character))
        Debug.LogError(typeof (SuokCircleService).Name + " - unregistering spectator not in list");
      BehaviorSubtreeUtility.SetCharacterSubtree(BehaviorSubtreeUtility.GetCharacterSubtree(character), null);
    }

    public void ComputeUpdate()
    {
      switch (state)
      {
        case SuokCircleTutorialStateEnum.Unknown:
          if (suokCombatants.Count != 1)
            break;
          playerEntity = ServiceLocator.GetService<ISimulation>().Player;
          playerView = ((IEntityView) playerEntity).GameObject;
          playerNpc = playerView.GetComponent<PlayerEnemy>();
          if (playerEntity != null && playerView != null)
          {
            playerParameters = playerEntity.GetComponent<ParametersComponent>();
            enemyView = suokCombatants[0];
            enemyEntity = EntityUtility.GetEntity(enemyView);
            enemyNpc = enemyView.GetComponent<NPCEnemy>();
            ((Entity) enemyEntity).AddListener(this);
            enemyParameters = enemyEntity.GetComponent<ParametersComponent>();
            SetState(SuokCircleTutorialStateEnum.WaitForInitialDialogEnd);
          }
          break;
        case SuokCircleTutorialStateEnum.WaitForLowStamina:
          if (punchCount < 1)
            break;
          timeLeft -= Time.deltaTime;
          if (timeLeft < 0.0)
            SetState(SuokCircleTutorialStateEnum.YourDamageIsLow);
          break;
        case SuokCircleTutorialStateEnum.YourDamageIsLow:
          timeLeft -= Time.deltaTime;
          if (timeLeft >= 0.0)
            break;
          Debug.Log("From update: SetState(SuokCircleTutorialStateEnum.YourDamageIsLow)");
          SetState(SuokCircleTutorialStateEnum.BlockExample);
          break;
        case SuokCircleTutorialStateEnum.RuinBlockExample:
          if (enemyNpc.IsAttacking || playerNpc.IsStagger)
            timeLeft = Mathf.Max(timeLeft, 1f);
          if (punchCount <= 0)
            break;
          timeLeft -= Time.deltaTime;
          if (timeLeft < 0.0)
          {
            Debug.Log("From update: SetState(SuokCircleTutorialStateEnum.StaminaGrowsFasterInBlock)");
            SetState(SuokCircleTutorialStateEnum.StaminaGrowsFasterInBlock);
          }
          break;
        case SuokCircleTutorialStateEnum.StaminaGrowsFasterInBlock:
          if (playerParameters.GetByName<float>(ParameterNameEnum.Stamina).Value <= 0.89999997615814209)
            break;
          Debug.Log("From update: SetState(SuokCircleTutorialStateEnum.WaitForFreeFight)");
          SetState(SuokCircleTutorialStateEnum.WaitForFreeFight);
          break;
      }
    }

    IEnumerator ISavesController.Load(IErrorLoadingHandler errorHandler)
    {
      yield break;
    }

    IEnumerator ISavesController.Load(
      XmlElement element,
      string context,
      IErrorLoadingHandler errorHandler)
    {
      yield break;
    }

    void ISavesController.Unload()
    {
      punchCount = 0;
      state = SuokCircleTutorialStateEnum.Unknown;
    }

    void ISavesController.Save(IDataWriter element, string context)
    {
    }

    public void OnParameterChanged(IParameter parameter)
    {
      IParameter<float> byName1 = playerParameters.GetByName<float>(ParameterNameEnum.Health);
      IParameter<float> byName2 = enemyParameters.GetByName<float>(ParameterNameEnum.Health);
      if (byName1 == parameter)
      {
        Debug.Log("Health_ChangeValueEvent");
        if (((IParameter<float>) parameter).Value >= 0.10000000149011612)
          return;
        Debug.Log("Health_ChangeValueEvent < 0.1");
        SetState(SuokCircleTutorialStateEnum.YouLoose);
      }
      else if (byName2 == parameter)
      {
        Debug.Log("EnemyHealth_ChangeValueEvent");
        if (((IParameter<float>) parameter).Value >= 0.10000000149011612)
          return;
        Debug.Log("EnemyHealth_ChangeValueEvent < 0.1");
        SetState(SuokCircleTutorialStateEnum.YouWin);
      }
      else
        Debug.LogError("!!! Такого быть не должно");
    }

    public void OnEntityEvent(IEntity sender, EntityEvents kind)
    {
      if (kind != EntityEvents.DisposeEvent)
        return;
      ServiceLocator.GetService<NotificationService>().RemoveNotify(NotificationEnum.Tooltip);
      if (currentTooltipBlueprint != null)
        Object.Destroy(currentTooltipBlueprint.gameObject);
      if (voicesBlueprint != null)
        Object.Destroy(voicesBlueprint.gameObject);
      ((Entity) sender).RemoveListener(this);
    }
  }
}
