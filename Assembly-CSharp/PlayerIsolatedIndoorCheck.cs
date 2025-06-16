using Engine.Common.Services;
using Engine.Impl.UI.Controls;
using Engine.Source.Services;
using System;
using UnityEngine;

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
    PlayerIsolatedIndoorCheck.overriden = true;
    PlayerIsolatedIndoorCheck.overridenIsolatedIndoor = value;
    Action overrideChangeEvent = PlayerIsolatedIndoorCheck.OverrideChangeEvent;
    if (overrideChangeEvent == null)
      return;
    overrideChangeEvent();
  }

  public static void ResetOverride()
  {
    PlayerIsolatedIndoorCheck.overriden = false;
    Action overrideChangeEvent = PlayerIsolatedIndoorCheck.OverrideChangeEvent;
    if (overrideChangeEvent == null)
      return;
    overrideChangeEvent();
  }

  private void ApplyState()
  {
    if (!((UnityEngine.Object) this.view != (UnityEngine.Object) null))
      return;
    this.view.Visible = PlayerIsolatedIndoorCheck.overriden ? PlayerIsolatedIndoorCheck.overridenIsolatedIndoor : this.isolatedIndoor;
  }

  private void SetIsolatedIndoor(bool value)
  {
    this.isolatedIndoor = value;
    this.ApplyState();
  }

  private void OnBeginExit() => this.SetIsolatedIndoor(false);

  protected override void OnConnectToEngine()
  {
    this.SetIsolatedIndoor(this.insideIndoorListener.IsolatedIndoor);
    this.insideIndoorListener.OnIsolatedIndoorChanged += new Action<bool>(this.SetIsolatedIndoor);
    this.insideIndoorListener.OnPlayerBeginsExit += new Action(this.OnBeginExit);
    PlayerIsolatedIndoorCheck.OverrideChangeEvent += new Action(this.ApplyState);
  }

  protected override void OnDisconnectFromEngine()
  {
    this.insideIndoorListener.OnIsolatedIndoorChanged -= new Action<bool>(this.SetIsolatedIndoor);
    this.insideIndoorListener.OnPlayerBeginsExit -= new Action(this.OnBeginExit);
    PlayerIsolatedIndoorCheck.OverrideChangeEvent -= new Action(this.ApplyState);
  }
}
