using System;
using Cofe.Meta;
using Engine.Source.Commons;

namespace Engine.Source.Services.Consoles.Binds
{
  [Initialisable]
  public class BindSystemConsoleCommand
  {
    [ConsoleCommand("throw")]
    private static string Throw(string command, ConsoleParameter[] parameters)
    {
      GameObject gameObject = new GameObject("ThrowScript", new Type[1]
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
