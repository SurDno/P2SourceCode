﻿using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class RandomSoundEventView : EventView
  {
    [SerializeField]
    private SoundCollection soundCollection;

    public override void Invoke()
    {
      AudioClip clip = soundCollection?.GetClip();
      if (clip == null)
        return;
      MonoBehaviourInstance<UISounds>.Instance?.PlaySound(clip);
    }
  }
}
