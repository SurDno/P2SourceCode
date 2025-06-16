using Engine.Impl.UI.Controls;
using System;
using UnityEngine;

public class CutsceneIndoorCheck : MonoBehaviour
{
  private static bool overridenInsideIndoor;
  [SerializeField]
  private HideableView view;

  private static event Action OverrideChangeEvent;

  public static void Set(bool value)
  {
    if (CutsceneIndoorCheck.overridenInsideIndoor == value)
      return;
    CutsceneIndoorCheck.overridenInsideIndoor = value;
    Action overrideChangeEvent = CutsceneIndoorCheck.OverrideChangeEvent;
    if (overrideChangeEvent == null)
      return;
    overrideChangeEvent();
  }

  private void ApplyState()
  {
    if (!((UnityEngine.Object) this.view != (UnityEngine.Object) null))
      return;
    this.view.Visible = CutsceneIndoorCheck.overridenInsideIndoor;
  }

  private void OnEnable()
  {
    this.ApplyState();
    CutsceneIndoorCheck.OverrideChangeEvent += new Action(this.ApplyState);
  }

  private void OnDisable()
  {
    CutsceneIndoorCheck.OverrideChangeEvent -= new Action(this.ApplyState);
  }
}
