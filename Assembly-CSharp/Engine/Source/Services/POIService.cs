// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.POIService
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using BehaviorDesigner.Runtime;
using Engine.Behaviours.Components;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable disable
namespace Engine.Source.Services
{
  [RuntimeService(new System.Type[] {typeof (POIService)})]
  public class POIService : IInitialisable, IUpdatable
  {
    private const int groupActivitiesMinimum = 3;
    private const float groupCreateInterval = 2f;
    private float timeFromLastGroupCreate = 2f;
    private Dictionary<GameObject, POIServiceCharacterInfo> characters = new Dictionary<GameObject, POIServiceCharacterInfo>();
    private Dictionary<POIServiceCharacterInfo, NpcStatePointOfInterest> freeDialogTargetCharacters = new Dictionary<POIServiceCharacterInfo, NpcStatePointOfInterest>();
    private List<POIDialogActivity> currentDialogs = new List<POIDialogActivity>();

    public void Initialise()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable((IUpdatable) this);
    }

    public void Terminate()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable((IUpdatable) this);
    }

    public void RegisterCharacter(GameObject character, bool searchClosestPOI = false, bool Crowd = false)
    {
      if (!this.characters.ContainsKey(character))
        this.characters.Add(character, new POIServiceCharacterInfo(character));
      this.characters[character].State = POIServiceCharacterStateEnum.Free;
      this.characters[character].SearchClosestPOI = searchClosestPOI;
      this.characters[character].IsCrowd = Crowd;
    }

    public void UnregisterCharacter(GameObject character)
    {
      if (this.characters.ContainsKey(character))
      {
        if (this.characters[character].State == POIServiceCharacterStateEnum.Dialog)
          this.CharacterCanceledDialogActivity(character);
        this.RemoveCharacterAsDialogTarget(character);
        this.characters[character].Clear();
        this.characters.Remove(character);
      }
      BehaviorSubtreeUtility.SetCharacterSubtree(BehaviorSubtreeUtility.GetCharacterSubtree(character), (ExternalBehaviorTree) null);
    }

    public void ComputeUpdate()
    {
      if (ServiceCache.OptimizationService.FrameHasSpike)
        return;
      this.timeFromLastGroupCreate -= Time.deltaTime;
      if ((double) this.timeFromLastGroupCreate <= 0.0)
      {
        this.timeFromLastGroupCreate = 2f;
        if (this.currentDialogs.Count < 3)
        {
          ServiceCache.OptimizationService.FrameHasSpike = true;
          this.TryCreateDialog();
          return;
        }
      }
      foreach (KeyValuePair<GameObject, POIServiceCharacterInfo> character in this.characters)
      {
        if (character.Value.State == POIServiceCharacterStateEnum.Free)
        {
          ServiceCache.OptimizationService.FrameHasSpike = true;
          this.SetSingleActivity(character);
          break;
        }
      }
    }

    public void AddCharacterAsDialogTarget(GameObject go, NpcStatePointOfInterest state)
    {
      if (!this.characters.ContainsKey(go))
        return;
      POIServiceCharacterInfo character = this.characters[go];
      if (character.IsIndoors)
        return;
      this.freeDialogTargetCharacters[character] = state;
    }

    public void RemoveCharacterAsDialogTarget(GameObject go)
    {
      if (!this.characters.ContainsKey(go))
        return;
      POIServiceCharacterInfo character = this.characters[go];
      if (this.freeDialogTargetCharacters.ContainsKey(character))
        this.freeDialogTargetCharacters.Remove(character);
      if (this.currentDialogs.Find((Predicate<POIDialogActivity>) (x => (UnityEngine.Object) x.DialogTarget.Character == (UnityEngine.Object) go)) == null)
        return;
      this.CharacterCanceledDialogActivity(go);
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
      if (this.freeDialogTargetCharacters.Keys.Count == 0)
        return;
      POIServiceCharacterInfo serviceCharacterInfo = this.freeDialogTargetCharacters.Keys.FirstOrDefault<POIServiceCharacterInfo>((Func<POIServiceCharacterInfo, bool>) (x => x.State != POIServiceCharacterStateEnum.Dialog));
      if (serviceCharacterInfo == null)
        return;
      POIServiceCharacterInfo dialogActor = this.GetDialogActor(serviceCharacterInfo);
      if (dialogActor == null)
        return;
      NpcStatePointOfInterest dialogTargetCharacter = this.freeDialogTargetCharacters[serviceCharacterInfo];
      POIBase poi = dialogTargetCharacter.GetPOI();
      if ((UnityEngine.Object) poi == (UnityEngine.Object) null)
      {
        this.RemoveCharacterAsDialogTarget(serviceCharacterInfo.Character);
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
        this.currentDialogs.Add(dialog);
        this.ActorGoToDialog(dialog);
      }
    }

    public void ActorGoToDialog(POIDialogActivity dialog)
    {
      Vector3 closestTargetPosition;
      dialog.poi.GetClosestTargetPoint(POIAnimationEnum.S_Dialog, 0, dialog.DialogActor.Character.GetComponent<POISetup>(), dialog.EnterPoint, out closestTargetPosition, out Quaternion _);
      BehaviorTree characterSubtree = BehaviorSubtreeUtility.GetCharacterSubtree(dialog.DialogActor.Character);
      BehaviorSubtreeUtility.SetCharacterSubtree(characterSubtree, ScriptableObjectInstance<ResourceFromCodeData>.Instance.POIDialogGoToTarget);
      characterSubtree.SetVariableValue("TargetPosition", (object) closestTargetPosition);
    }

    public void CharacterReadyForDialog(GameObject character)
    {
      CoroutineService.Instance.WaitFrame((Action) (() => this.CharacterReadyForDialogAction(character)));
    }

    private void CharacterReadyForDialogAction(GameObject character)
    {
      POIDialogActivity poiDialogActivity = this.currentDialogs.Find((Predicate<POIDialogActivity>) (x => (UnityEngine.Object) x.DialogActor.Character == (UnityEngine.Object) character));
      if (poiDialogActivity == null)
        return;
      BehaviorTree characterSubtree = BehaviorSubtreeUtility.GetCharacterSubtree(character);
      BehaviorSubtreeUtility.SetCharacterSubtree(characterSubtree, ScriptableObjectInstance<ResourceFromCodeData>.Instance.POIDialogSpeakToCharacter);
      if ((UnityEngine.Object) characterSubtree != (UnityEngine.Object) null && poiDialogActivity.DialogTarget != null)
        characterSubtree.SetVariableValue("TargetGameObject", (object) poiDialogActivity.DialogTarget.Character);
      if (poiDialogActivity.DialogTarget == null || !((UnityEngine.Object) poiDialogActivity.DialogTarget.Character != (UnityEngine.Object) null))
        return;
      poiDialogActivity.DialogTargetPoiState.LookAt(poiDialogActivity.DialogActor.Character);
    }

    private POIServiceCharacterInfo GetDialogActor(POIServiceCharacterInfo target)
    {
      POIServiceCharacterInfo dialogActor = (POIServiceCharacterInfo) null;
      float num = float.MaxValue;
      foreach (KeyValuePair<GameObject, POIServiceCharacterInfo> character in this.characters)
      {
        if (character.Value != target && !character.Value.IsIndoors && (character.Value.State == POIServiceCharacterStateEnum.Free || character.Value.State == POIServiceCharacterStateEnum.SingleActivity))
        {
          Pivot component1 = character.Key.GetComponent<Pivot>();
          if (!((UnityEngine.Object) component1.SoundBank == (UnityEngine.Object) null))
          {
            Pivot component2 = target.Character.GetComponent<Pivot>();
            if (!((UnityEngine.Object) component2.SoundBank == (UnityEngine.Object) null) && (component2.SoundBank.DialogRole != NPCSoundBankDialogRoleEnum.Child || component1.SoundBank.DialogRole == NPCSoundBankDialogRoleEnum.Child) && (component2.SoundBank.DialogRole == NPCSoundBankDialogRoleEnum.Child || component1.SoundBank.DialogRole != NPCSoundBankDialogRoleEnum.Child))
            {
              POISetup component3 = character.Key.GetComponent<POISetup>();
              if (!((UnityEngine.Object) component3 == (UnityEngine.Object) null) && component3.SupportedAnimations.HasValue<POIAnimationEnum>(POIAnimationEnum.S_Dialog))
              {
                float magnitude = (character.Value.Character.transform.position - target.Character.transform.position).magnitude;
                if ((double) magnitude < (double) num)
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
      this.StopDialog(character);
      CoroutineService.Instance.WaitFrame((Action) (() => this.CharacterCanceledDialogActivityAction(character)));
    }

    private void CharacterCanceledDialogActivityAction(GameObject character)
    {
      POIDialogActivity dialog = this.currentDialogs.Find((Predicate<POIDialogActivity>) (x => (UnityEngine.Object) x.DialogTarget.Character == (UnityEngine.Object) character || (UnityEngine.Object) x.DialogActor.Character == (UnityEngine.Object) character));
      if (dialog == null)
        return;
      this.CancelDialogActivity(dialog);
    }

    public void CancelDialogActivity(POIDialogActivity dialog)
    {
      dialog.DialogActor.State = POIServiceCharacterStateEnum.Free;
      dialog.DialogTarget.State = POIServiceCharacterStateEnum.SingleActivity;
      dialog.DialogTargetPoiState.SetDialogFreeze(false);
      dialog.IsCanceled = true;
      this.currentDialogs.Remove(dialog);
    }

    public void StartDialog(GameObject actor, GameObject target)
    {
      POIDialogActivity dialog = this.currentDialogs.Find((Predicate<POIDialogActivity>) (x => (UnityEngine.Object) x.DialogActor.Character == (UnityEngine.Object) actor));
      if (dialog == null)
        return;
      Pivot component1 = actor.GetComponent<Pivot>();
      Pivot component2 = target.GetComponent<Pivot>();
      if ((UnityEngine.Object) component1 == (UnityEngine.Object) null || (UnityEngine.Object) component2 == (UnityEngine.Object) null)
        return;
      dialog.ActorSoundBank = component1.SoundBank;
      dialog.TargetSoundBank = component2.SoundBank;
      if ((UnityEngine.Object) dialog.ActorSoundBank == (UnityEngine.Object) null || (UnityEngine.Object) dialog.TargetSoundBank == (UnityEngine.Object) null)
        return;
      NPCSoundBankDialogTypeEnum type = this.GetRightDialogType(component1.SoundBank, component2.SoundBank);
      NPCSoundBankDialogObject bankDialogObject1 = dialog.ActorSoundBank.CivilTalks.Find((Predicate<NPCSoundBankDialogObject>) (x => x.Type == type));
      NPCSoundBankDialogObject bankDialogObject2 = dialog.TargetSoundBank.CivilTalks.Find((Predicate<NPCSoundBankDialogObject>) (x => x.Type == type));
      if (bankDialogObject1 == null)
        return;
      dialog.CurrentDialogPairIndex = (int) ((double) UnityEngine.Random.value * (double) bankDialogObject1.Pairs.Count);
      if (dialog.CurrentDialogPairIndex >= bankDialogObject1.Pairs.Count)
        return;
      NPCSoundBankDialogObjectPair pair1 = bankDialogObject1.Pairs[dialog.CurrentDialogPairIndex];
      NPCSoundBankDialogObjectPair pair2 = bankDialogObject2 == null || dialog.CurrentDialogPairIndex >= bankDialogObject2.Pairs.Count ? (NPCSoundBankDialogObjectPair) null : bankDialogObject2.Pairs[dialog.CurrentDialogPairIndex];
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
      this.PlayDialogLipsyncs(dialog);
    }

    private void PlayDialogLipsyncs(POIDialogActivity dialog)
    {
      if (dialog.IsCanceled || dialog.IsPlaying)
        return;
      if (dialog.ActorLipsync != null)
        dialog.ActorLipsync.Play3D((ILipSyncObject) dialog.ActorLipsyncObject, ScriptableObjectInstance<ResourceFromCodeData>.Instance.AudioSourceForNpcDialogs, true);
      if (dialog.TargetLipsync != null && dialog.TargetLipsyncObject != null)
        dialog.TargetLipsync.Play3D((ILipSyncObject) dialog.TargetLipsyncObject, ScriptableObjectInstance<ResourceFromCodeData>.Instance.AudioSourceForNpcDialogs, true);
      dialog.IsPlaying = true;
      dialog.OnComplete = (Action) (() => this.DialogComplete(dialog.DialogActor.Character));
      if (dialog.ActorLipsync == null)
        return;
      dialog.ActorLipsync.PlayCompleteEvent -= dialog.OnComplete;
      dialog.ActorLipsync.PlayCompleteEvent += dialog.OnComplete;
    }

    private void DialogComplete(GameObject character)
    {
      POIDialogActivity dialog = this.currentDialogs.Find((Predicate<POIDialogActivity>) (x => (UnityEngine.Object) x.DialogActor.Character == (UnityEngine.Object) character));
      if (dialog == null)
        return;
      dialog.IsPlaying = false;
      CoroutineService.Instance.WaitFrame((Action) (() =>
      {
        if (dialog.IsCanceled)
          return;
        this.PlayDialogLipsyncs(dialog);
      }));
    }

    public void StopDialog(GameObject character)
    {
      POIDialogActivity poiDialogActivity = this.currentDialogs.Find((Predicate<POIDialogActivity>) (x => (UnityEngine.Object) x.DialogActor.Character == (UnityEngine.Object) character || (UnityEngine.Object) x.DialogTarget.Character == (UnityEngine.Object) character));
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
      POIDialogActivity poiDialogActivity = this.currentDialogs.Find((Predicate<POIDialogActivity>) (x => (UnityEngine.Object) x.DialogTarget.Character == (UnityEngine.Object) character || (UnityEngine.Object) x.DialogActor.Character == (UnityEngine.Object) character));
      if (poiDialogActivity == null)
        return;
      if (poiDialogActivity.ActorLipsync != null)
        poiDialogActivity.ActorLipsync.Play3D((ILipSyncObject) poiDialogActivity.ActorLipsyncObject, ScriptableObjectInstance<ResourceFromCodeData>.Instance.AudioSourceForNpcDialogs, true);
      if (poiDialogActivity.TargetLipsync != null && poiDialogActivity.TargetLipsyncObject != null)
        poiDialogActivity.TargetLipsync.Play3D((ILipSyncObject) poiDialogActivity.TargetLipsyncObject, ScriptableObjectInstance<ResourceFromCodeData>.Instance.AudioSourceForNpcDialogs, true);
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
