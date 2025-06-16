using System;
using Engine.Common.Services;
using Engine.Impl.UI.Controls;
using Engine.Source.Services;
using UnityEngine;

public class PlayerIndoorCheck : EngineDependent
{
  private static bool overriden;
  private static bool overridenInsideIndoor;
  [SerializeField]
  private HideableView view;
  [FromLocator]
  private InsideIndoorListener insideIndoorListener;
  private bool insideIndoor;

  private static event Action OverrideChangeEvent;

  public static void Override(bool value)
  {
    overriden = true;
    overridenInsideIndoor = value;
    Action overrideChangeEvent = OverrideChangeEvent;
    if (overrideChangeEvent == null)
      return;
    overrideChangeEvent();
  }

  public static void ResetOverride()
  {
    overriden = false;
    Action overrideChangeEvent = OverrideChangeEvent;
    if (overrideChangeEvent == null)
      return;
    overrideChangeEvent();
  }

  private void ApplyState()
  {
    if (!(view != null))
      return;
    view.Visible = overriden ? overridenInsideIndoor : insideIndoor;
  }

  private void SetInsideIndoor(bool value)
  {
    insideIndoor = value;
    ApplyState();
  }

  private void OnBeginExit() => SetInsideIndoor(false);

  protected override void OnConnectToEngine()
  {
    SetInsideIndoor(insideIndoorListener.InsideIndoor);
    insideIndoorListener.OnInsideIndoorChanged += SetInsideIndoor;
    insideIndoorListener.OnPlayerBeginsExit += OnBeginExit;
    OverrideChangeEvent += ApplyState;
  }

  protected override void OnDisconnectFromEngine()
  {
    insideIndoorListener.OnInsideIndoorChanged -= SetInsideIndoor;
    insideIndoorListener.OnPlayerBeginsExit -= OnBeginExit;
    OverrideChangeEvent -= ApplyState;
  }
}
