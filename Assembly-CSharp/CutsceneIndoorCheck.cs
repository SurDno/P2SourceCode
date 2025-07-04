﻿using System;
using Engine.Impl.UI.Controls;
using UnityEngine;

public class CutsceneIndoorCheck : MonoBehaviour
{
  private static bool overridenInsideIndoor;
  [SerializeField]
  private HideableView view;

  private static event Action OverrideChangeEvent;

  public static void Set(bool value)
  {
    if (overridenInsideIndoor == value)
      return;
    overridenInsideIndoor = value;
    Action overrideChangeEvent = OverrideChangeEvent;
    if (overrideChangeEvent == null)
      return;
    overrideChangeEvent();
  }

  private void ApplyState()
  {
    if (!(view != null))
      return;
    view.Visible = overridenInsideIndoor;
  }

  private void OnEnable()
  {
    ApplyState();
    OverrideChangeEvent += ApplyState;
  }

  private void OnDisable()
  {
    OverrideChangeEvent -= ApplyState;
  }
}
