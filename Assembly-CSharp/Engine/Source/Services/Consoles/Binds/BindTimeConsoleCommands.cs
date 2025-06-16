using System;
using Cofe.Meta;
using Engine.Common.Services;
using Engine.Impl.Services;

namespace Engine.Source.Services.Consoles.Binds
{
  [Initialisable]
  public static class BindTimeConsoleCommands
  {
    [Initialise]
    private static void Initialise()
    {
      ConsoleTargetService.AddTarget(typeof (TimeService).Name, value => ServiceLocator.GetService<ITimeService>());
      SetConsoleCommand.AddBind("solar_time", false, (Action<TimeService, TimeSpan>) ((target, value) => target.SolarTime = value));
      GetConsoleCommand.AddBind("solar_time", true, (Func<TimeService, TimeSpan>) (target => target.SolarTime));
      SetConsoleCommand.AddBind("game_time", false, (Action<TimeService, TimeSpan>) ((target, value) => target.SetGameTime(value)));
      GetConsoleCommand.AddBind("game_time", true, (Func<TimeService, TimeSpan>) (target => target.GameTime));
    }
  }
}
