using System;

[Serializable]
public class POIAnimationSetupElementQuick : POIAnimationSetupElementBase
{
  [SerializeField]
  [FormerlySerializedAs("AnimationClip")]
  public AnimationClip AnimationClipQuick;

  public override object Clone()
  {
    POIAnimationSetupElementQuick setupElementQuick = new POIAnimationSetupElementQuick();
    setupElementQuick.AnimationClipQuick = AnimationClipQuick;
    setupElementQuick.Offset = Offset;
    setupElementQuick.PlayOnce = PlayOnce;
    setupElementQuick.NeedSynchroniseAnimation = NeedSynchroniseAnimation;
    setupElementQuick.AnimationSyncDelay = AnimationSyncDelay;
    return setupElementQuick;
  }
}
