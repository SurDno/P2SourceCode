using System;
using UnityEngine;

[Serializable]
public class POIAnimationSetupElementBase : ICloneable
{
  [SerializeField]
  public AnimationClip AnimationClip;
  [SerializeField]
  public Vector3 Offset;
  [SerializeField]
  public bool PlayOnce;
  [SerializeField]
  public bool NeedSynchroniseAnimation;
  [SerializeField]
  public float AnimationSyncDelay;

  public virtual object Clone() => throw new NotImplementedException();
}
