// Decompiled with JetBrains decompiler
// Type: Engine.Source.Debugs.ConsoleGroupDebug
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Meta;
using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Services.Consoles;
using Engine.Source.Utility;
using System;
using UnityEngine;

#nullable disable
namespace Engine.Source.Debugs
{
  [Initialisable]
  public static class ConsoleGroupDebug
  {
    [Cofe.Meta.Initialise]
    private static void Initialise()
    {
      InstanceByRequest<EngineApplication>.Instance.OnInitialized += (Action) (() => InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable((IUpdatable) new UpdatableProxy((Action) (() => ConsoleGroupDebug.Update()))));
    }

    private static void Update()
    {
      ConsoleGroupDebug.TryExecByHotKey(KeyCode.Q);
      ConsoleGroupDebug.TryExecByHotKey(KeyCode.W);
      ConsoleGroupDebug.TryExecByHotKey(KeyCode.E);
      ConsoleGroupDebug.TryExecByHotKey(KeyCode.R);
      ConsoleGroupDebug.TryExecByHotKey(KeyCode.T);
      ConsoleGroupDebug.TryExecByHotKey(KeyCode.Y);
      ConsoleGroupDebug.TryExecByHotKey(KeyCode.U);
      ConsoleGroupDebug.TryExecByHotKey(KeyCode.I);
      ConsoleGroupDebug.TryExecByHotKey(KeyCode.O);
      ConsoleGroupDebug.TryExecByHotKey(KeyCode.P);
    }

    private static void TryExecByHotKey(KeyCode key)
    {
      if (!InputUtility.IsKeyDown(key, KeyModifficator.Control | KeyModifficator.Shift))
        return;
      ServiceLocator.GetService<ConsoleService>().ExecuteCommand("exec hotkey_" + key.ToString().ToLowerInvariant() + ".txt");
    }
  }
}
