// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.Consoles.Binds.BindSettingsConsoleCommands
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Meta;
using System;

#nullable disable
namespace Engine.Source.Services.Consoles.Binds
{
  [Initialisable]
  public static class BindSettingsConsoleCommands
  {
    [Cofe.Meta.Initialise]
    private static void Initialise()
    {
      ConsoleTargetService.AddTarget(typeof (GameSettingsData).Name, (Func<string, object>) (value => (object) ScriptableObjectInstance<GameSettingsData>.Instance));
      ConsoleTargetService.AddTarget(typeof (InputSettingsData).Name, (Func<string, object>) (value => (object) ScriptableObjectInstance<InputSettingsData>.Instance));
      ConsoleTargetService.AddTarget(typeof (BuildSettings).Name, (Func<string, object>) (value => (object) ScriptableObjectInstance<BuildSettings>.Instance));
    }
  }
}
