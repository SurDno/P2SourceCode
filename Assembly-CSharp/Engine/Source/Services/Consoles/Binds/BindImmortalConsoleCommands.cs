using Cofe.Meta;
using Engine.Common.Components.Parameters;
using Engine.Source.Components;

namespace Engine.Source.Services.Consoles.Binds;

[Initialisable]
public static class BindImmortalConsoleCommands {
	[Initialise]
	private static void Initialise() {
		SetConsoleCommand.AddBind<ParametersComponent, bool>("immortal", true, (target, value) => {
			var byName = target.GetByName<bool>(ParameterNameEnum.Immortal);
			if (byName == null)
				return;
			byName.Value = value;
		});
		GetConsoleCommand.AddBind<ParametersComponent, bool>("immortal", true, target => {
			var byName = target.GetByName<bool>(ParameterNameEnum.Immortal);
			return byName != null && byName.Value;
		});
	}
}