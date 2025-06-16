// Decompiled with JetBrains decompiler
// Type: AnimationClipOverrides
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class AnimationClipOverrides : List<KeyValuePair<AnimationClip, AnimationClip>>
{
  public AnimationClipOverrides(int capacity)
    : base(capacity)
  {
  }

  public AnimationClip this[string name]
  {
    get
    {
      return this.Find((Predicate<KeyValuePair<AnimationClip, AnimationClip>>) (x => x.Key.name.Equals(name))).Value;
    }
    set
    {
      int index = this.FindIndex((Predicate<KeyValuePair<AnimationClip, AnimationClip>>) (x => x.Key.name.Equals(name)));
      if (index == -1)
        return;
      this[index] = new KeyValuePair<AnimationClip, AnimationClip>(this[index].Key, value);
    }
  }
}
