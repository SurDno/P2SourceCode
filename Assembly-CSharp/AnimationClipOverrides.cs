using System;
using System.Collections.Generic;
using UnityEngine;

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
