// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.Consoles.Binds.BindSystemConsoleCommand
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Meta;
using Engine.Source.Commons;
using UnityEngine;

#nullable disable
namespace Engine.Source.Services.Consoles.Binds
{
  [Initialisable]
  public class BindSystemConsoleCommand
  {
    [ConsoleCommand("throw")]
    private static string Throw(string command, ConsoleParameter[] parameters)
    {
      GameObject gameObject = new GameObject("ThrowScript", new System.Type[1]
      {
        typeof (ThrowScript)
      });
      return command;
    }

    [ConsoleCommand("exit")]
    private static string Exit(string command, ConsoleParameter[] parameters)
    {
      InstanceByRequest<EngineApplication>.Instance.Exit();
      return command;
    }
  }
}
