using System;
using UnityEngine;
using UnityEngine.Serialization;

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
