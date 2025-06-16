using Cofe.Meta;

namespace Engine.Source.Services.Consoles.Binds
{
  [Initialisable]
  public static class BindSettingsConsoleCommands
  {
    [Initialise]
    private static void Initialise()
    {
      ConsoleTargetService.AddTarget(typeof (GameSettingsData).Name, value => ScriptableObjectInstance<GameSettingsData>.Instance);
      ConsoleTargetService.AddTarget(typeof (InputSettingsData).Name, value => ScriptableObjectInstance<InputSettingsData>.Instance);
      ConsoleTargetService.AddTarget(typeof (BuildSettings).Name, value => ScriptableObjectInstance<BuildSettings>.Instance);
    }
  }
}
