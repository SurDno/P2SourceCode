using Engine.Common.Services;
using Engine.Impl.UI.Controls;
using Engine.Source.Services;
using System;
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
    PlayerIndoorCheck.overriden = true;
    PlayerIndoorCheck.overridenInsideIndoor = value;
    Action overrideChangeEvent = PlayerIndoorCheck.OverrideChangeEvent;
    if (overrideChangeEvent == null)
      return;
    overrideChangeEvent();
  }

  public static void ResetOverride()
  {
    PlayerIndoorCheck.overriden = false;
    Action overrideChangeEvent = PlayerIndoorCheck.OverrideChangeEvent;
    if (overrideChangeEvent == null)
      return;
    overrideChangeEvent();
  }

  private void ApplyState()
  {
    if (!((UnityEngine.Object) this.view != (UnityEngine.Object) null))
      return;
    this.view.Visible = PlayerIndoorCheck.overriden ? PlayerIndoorCheck.overridenInsideIndoor : this.insideIndoor;
  }

  private void SetInsideIndoor(bool value)
  {
    this.insideIndoor = value;
    this.ApplyState();
  }

  private void OnBeginExit() => this.SetInsideIndoor(false);

  protected override void OnConnectToEngine()
  {
    this.SetInsideIndoor(this.insideIndoorListener.InsideIndoor);
    this.insideIndoorListener.OnInsideIndoorChanged += new Action<bool>(this.SetInsideIndoor);
    this.insideIndoorListener.OnPlayerBeginsExit += new Action(this.OnBeginExit);
    PlayerIndoorCheck.OverrideChangeEvent += new Action(this.ApplyState);
  }

  protected override void OnDisconnectFromEngine()
  {
    this.insideIndoorListener.OnInsideIndoorChanged -= new Action<bool>(this.SetInsideIndoor);
    this.insideIndoorListener.OnPlayerBeginsExit -= new Action(this.OnBeginExit);
    PlayerIndoorCheck.OverrideChangeEvent -= new Action(this.ApplyState);
  }
}
