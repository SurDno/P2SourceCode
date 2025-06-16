// Decompiled with JetBrains decompiler
// Type: POIAnimationSetupBase
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

#nullable disable
[Serializable]
public class POIAnimationSetupBase
{
  [SerializeField]
  public AnimationClip EnterAnimationClip;
  [SerializeField]
  public AnimationClip ExitAnimationClip;
  [SerializeField]
  [FormerlySerializedAs("Elements")]
  public List<POIAnimationSetupElementBase> ElementsOld = new List<POIAnimationSetupElementBase>();

  public virtual List<POIAnimationSetupElementBase> Elements { get; }
}
