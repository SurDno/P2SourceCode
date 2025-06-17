using System;
using System.Collections.Generic;
using System.Linq;
using BehaviorDesigner.Runtime;
using Engine.Behaviours.Components;
using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Engine.Source.Services
{
  [RuntimeService(typeof (POIService))]
  public class POIService : IInitialisable, IUpdatable
  {
    private const int groupActivitiesMinimum = 3;
    private const float groupCreateInterval = 2f;
    private float timeFromLastGroupCreate = 2f;
    private Dictionary<GameObject, POIServiceCharacterInfo> characters = new();
    private Dictionary<POIServiceCharacterInfo, NpcStatePointOfInterest> freeDialogTargetCharacters = new();
    private List<POIDialogActivity> currentDialogs = [];

    public void Initialise()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable(this);
    }

    public void Terminate()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable(this);
    }

    public void RegisterCharacter(GameObject character, bool searchClosestPOI = false, bool Crowd = false)
    {
      if (!characters.ContainsKey(character))
        characters.Add(character, new POIServiceCharacterInfo(character));
      characters[character].State = POIServiceCharacterStateEnum.Free;
      characters[character].SearchClosestPOI = searchClosestPOI;
      characters[character].IsCrowd = Crowd;
    }

    public void UnregisterCharacter(GameObject character)
    {
      if (characters.ContainsKey(character))
      {
        if (characters[character].State == POIServiceCharacterStateEnum.Dialog)
          CharacterCanceledDialogActivity(character);
        RemoveCharacterAsDialogTarget(character);
        characters[character].Clear();
        characters.Remove(character);
      }
      BehaviorSubtreeUtility.SetCharacterSubtree(BehaviorSubtreeUtility.GetCharacterSubtree(character), null);
    }

    public void ComputeUpdate()
    {
      if (ServiceCache.OptimizationService.FrameHasSpike)
        return;
      timeFromLastGroupCreate -= Time.deltaTime;
      if (timeFromLastGroupCreate <= 0.0)
      {
        timeFromLastGroupCreate = 2f;
        if (currentDialogs.Count < 3)
        {
          ServiceCache.OptimizationService.FrameHasSpike = true;
          TryCreateDialog();
          return;
        }
      }
      foreach (KeyValuePair<GameObject, POIServiceCharacterInfo> character in characters)
      {
        if (character.Value.State == POIServiceCharacterStateEnum.Free)
        {
          ServiceCache.OptimizationService.FrameHasSpike = true;
          SetSingleActivity(character);
          break;
        }
      }
    }

    public void AddCharacterAsDialogTarget(GameObject go, NpcStatePointOfInterest state)
    {
      if (!characters.ContainsKey(go))
        return;
      POIServiceCharacterInfo character = characters[go];
      if (character.IsIndoors)
        return;
      freeDialogTargetCharacters[character] = state;
    }

    public void RemoveCharacterAsDialogTarget(GameObject go)
    {
      if (!characters.ContainsKey(go))
        return;
      POIServiceCharacterInfo character = characters[go];
      if (freeDialogTargetCharacters.ContainsKey(character))
        freeDialogTargetCharacters.Remove(character);
      if (currentDialogs.Find(x => x.DialogTarget.Character == go) == null)
        return;
      CharacterCanceledDialogActivity(go);
    }

    private void SetSingleActivity(
      KeyValuePair<GameObject, POIServiceCharacterInfo> pair)
    {
      BehaviorTree characterSubtree = BehaviorSubtreeUtility.GetCharacterSubtree(pair.Key);
      bool flag = false;
      IEntity owner = pair.Key.GetComponent<EngineGameObject>()?.Owner;
      if (owner != null)
      {
        CrowdItemComponent component = owner.GetComponent<CrowdItemComponent>();
        if (component != null)
          flag = component.Crowd != null && pair.Value.IsCrowd;
      }
      if (pair.Value.SearchClosestPOI)
        BehaviorSubtreeUtility.SetCharacterSubtree(characterSubtree, ScriptableObjectInstance<ResourceFromCodeData>.Instance.POIAIClosest);
      else if (flag)
      {
        if (pair.Value.IsIndoors)
          BehaviorSubtreeUtility.SetCharacterSubtree(characterSubtree, ScriptableObjectInstance<ResourceFromCodeData>.Instance.POIAIIndoorsCrowd);
        else
          BehaviorSubtreeUtility.SetCharacterSubtree(characterSubtree, ScriptableObjectInstance<ResourceFromCodeData>.Instance.POIAICrowd);
      }
      else
        BehaviorSubtreeUtility.SetCharacterSubtree(characterSubtree, ScriptableObjectInstance<ResourceFromCodeData>.Instance.POIAI);
      pair.Value.State = POIServiceCharacterStateEnum.SingleActivity;
    }

    private void TryCreateDialog()
    {
      if (freeDialogTargetCharacters.Keys.Count == 0)
        return;
      POIServiceCharacterInfo serviceCharacterInfo = freeDialogTargetCharacters.Keys.FirstOrDefault(x => x.State != POIServiceCharacterStateEnum.Dialog);
      if (serviceCharacterInfo == null)
        return;
      POIServiceCharacterInfo dialogActor = GetDialogActor(serviceCharacterInfo);
      if (dialogActor == null)
        return;
      NpcStatePointOfInterest dialogTargetCharacter = freeDialogTargetCharacters[serviceCharacterInfo];
      POIBase poi = dialogTargetCharacter.GetPOI();
      if (poi == null)
      {
        RemoveCharacterAsDialogTarget(serviceCharacterInfo.Character);
      }
      else
      {
        serviceCharacterInfo.State = POIServiceCharacterStateEnum.Dialog;
        dialogTargetCharacter.SetDialogFreeze(true);
        dialogActor.State = POIServiceCharacterStateEnum.Dialog;
        POIDialogActivity dialog = new POIDialogActivity();
        dialog.DialogTarget = serviceCharacterInfo;
        dialog.DialogTargetPoiState = dialogTargetCharacter;
        dialog.DialogActor = dialogActor;
        dialog.EnterPoint = dialogTargetCharacter.GetEnterPoint();
        dialog.poi = poi;
        currentDialogs.Add(dialog);
        ActorGoToDialog(dialog);
      }
    }

    public void ActorGoToDialog(POIDialogActivity dialog)
    {
      dialog.poi.GetClosestTargetPoint(POIAnimationEnum.S_Dialog, 0, dialog.DialogActor.Character.GetComponent<POISetup>(), dialog.EnterPoint, out Vector3 closestTargetPosition, out Quaternion _);
      BehaviorTree characterSubtree = BehaviorSubtreeUtility.GetCharacterSubtree(dialog.DialogActor.Character);
      BehaviorSubtreeUtility.SetCharacterSubtree(characterSubtree, ScriptableObjectInstance<ResourceFromCodeData>.Instance.POIDialogGoToTarget);
      characterSubtree.SetVariableValue("TargetPosition", closestTargetPosition);
    }

    public void CharacterReadyForDialog(GameObject character)
    {
      CoroutineService.Instance.WaitFrame((Action) (() => CharacterReadyForDialogAction(character)));
    }

    private void CharacterReadyForDialogAction(GameObject character)
    {
      POIDialogActivity poiDialogActivity = currentDialogs.Find(x => x.DialogActor.Character == character);
      if (poiDialogActivity == null)
        return;
      BehaviorTree characterSubtree = BehaviorSubtreeUtility.GetCharacterSubtree(character);
      BehaviorSubtreeUtility.SetCharacterSubtree(characterSubtree, ScriptableObjectInstance<ResourceFromCodeData>.Instance.POIDialogSpeakToCharacter);
      if (characterSubtree != null && poiDialogActivity.DialogTarget != null)
        characterSubtree.SetVariableValue("TargetGameObject", poiDialogActivity.DialogTarget.Character);
      if (poiDialogActivity.DialogTarget == null || !(poiDialogActivity.DialogTarget.Character != null))
        return;
      poiDialogActivity.DialogTargetPoiState.LookAt(poiDialogActivity.DialogActor.Character);
    }

    private POIServiceCharacterInfo GetDialogActor(POIServiceCharacterInfo target)
    {
      POIServiceCharacterInfo dialogActor = null;
      float num = float.MaxValue;
      foreach (KeyValuePair<GameObject, POIServiceCharacterInfo> character in characters)
      {
        if (character.Value != target && !character.Value.IsIndoors && (character.Value.State == POIServiceCharacterStateEnum.Free || character.Value.State == POIServiceCharacterStateEnum.SingleActivity))
        {
          Pivot component1 = character.Key.GetComponent<Pivot>();
          if (!(component1.SoundBank == null))
          {
            Pivot component2 = target.Character.GetComponent<Pivot>();
            if (!(component2.SoundBank == null) && (component2.SoundBank.DialogRole != NPCSoundBankDialogRoleEnum.Child || component1.SoundBank.DialogRole == NPCSoundBankDialogRoleEnum.Child) && (component2.SoundBank.DialogRole == NPCSoundBankDialogRoleEnum.Child || component1.SoundBank.DialogRole != NPCSoundBankDialogRoleEnum.Child))
            {
              POISetup component3 = character.Key.GetComponent<POISetup>();
              if (!(component3 == null) && component3.SupportedAnimations.HasValue(POIAnimationEnum.S_Dialog))
              {
                float magnitude = (character.Value.Character.transform.position - target.Character.transform.position).magnitude;
                if (magnitude < (double) num)
                {
                  num = magnitude;
                  dialogActor = character.Value;
                }
              }
            }
          }
        }
      }
      return dialogActor;
    }

    public void CharacterCanceledDialogActivity(GameObject character)
    {
      StopDialog(character);
      CoroutineService.Instance.WaitFrame((Action) (() => CharacterCanceledDialogActivityAction(character)));
    }

    private void CharacterCanceledDialogActivityAction(GameObject character)
    {
      POIDialogActivity dialog = currentDialogs.Find(x => x.DialogTarget.Character == character || x.DialogActor.Character == character);
      if (dialog == null)
        return;
      CancelDialogActivity(dialog);
    }

    public void CancelDialogActivity(POIDialogActivity dialog)
    {
      dialog.DialogActor.State = POIServiceCharacterStateEnum.Free;
      dialog.DialogTarget.State = POIServiceCharacterStateEnum.SingleActivity;
      dialog.DialogTargetPoiState.SetDialogFreeze(false);
      dialog.IsCanceled = true;
      currentDialogs.Remove(dialog);
    }

    public void StartDialog(GameObject actor, GameObject target)
    {
      POIDialogActivity dialog = currentDialogs.Find(x => x.DialogActor.Character == actor);
      if (dialog == null)
        return;
      Pivot component1 = actor.GetComponent<Pivot>();
      Pivot component2 = target.GetComponent<Pivot>();
      if (component1 == null || component2 == null)
        return;
      dialog.ActorSoundBank = component1.SoundBank;
      dialog.TargetSoundBank = component2.SoundBank;
      if (dialog.ActorSoundBank == null || dialog.TargetSoundBank == null)
        return;
      NPCSoundBankDialogTypeEnum type = GetRightDialogType(component1.SoundBank, component2.SoundBank);
      NPCSoundBankDialogObject bankDialogObject1 = dialog.ActorSoundBank.CivilTalks.Find(x => x.Type == type);
      NPCSoundBankDialogObject bankDialogObject2 = dialog.TargetSoundBank.CivilTalks.Find(x => x.Type == type);
      if (bankDialogObject1 == null)
        return;
      dialog.CurrentDialogPairIndex = (int) (Random.value * (double) bankDialogObject1.Pairs.Count);
      if (dialog.CurrentDialogPairIndex >= bankDialogObject1.Pairs.Count)
        return;
      NPCSoundBankDialogObjectPair pair1 = bankDialogObject1.Pairs[dialog.CurrentDialogPairIndex];
      NPCSoundBankDialogObjectPair pair2 = bankDialogObject2 == null || dialog.CurrentDialogPairIndex >= bankDialogObject2.Pairs.Count ? null : bankDialogObject2.Pairs[dialog.CurrentDialogPairIndex];
      dialog.ActorLipsyncObject = pair1.DialogA.Value;
      if (pair2 != null)
        dialog.TargetLipsyncObject = pair2.DialogB.Value;
      if (dialog.ActorLipsyncObject == null)
        return;
      IEntity entity1 = dialog.DialogActor.Entity;
      IEntity entity2 = dialog.DialogTarget.Entity;
      if (entity1 == null || entity2 == null)
        return;
      dialog.ActorLipsync = entity1.GetComponent<LipSyncComponent>();
      dialog.TargetLipsync = entity2.GetComponent<LipSyncComponent>();
      PlayDialogLipsyncs(dialog);
    }

    private void PlayDialogLipsyncs(POIDialogActivity dialog)
    {
      if (dialog.IsCanceled || dialog.IsPlaying)
        return;
      if (dialog.ActorLipsync != null)
        dialog.ActorLipsync.Play3D(dialog.ActorLipsyncObject, ScriptableObjectInstance<ResourceFromCodeData>.Instance.AudioSourceForNpcDialogs, true);
      if (dialog.TargetLipsync != null && dialog.TargetLipsyncObject != null)
        dialog.TargetLipsync.Play3D(dialog.TargetLipsyncObject, ScriptableObjectInstance<ResourceFromCodeData>.Instance.AudioSourceForNpcDialogs, true);
      dialog.IsPlaying = true;
      dialog.OnComplete = (Action) (() => DialogComplete(dialog.DialogActor.Character));
      if (dialog.ActorLipsync == null)
        return;
      dialog.ActorLipsync.PlayCompleteEvent -= dialog.OnComplete;
      dialog.ActorLipsync.PlayCompleteEvent += dialog.OnComplete;
    }

    private void DialogComplete(GameObject character)
    {
      POIDialogActivity dialog = currentDialogs.Find(x => x.DialogActor.Character == character);
      if (dialog == null)
        return;
      dialog.IsPlaying = false;
      CoroutineService.Instance.WaitFrame((Action) (() =>
      {
        if (dialog.IsCanceled)
          return;
        PlayDialogLipsyncs(dialog);
      }));
    }

    public void StopDialog(GameObject character)
    {
      POIDialogActivity poiDialogActivity = currentDialogs.Find(x => x.DialogActor.Character == character || x.DialogTarget.Character == character);
      if (poiDialogActivity == null)
        return;
      poiDialogActivity.IsPlaying = false;
      poiDialogActivity.IsCanceled = true;
      if (poiDialogActivity.ActorLipsync != null)
      {
        poiDialogActivity.ActorLipsync.PlayCompleteEvent -= poiDialogActivity.OnComplete;
        poiDialogActivity.ActorLipsync.Stop();
      }
      if (poiDialogActivity.TargetLipsync != null)
        poiDialogActivity.TargetLipsync.Stop();
    }

    public void ResumeDialog(GameObject character)
    {
      POIDialogActivity poiDialogActivity = currentDialogs.Find(x => x.DialogTarget.Character == character || x.DialogActor.Character == character);
      if (poiDialogActivity == null)
        return;
      if (poiDialogActivity.ActorLipsync != null)
        poiDialogActivity.ActorLipsync.Play3D(poiDialogActivity.ActorLipsyncObject, ScriptableObjectInstance<ResourceFromCodeData>.Instance.AudioSourceForNpcDialogs, true);
      if (poiDialogActivity.TargetLipsync != null && poiDialogActivity.TargetLipsyncObject != null)
        poiDialogActivity.TargetLipsync.Play3D(poiDialogActivity.TargetLipsyncObject, ScriptableObjectInstance<ResourceFromCodeData>.Instance.AudioSourceForNpcDialogs, true);
      poiDialogActivity.IsPlaying = true;
      if (poiDialogActivity.ActorLipsync != null)
      {
        poiDialogActivity.ActorLipsync.PlayCompleteEvent -= poiDialogActivity.OnComplete;
        poiDialogActivity.ActorLipsync.PlayCompleteEvent += poiDialogActivity.OnComplete;
      }
    }

    private NPCSoundBankDialogTypeEnum GetRightDialogType(
      NPCSoundBankDescription actor,
      NPCSoundBankDescription target)
    {
      if (actor.DialogRole == NPCSoundBankDialogRoleEnum.MaleAdult && target.DialogRole == NPCSoundBankDialogRoleEnum.MaleAdult)
        return NPCSoundBankDialogTypeEnum.MaleToMale;
      if (actor.DialogRole == NPCSoundBankDialogRoleEnum.FemaleAdult && target.DialogRole == NPCSoundBankDialogRoleEnum.FemaleAdult)
        return NPCSoundBankDialogTypeEnum.FemaleToFemale;
      if (actor.DialogRole == NPCSoundBankDialogRoleEnum.MaleAdult && target.DialogRole == NPCSoundBankDialogRoleEnum.FemaleAdult || actor.DialogRole == NPCSoundBankDialogRoleEnum.FemaleAdult && target.DialogRole == NPCSoundBankDialogRoleEnum.MaleAdult)
        return NPCSoundBankDialogTypeEnum.MaleToFemale;
      if ((actor.DialogRole == NPCSoundBankDialogRoleEnum.MaleAdult || actor.DialogRole == NPCSoundBankDialogRoleEnum.FemaleAdult) && target.DialogRole == NPCSoundBankDialogRoleEnum.Child)
        return NPCSoundBankDialogTypeEnum.AdultToChild;
      return actor.DialogRole == NPCSoundBankDialogRoleEnum.Child ? NPCSoundBankDialogTypeEnum.ChildToAnyone : NPCSoundBankDialogTypeEnum.Unknown;
    }
  }
}
