// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.POIDialogActivity
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Source.Commons;
using Engine.Source.Components;
using System;
using UnityEngine;

#nullable disable
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
