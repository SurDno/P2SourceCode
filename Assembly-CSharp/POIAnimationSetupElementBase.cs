// Decompiled with JetBrains decompiler
// Type: POIAnimationSetupElementBase
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
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
