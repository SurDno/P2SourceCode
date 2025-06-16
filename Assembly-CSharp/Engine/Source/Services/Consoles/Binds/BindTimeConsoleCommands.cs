using System;
using Cofe.Meta;
using Engine.Common.Services;
using Engine.Impl.Services;

namespace Engine.Source.Services.Consoles.Binds;

[Initialisable]
public static class BindTimeConsoleCommands {
	[Initialise]
	private static void Initialise() {
		ConsoleTargetService.AddTarget(typeof(TimeService).Name, value => ServiceLocator.GetService<ITimeService>());
		SetConsoleCommand.AddBind<TimeService, TimeSpan>("solar_time", false,
			(target, value) => target.SolarTime = value);
		GetConsoleCommand.AddBind<TimeService, TimeSpan>("solar_time", true, target => target.SolarTime);
		SetConsoleCommand.AddBind<TimeService, TimeSpan>("game_time", false,
			(target, value) => target.SetGameTime(value));
		GetConsoleCommand.AddBind<TimeService, TimeSpan>("game_time", true, target => target.GameTime);
	}
}