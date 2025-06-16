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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace Engine.Source.Services
{
  [RuntimeService(new System.Type[] {typeof (SuokCircleService)})]
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
    private List<GameObject> suokCombatants = new List<GameObject>();
    [Inspected]
    private List<GameObject> suokSpectators = new List<GameObject>();
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
    private int punchCount = 0;
    private float currentStamina;

    public event Action OnStateChangedEvent;

    public float CurrentStamina => this.currentStamina;

    public SuokCircleTutorialStateEnum GetState() => this.state;

    private void SetState(SuokCircleTutorialStateEnum state)
    {
      Debug.Log((object) string.Format("SetState {0}", (object) state));
      this.playerParameters.GetByName<float>(ParameterNameEnum.Stamina);
      IParameter<float> byName1 = this.playerParameters.GetByName<float>(ParameterNameEnum.Health);
      this.playerParameters.GetByName<float>(ParameterNameEnum.Thirst);
      IParameter<float> byName2 = this.enemyParameters.GetByName<float>(ParameterNameEnum.Health);
      BehaviorTree characterSubtree = BehaviorSubtreeUtility.GetCharacterSubtree(this.enemyView);
      Action handlerOnBeginTalking;
      switch (state)
      {
        case SuokCircleTutorialStateEnum.WaitForInitialDialogEnd:
          this.currentStamina = 1f;
          this.voicesBlueprint = BlueprintServiceUtility.Start(ScriptableObjectInstance<IntroData>.Instance.SuokVoicesBlueprint, (IEntity) null, (Action) null, typeof (SuokCircleService).Name);
          ISpeakingComponent speaking1 = this.enemyEntity.GetComponent<ISpeakingComponent>();
          handlerOnBeginTalking = (Action) null;
          handlerOnBeginTalking = (Action) (() =>
          {
            this.SetState(SuokCircleTutorialStateEnum.WaitForUnholster);
            speaking1.OnBeginTalking -= handlerOnBeginTalking;
          });
          speaking1.OnBeginTalking += handlerOnBeginTalking;
          BehaviorSubtreeUtility.SetCharacterSubtree(characterSubtree, ScriptableObjectInstance<IntroData>.Instance.SuokSubAI_WaitForUnholster);
          break;
        case SuokCircleTutorialStateEnum.WaitForUnholster:
          this.currentTooltipBlueprint = BlueprintServiceUtility.Start(ScriptableObjectInstance<IntroData>.Instance.SuokNotificationBlueprint, (IEntity) null, (Action) null, typeof (SuokCircleService).Name);
          IAttackerPlayerComponent component = this.playerEntity.GetComponent<IAttackerPlayerComponent>();
          component.HandsHolster();
          this.currentTooltipBlueprint.SendEvent("WaitForUnholster");
          BehaviorSubtreeUtility.SetCharacterSubtree(characterSubtree, ScriptableObjectInstance<IntroData>.Instance.SuokSubAI_WaitForUnholster);
          component.WeaponUnholsterEndEvent += new Action<WeaponKind>(this.Attacker_WeaponUnholsterEndEvent);
          break;
        case SuokCircleTutorialStateEnum.WaitForPunch:
          this.playerEntity.GetComponent<IAttackerPlayerComponent>().WeaponUnholsterEndEvent -= new Action<WeaponKind>(this.Attacker_WeaponUnholsterEndEvent);
          this.voicesBlueprint?.SendEvent("Stop");
          this.currentStamina = 1f;
          this.currentTooltipBlueprint?.SendEvent("WaitForPunch");
          BehaviorSubtreeUtility.SetCharacterSubtree(characterSubtree, ScriptableObjectInstance<IntroData>.Instance.SuokSubAI_WaitForPunch);
          this.enemyNpc.WasPrepunchEvent += new Action<EnemyBase>(this.EnemyNpc_WasPrepunch);
          break;
        case SuokCircleTutorialStateEnum.WaitForUppercut:
          this.enemyNpc.WasPrepunchEvent -= new Action<EnemyBase>(this.EnemyNpc_WasPrepunch);
          this.currentTooltipBlueprint?.SendEvent("WaitForUppercut");
          BehaviorSubtreeUtility.SetCharacterSubtree(characterSubtree, ScriptableObjectInstance<IntroData>.Instance.SuokSubAI_WaitForUppercut);
          this.enemyNpc.WasStaggeredEvent += new Action<EnemyBase>(this.EnemyNpc_WasStaggeredEvent);
          break;
        case SuokCircleTutorialStateEnum.WaitForLowStamina:
          this.currentStamina = 1f;
          this.enemyNpc.WasStaggeredEvent -= new Action<EnemyBase>(this.EnemyNpc_WasStaggeredEvent);
          this.punchCount = 0;
          this.timeLeft = 7.5f;
          this.currentTooltipBlueprint?.SendEvent("WaitForLowStamina");
          BehaviorSubtreeUtility.SetCharacterSubtree(characterSubtree, ScriptableObjectInstance<IntroData>.Instance.SuokSubAI_WaitForLowStamina);
          this.enemyNpc.WasPunchedToBlockEvent += new Action<EnemyBase>(this.EnemyNpc_WasPuncheDone);
          this.enemyNpc.WasPunchedToQuickBlock += new Action<EnemyBase>(this.EnemyNpc_WasPuncheDone);
          this.enemyNpc.WasPunchedToDodgeEvent += new Action<EnemyBase>(this.EnemyNpc_WasPuncheDone);
          this.enemyNpc.WasPunchedEvent += new Action<EnemyBase>(this.EnemyNpc_WasPuncheDone);
          this.enemyNpc.WasStaggeredEvent += new Action<EnemyBase>(this.EnemyNpc_WasPuncheDone);
          this.enemyNpc.WasPunchedToStaggerEvent += new Action<EnemyBase>(this.EnemyNpc_WasPuncheDone);
          this.playerNpc.PunchEvent += new Action<IEntity, ShotType, ReactionType, WeaponEnum, ShotSubtypeEnum>(this.PlayerNpc_PunchFired);
          break;
        case SuokCircleTutorialStateEnum.YourDamageIsLow:
          this.enemyNpc.WasPunchedToBlockEvent -= new Action<EnemyBase>(this.EnemyNpc_WasPuncheDone);
          this.enemyNpc.WasPunchedToQuickBlock -= new Action<EnemyBase>(this.EnemyNpc_WasPuncheDone);
          this.enemyNpc.WasPunchedToDodgeEvent -= new Action<EnemyBase>(this.EnemyNpc_WasPuncheDone);
          this.enemyNpc.WasPunchedEvent -= new Action<EnemyBase>(this.EnemyNpc_WasPuncheDone);
          this.enemyNpc.WasStaggeredEvent -= new Action<EnemyBase>(this.EnemyNpc_WasPuncheDone);
          this.enemyNpc.WasPunchedToStaggerEvent -= new Action<EnemyBase>(this.EnemyNpc_WasPuncheDone);
          this.playerNpc.PunchEvent -= new Action<IEntity, ShotType, ReactionType, WeaponEnum, ShotSubtypeEnum>(this.PlayerNpc_PunchFired);
          this.timeLeft = 7.5f;
          this.punchCount = 0;
          this.currentTooltipBlueprint?.SendEvent("YourDamageIsLow");
          BehaviorSubtreeUtility.SetCharacterSubtree(characterSubtree, ScriptableObjectInstance<IntroData>.Instance.SuokSubAI_WaitForLowStamina);
          this.playerView.GetComponent<PlayerWeaponServiceNew>().WeaponShootEvent += new Action<WeaponKind, IEntity, ShotType, ReactionType, ShotSubtypeEnum>(this.PlayerWeaponService_WeaponShootEvent);
          break;
        case SuokCircleTutorialStateEnum.BlockExample:
          this.playerView.GetComponent<PlayerWeaponServiceNew>().WeaponShootEvent -= new Action<WeaponKind, IEntity, ShotType, ReactionType, ShotSubtypeEnum>(this.PlayerWeaponService_WeaponShootEvent);
          this.currentTooltipBlueprint?.SendEvent("BlockExample");
          BehaviorSubtreeUtility.SetCharacterSubtree(characterSubtree, ScriptableObjectInstance<IntroData>.Instance.SuokSubAI_BlockExample);
          this.punchCount = 0;
          this.playerNpc.WasPunchedToBlockEvent += new Action<EnemyBase>(this.PlayerNpc_WasPuncheToBlockDone);
          break;
        case SuokCircleTutorialStateEnum.RuinBlockExample:
          this.playerNpc.WasPunchedToBlockEvent -= new Action<EnemyBase>(this.PlayerNpc_WasPuncheToBlockDone);
          BehaviorSubtreeUtility.SetCharacterSubtree(characterSubtree, ScriptableObjectInstance<IntroData>.Instance.SuokSubAI_RuinBlockExample);
          this.punchCount = 0;
          this.playerNpc.WasStaggeredEvent += new Action<EnemyBase>(this.PlayerNpc_WasStaggered);
          this.playerNpc.WasPunchedToStaggerEvent += new Action<EnemyBase>(this.PlayerNpc_WasStaggered);
          this.timeLeft = 5f;
          break;
        case SuokCircleTutorialStateEnum.StaminaGrowsFasterInBlock:
          this.playerNpc.WasStaggeredEvent -= new Action<EnemyBase>(this.PlayerNpc_WasStaggered);
          this.playerNpc.WasPunchedToStaggerEvent -= new Action<EnemyBase>(this.PlayerNpc_WasStaggered);
          BehaviorSubtreeUtility.SetCharacterSubtree(characterSubtree, ScriptableObjectInstance<IntroData>.Instance.SuokSubAI_WaitForStaminaGrows);
          this.currentTooltipBlueprint.SendEvent("StaminaGrowsFasterInBlock");
          break;
        case SuokCircleTutorialStateEnum.WaitForFreeFight:
          this.currentTooltipBlueprint?.SendEvent("FreeFight");
          BehaviorSubtreeUtility.SetCharacterSubtree(characterSubtree, ScriptableObjectInstance<IntroData>.Instance.SuokSubAI_WaitForFreeFight);
          this.enemyNpc.WasPunchedEvent += new Action<EnemyBase>(this.EnemyNpc_WasAttacked);
          this.enemyNpc.WasPunchedToBlockEvent += new Action<EnemyBase>(this.EnemyNpc_WasAttacked);
          this.enemyNpc.WasStaggeredEvent += new Action<EnemyBase>(this.EnemyNpc_WasAttacked);
          this.enemyNpc.WasPunchedToDodgeEvent += new Action<EnemyBase>(this.EnemyNpc_WasAttacked);
          this.enemyNpc.WasPunchedToQuickBlock += new Action<EnemyBase>(this.EnemyNpc_WasAttacked);
          break;
        case SuokCircleTutorialStateEnum.FreeFight:
          this.enemyNpc.WasPunchedEvent -= new Action<EnemyBase>(this.EnemyNpc_WasAttacked);
          this.enemyNpc.WasPunchedToBlockEvent -= new Action<EnemyBase>(this.EnemyNpc_WasAttacked);
          this.enemyNpc.WasStaggeredEvent -= new Action<EnemyBase>(this.EnemyNpc_WasAttacked);
          this.enemyNpc.WasPunchedToDodgeEvent -= new Action<EnemyBase>(this.EnemyNpc_WasAttacked);
          this.enemyNpc.WasPunchedToQuickBlock -= new Action<EnemyBase>(this.EnemyNpc_WasAttacked);
          BehaviorSubtreeUtility.SetCharacterSubtree(characterSubtree, ScriptableObjectInstance<IntroData>.Instance.SuokSubAI_FreeFight);
          byName1.AddListener((IChangeParameterListener) this);
          byName2.AddListener((IChangeParameterListener) this);
          break;
        case SuokCircleTutorialStateEnum.YouWin:
          byName1.RemoveListener((IChangeParameterListener) this);
          byName2.RemoveListener((IChangeParameterListener) this);
          this.currentTooltipBlueprint?.SendEvent("EnemyCanSurrender");
          this.currentTooltipBlueprint?.SendEvent("Finished");
          BehaviorSubtreeUtility.SetCharacterSubtree(characterSubtree, ScriptableObjectInstance<IntroData>.Instance.SuokSubAI_FinishedYouWin);
          ISpeakingComponent speaking = this.enemyEntity.GetComponent<ISpeakingComponent>();
          Action handlerOnBeginTalking1 = (Action) null;
          handlerOnBeginTalking = (Action) (() =>
          {
            this.SetState(SuokCircleTutorialStateEnum.AfterWinTalk);
            speaking.OnBeginTalking -= handlerOnBeginTalking1;
          });
          speaking.OnBeginTalking += handlerOnBeginTalking1;
          this.punchCount = 0;
          this.enemyNpc.WasPunchedEvent += new Action<EnemyBase>(this.EnemyHealth_ChangeValueEvent_Ragdoll);
          this.enemyNpc.WasPunchedToDodgeEvent += new Action<EnemyBase>(this.EnemyHealth_ChangeValueEvent_Ragdoll);
          this.enemyNpc.WasPunchedToQuickBlock += new Action<EnemyBase>(this.EnemyHealth_ChangeValueEvent_Ragdoll);
          this.enemyNpc.WasPunchedToSurrenderEvent += new Action<EnemyBase>(this.EnemyHealth_ChangeValueEvent_Ragdoll);
          this.enemyNpc.WasStaggeredEvent += new Action<EnemyBase>(this.EnemyHealth_ChangeValueEvent_Ragdoll);
          this.enemyNpc.WasPunchedToStaggerEvent += new Action<EnemyBase>(this.EnemyHealth_ChangeValueEvent_Ragdoll);
          break;
        case SuokCircleTutorialStateEnum.YouLoose:
          byName1.RemoveListener((IChangeParameterListener) this);
          byName2.RemoveListener((IChangeParameterListener) this);
          this.currentTooltipBlueprint?.SendEvent("FinishedLose");
          BehaviorSubtreeUtility.SetCharacterSubtree(characterSubtree, ScriptableObjectInstance<IntroData>.Instance.SuokSubAI_FinishedYouLoose);
          break;
        case SuokCircleTutorialStateEnum.Ragdoll:
          this.enemyNpc.WasPunchedEvent += new Action<EnemyBase>(this.EnemyHealth_ChangeValueEvent_Ragdoll);
          this.enemyNpc.WasPunchedToDodgeEvent += new Action<EnemyBase>(this.EnemyHealth_ChangeValueEvent_Ragdoll);
          this.enemyNpc.WasPunchedToQuickBlock += new Action<EnemyBase>(this.EnemyHealth_ChangeValueEvent_Ragdoll);
          this.enemyNpc.WasPunchedToSurrenderEvent += new Action<EnemyBase>(this.EnemyHealth_ChangeValueEvent_Ragdoll);
          this.enemyNpc.WasStaggeredEvent += new Action<EnemyBase>(this.EnemyHealth_ChangeValueEvent_Ragdoll);
          this.enemyNpc.WasPunchedToStaggerEvent += new Action<EnemyBase>(this.EnemyHealth_ChangeValueEvent_Ragdoll);
          BehaviorSubtreeUtility.SetCharacterSubtree(BehaviorSubtreeUtility.GetCharacterSubtree(this.enemyView), ScriptableObjectInstance<IntroData>.Instance.SuokSubAI_Ragdoll);
          FlowScriptController tooltipBlueprint = this.currentTooltipBlueprint;
          if (tooltipBlueprint != null)
          {
            tooltipBlueprint.SendEvent("YouKilledSurrender");
            break;
          }
          break;
      }
      this.state = state;
      Action stateChangedEvent = this.OnStateChangedEvent;
      if (stateChangedEvent == null)
        return;
      stateChangedEvent();
    }

    private void EnemyHealth_ChangeValueEvent_Ragdoll(EnemyBase enemy)
    {
      Debug.Log((object) nameof (EnemyHealth_ChangeValueEvent_Ragdoll));
      ++this.punchCount;
      if (this.punchCount != 4)
        return;
      Debug.Log((object) "EnemyHealth_ChangeValueEvent_Ragdoll 4");
      this.SetState(SuokCircleTutorialStateEnum.Ragdoll);
    }

    private void EnemyNpc_WasAttacked(EnemyBase enemy)
    {
      Debug.Log((object) nameof (EnemyNpc_WasAttacked));
      this.SetState(SuokCircleTutorialStateEnum.FreeFight);
    }

    private void EnemyNpc_WasStaggeredEvent(EnemyBase enemy)
    {
      Debug.Log((object) nameof (EnemyNpc_WasStaggeredEvent));
      this.SetState(SuokCircleTutorialStateEnum.WaitForLowStamina);
    }

    private void EnemyNpc_WasPrepunch(EnemyBase enemy)
    {
      Debug.Log((object) nameof (EnemyNpc_WasPrepunch));
      ++this.punchCount;
      if (4 != this.punchCount)
        return;
      Debug.Log((object) string.Format("EnemyNpc_WasPrepunch {0}", (object) this.punchCount));
      this.SetState(SuokCircleTutorialStateEnum.WaitForUppercut);
    }

    private void PlayerNpc_PunchFired(
      IEntity entity,
      ShotType shotType,
      ReactionType reactionType,
      WeaponEnum weaponEnum,
      ShotSubtypeEnum shotSubtypeEnum)
    {
      Debug.Log((object) nameof (PlayerNpc_PunchFired));
      if (shotType == ShotType.LowStamina)
      {
        Debug.Log((object) "PlayerNpc_PunchFired low stamina");
        this.SetState(SuokCircleTutorialStateEnum.YourDamageIsLow);
      }
      else
      {
        Debug.Log((object) "PlayerNpc_PunchFired other");
        ++this.punchCount;
        this.currentStamina -= 0.2f;
      }
    }

    private void EnemyNpc_WasPuncheDone(EnemyBase enemy)
    {
      Debug.Log((object) nameof (EnemyNpc_WasPuncheDone));
      ++this.punchCount;
    }

    private void PlayerWeaponService_WeaponShootEvent(
      WeaponKind arg1,
      IEntity arg2,
      ShotType arg3,
      ReactionType arg4,
      ShotSubtypeEnum arg5)
    {
      Debug.Log((object) nameof (PlayerWeaponService_WeaponShootEvent));
      ++this.punchCount;
      if (this.punchCount != 1)
        return;
      Debug.Log((object) "PlayerWeaponService_WeaponShootEvent 1");
      this.SetState(SuokCircleTutorialStateEnum.BlockExample);
    }

    private void PlayerNpc_WasPuncheToBlockDone(EnemyBase enemy)
    {
      Debug.Log((object) nameof (PlayerNpc_WasPuncheToBlockDone));
      ++this.punchCount;
      if (this.punchCount != 2)
        return;
      Debug.Log((object) "PlayerNpc_WasPuncheToBlockDone 2");
      this.SetState(SuokCircleTutorialStateEnum.RuinBlockExample);
    }

    private void PlayerNpc_WasStaggered(EnemyBase enemy)
    {
      Debug.Log((object) string.Format("PlayerNpc_WasStaggered {0}", (object) this.punchCount));
      ++this.punchCount;
      this.timeLeft = 5f;
      if (this.punchCount == 1)
      {
        Debug.Log((object) string.Format("PlayerNpc_WasStaggered 1 {0}", (object) this.punchCount));
        this.currentTooltipBlueprint?.SendEvent("RuinBlockExample");
      }
      else
      {
        if (this.punchCount != 2)
          return;
        Debug.Log((object) string.Format("PlayerNpc_WasStaggered 2 {0}", (object) this.punchCount));
        BehaviorSubtreeUtility.SetCharacterSubtree(BehaviorSubtreeUtility.GetCharacterSubtree(this.enemyView), ScriptableObjectInstance<IntroData>.Instance.SuokSubAI_WaitForStaminaGrows);
      }
    }

    private void Attacker_WeaponUnholsterEndEvent(WeaponKind weaponKind)
    {
      Debug.Log((object) nameof (Attacker_WeaponUnholsterEndEvent));
      this.SetState(SuokCircleTutorialStateEnum.WaitForPunch);
    }

    public void Initialise()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable((IUpdatable) this);
    }

    public void Terminate()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable((IUpdatable) this);
    }

    public void RegisterCombatant(GameObject character)
    {
      if (this.suokCombatants.Contains(character))
        Debug.LogError((object) (typeof (SuokCircleService).Name + " - registering combatant already in list"));
      else
        this.suokCombatants.Add(character);
    }

    public void UnregisterCombatant(GameObject character)
    {
      if (!this.suokCombatants.Remove(character))
        Debug.LogError((object) (typeof (SuokCircleService).Name + " - unregistering combatant not in list"));
      BehaviorSubtreeUtility.SetCharacterSubtree(BehaviorSubtreeUtility.GetCharacterSubtree(character), (ExternalBehaviorTree) null);
    }

    public void RegisterSpectator(GameObject character)
    {
      if (this.suokSpectators.Contains(character))
        Debug.LogError((object) (typeof (SuokCircleService).Name + " - registering spectator already in list"));
      else
        this.suokSpectators.Add(character);
    }

    public void UnregisterSpectator(GameObject character)
    {
      if (!this.suokSpectators.Remove(character))
        Debug.LogError((object) (typeof (SuokCircleService).Name + " - unregistering spectator not in list"));
      BehaviorSubtreeUtility.SetCharacterSubtree(BehaviorSubtreeUtility.GetCharacterSubtree(character), (ExternalBehaviorTree) null);
    }

    public void ComputeUpdate()
    {
      switch (this.state)
      {
        case SuokCircleTutorialStateEnum.Unknown:
          if (this.suokCombatants.Count != 1)
            break;
          this.playerEntity = ServiceLocator.GetService<ISimulation>().Player;
          this.playerView = ((IEntityView) this.playerEntity).GameObject;
          this.playerNpc = this.playerView.GetComponent<PlayerEnemy>();
          if (this.playerEntity != null && (UnityEngine.Object) this.playerView != (UnityEngine.Object) null)
          {
            this.playerParameters = this.playerEntity.GetComponent<ParametersComponent>();
            this.enemyView = this.suokCombatants[0];
            this.enemyEntity = EntityUtility.GetEntity(this.enemyView);
            this.enemyNpc = this.enemyView.GetComponent<NPCEnemy>();
            ((Entity) this.enemyEntity).AddListener((IEntityEventsListener) this);
            this.enemyParameters = this.enemyEntity.GetComponent<ParametersComponent>();
            this.SetState(SuokCircleTutorialStateEnum.WaitForInitialDialogEnd);
          }
          break;
        case SuokCircleTutorialStateEnum.WaitForLowStamina:
          if (this.punchCount < 1)
            break;
          this.timeLeft -= Time.deltaTime;
          if ((double) this.timeLeft < 0.0)
            this.SetState(SuokCircleTutorialStateEnum.YourDamageIsLow);
          break;
        case SuokCircleTutorialStateEnum.YourDamageIsLow:
          this.timeLeft -= Time.deltaTime;
          if ((double) this.timeLeft >= 0.0)
            break;
          Debug.Log((object) "From update: SetState(SuokCircleTutorialStateEnum.YourDamageIsLow)");
          this.SetState(SuokCircleTutorialStateEnum.BlockExample);
          break;
        case SuokCircleTutorialStateEnum.RuinBlockExample:
          if (this.enemyNpc.IsAttacking || this.playerNpc.IsStagger)
            this.timeLeft = Mathf.Max(this.timeLeft, 1f);
          if (this.punchCount <= 0)
            break;
          this.timeLeft -= Time.deltaTime;
          if ((double) this.timeLeft < 0.0)
          {
            Debug.Log((object) "From update: SetState(SuokCircleTutorialStateEnum.StaminaGrowsFasterInBlock)");
            this.SetState(SuokCircleTutorialStateEnum.StaminaGrowsFasterInBlock);
          }
          break;
        case SuokCircleTutorialStateEnum.StaminaGrowsFasterInBlock:
          if ((double) this.playerParameters.GetByName<float>(ParameterNameEnum.Stamina).Value <= 0.89999997615814209)
            break;
          Debug.Log((object) "From update: SetState(SuokCircleTutorialStateEnum.WaitForFreeFight)");
          this.SetState(SuokCircleTutorialStateEnum.WaitForFreeFight);
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
      this.punchCount = 0;
      this.state = SuokCircleTutorialStateEnum.Unknown;
    }

    void ISavesController.Save(IDataWriter element, string context)
    {
    }

    public void OnParameterChanged(IParameter parameter)
    {
      IParameter<float> byName1 = this.playerParameters.GetByName<float>(ParameterNameEnum.Health);
      IParameter<float> byName2 = this.enemyParameters.GetByName<float>(ParameterNameEnum.Health);
      if (byName1 == parameter)
      {
        Debug.Log((object) "Health_ChangeValueEvent");
        if ((double) ((IParameter<float>) parameter).Value >= 0.10000000149011612)
          return;
        Debug.Log((object) "Health_ChangeValueEvent < 0.1");
        this.SetState(SuokCircleTutorialStateEnum.YouLoose);
      }
      else if (byName2 == parameter)
      {
        Debug.Log((object) "EnemyHealth_ChangeValueEvent");
        if ((double) ((IParameter<float>) parameter).Value >= 0.10000000149011612)
          return;
        Debug.Log((object) "EnemyHealth_ChangeValueEvent < 0.1");
        this.SetState(SuokCircleTutorialStateEnum.YouWin);
      }
      else
        Debug.LogError((object) "!!! Такого быть не должно");
    }

    public void OnEntityEvent(IEntity sender, EntityEvents kind)
    {
      if (kind != EntityEvents.DisposeEvent)
        return;
      ServiceLocator.GetService<NotificationService>().RemoveNotify(NotificationEnum.Tooltip);
      if ((UnityEngine.Object) this.currentTooltipBlueprint != (UnityEngine.Object) null)
        UnityEngine.Object.Destroy((UnityEngine.Object) this.currentTooltipBlueprint.gameObject);
      if ((UnityEngine.Object) this.voicesBlueprint != (UnityEngine.Object) null)
        UnityEngine.Object.Destroy((UnityEngine.Object) this.voicesBlueprint.gameObject);
      ((Entity) sender).RemoveListener((IEntityEventsListener) this);
    }
  }
}
