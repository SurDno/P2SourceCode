using Engine.Source.Commons;
using Engine.Source.Components;
using System;
using UnityEngine;

namespace Engine.Source.Services
{
  public class POIDialogActivity
  {
    public POIServiceCharacterInfo DialogActor;
    public POIServiceCharacterInfo DialogTarget;
    public NpcStatePointOfInterest DialogTargetPoiState;
    public Vector3 EnterPoint;
    public POIBase poi;
    public bool IsPlaying;
    public bool IsCanceled;
    public Action OnComplete;
    public int CurrentDialogPairIndex = 0;
    public LipSyncObject ActorLipsyncObject;
    public LipSyncObject TargetLipsyncObject;
    public NPCSoundBankDescription ActorSoundBank;
    public NPCSoundBankDescription TargetSoundBank;
    public LipSyncComponent ActorLipsync;
    public LipSyncComponent TargetLipsync;
    public Transform ActorTransform;
    public Transform TargetTransform;
  }
}
