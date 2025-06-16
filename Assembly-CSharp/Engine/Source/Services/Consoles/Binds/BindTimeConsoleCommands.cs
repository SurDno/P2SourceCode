using Cofe.Meta;
using Engine.Common.Services;
using Engine.Impl.Services;
using System;

namespace Engine.Source.Services.Consoles.Binds
{
  [Initialisable]
  public static class BindTimeConsoleCommands
  {
    [Cofe.Meta.Initialise]
    private static void Initialise()
    {
      ConsoleTargetService.AddTarget(typeof (TimeService).Name, (Func<string, object>) (value => (object) ServiceLocator.GetService<ITimeService>()));
      SetConsoleCommand.AddBind<TimeService, TimeSpan>("solar_time", false, (Action<TimeService, TimeSpan>) ((target, value) => target.SolarTime = value));
      GetConsoleCommand.AddBind<TimeService, TimeSpan>("solar_time", true, (Func<TimeService, TimeSpan>) (target => target.SolarTime));
      SetConsoleCommand.AddBind<TimeService, TimeSpan>("game_time", false, (Action<TimeService, TimeSpan>) ((target, value) => target.SetGameTime(value)));
      GetConsoleCommand.AddBind<TimeService, TimeSpan>("game_time", true, (Func<TimeService, TimeSpan>) (target => target.GameTime));
    }
  }
}
