// Decompiled with JetBrains decompiler
// Type: Engine.Source.Debugs.HerbRootsGroupDebug
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Meta;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Services.Gizmos;
using Engine.Source.Utility;
using System;
using UnityEngine;

#nullable disable
namespace Engine.Source.Debugs
{
  [Initialisable]
  public static class HerbRootsGroupDebug
  {
    private static string name = "[HerbRoots]";
    private static KeyCode key = KeyCode.H;
    private static KeyModifficator modifficators = KeyModifficator.Control | KeyModifficator.Shift;
    private static Color headerColor = Color.green;
    private static Color trueColor = Color.white;
    private static Color falseColor = ColorPreset.LightGray;

    public static bool IsGroupVisible => GroupDebugService.IsGroupVisible(HerbRootsGroupDebug.name);

    [Cofe.Meta.Initialise]
    private static void Initialise()
    {
      InstanceByRequest<EngineApplication>.Instance.OnInitialized += (Action) (() => GroupDebugService.RegisterGroup(HerbRootsGroupDebug.name, HerbRootsGroupDebug.key, HerbRootsGroupDebug.modifficators, new Action(HerbRootsGroupDebug.Update)));
    }

    public static void DrawHeader()
    {
      string text = "\n" + HerbRootsGroupDebug.name + " (" + InputUtility.GetHotKeyText(HerbRootsGroupDebug.key, HerbRootsGroupDebug.modifficators) + ")";
      ServiceLocator.GetService<GizmoService>().DrawText(text, HerbRootsGroupDebug.headerColor);
    }

    private static void Update()
    {
    }
  }
}
