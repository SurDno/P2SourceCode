using Cofe.Meta;
using System;

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
