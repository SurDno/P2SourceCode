// Decompiled with JetBrains decompiler
// Type: POIAnimationSetupElementQuick
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.Serialization;

#nullable disable
[Serializable]
public class POIAnimationSetupElementQuick : POIAnimationSetupElementBase
{
  [SerializeField]
  [FormerlySerializedAs("AnimationClip")]
  public AnimationClip AnimationClipQuick;

  public override object Clone()
  {
    POIAnimationSetupElementQuick setupElementQuick = new POIAnimationSetupElementQuick();
    setupElementQuick.AnimationClipQuick = this.AnimationClipQuick;
    setupElementQuick.Offset = this.Offset;
    setupElementQuick.PlayOnce = this.PlayOnce;
    setupElementQuick.NeedSynchroniseAnimation = this.NeedSynchroniseAnimation;
    setupElementQuick.AnimationSyncDelay = this.AnimationSyncDelay;
    return (object) setupElementQuick;
  }
}
