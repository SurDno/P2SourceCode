using System;
using Engine.Common.Services;
using Engine.Impl.UI.Controls;
using Engine.Source.Services;

public class PlayerIsolatedIndoorCheck : EngineDependent
{
  private static bool overriden;
  private static bool overridenIsolatedIndoor;
  [SerializeField]
  private HideableView view;
  [FromLocator]
  private InsideIndoorListener insideIndoorListener;
  private bool isolatedIndoor;

  private static event Action OverrideChangeEvent;

  public static void Override(bool value)
  {
    overriden = true;
    overridenIsolatedIndoor = value;
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
    if (!((UnityEngine.Object) view != (UnityEngine.Object) null))
      return;
    view.Visible = overriden ? overridenIsolatedIndoor : isolatedIndoor;
  }

  private void SetIsolatedIndoor(bool value)
  {
    isolatedIndoor = value;
    ApplyState();
  }

  private void OnBeginExit() => SetIsolatedIndoor(false);

  protected override void OnConnectToEngine()
  {
    SetIsolatedIndoor(insideIndoorListener.IsolatedIndoor);
    insideIndoorListener.OnIsolatedIndoorChanged += SetIsolatedIndoor;
    insideIndoorListener.OnPlayerBeginsExit += OnBeginExit;
    OverrideChangeEvent += ApplyState;
  }

  protected override void OnDisconnectFromEngine()
  {
    insideIndoorListener.OnIsolatedIndoorChanged -= SetIsolatedIndoor;
    insideIndoorListener.OnPlayerBeginsExit -= OnBeginExit;
    OverrideChangeEvent -= ApplyState;
  }
}
