using Cofe.Meta;
using Cofe.Serializations.Converters;
using Engine.Common.Services;
using Engine.Services;

namespace Engine.Source.Services.Consoles.Binds;

[Initialisable]
public static class CommonTargets {
	[Initialise]
	private static void RegisterTargets() {
		ConsoleTargetService.AddTarget("-player", value => ServiceLocator.GetService<ISimulation>().Player);
		ConsoleTargetService.AddTarget("-slot",
			value => ServiceLocator.GetService<SelectionService>().GetSelection(DefaultConverter.ParseInt(value)));
	}
}