// Decompiled with JetBrains decompiler
// Type: CutsceneIndoorCheck
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Impl.UI.Controls;
using System;
using UnityEngine;

#nullable disable
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
