using System;
using System.Collections.Generic;
using System.Linq;
using Cofe.Meta;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Services;

namespace Engine.Source.Services.Consoles.Binds;

[Initialisable]
public static class BindStorableConsoleCommands {
	[Initialise]
	private static void Initialise() {
		ConsoleTargetService.AddTarget("-storable", value => Storables().FirstOrDefault(o => o.Owner.Name == value));
		EnumConsoleCommand.AddBind("-storable", (Func<string>)(() => {
			var list = Storables().ToList();
			list.Sort((a, b) => a.Owner.Name.CompareTo(b.Owner.Name));
			var str = "\nStorables :\n";
			foreach (var storableComponent in list)
				str = str + storableComponent.Owner.Name + "\n";
			return str;
		}));
	}

	[ConsoleCommand("add_storable")]
	private static string StorableCommand(string command, ConsoleParameter[] parameters) {
		if (parameters.Length == 0 || (parameters.Length == 1 && parameters[0].Value == "?"))
			return command + " [-player | -slot:index] -storable:name";
		IStorageComponent target1;
		IStorableComponent target2;
		if (parameters.Length == 1) {
			target1 =
				ConsoleTargetService.GetTarget(typeof(IStorageComponent), ConsoleParameter.Empty) as IStorageComponent;
			target2 = ConsoleTargetService.GetTarget(typeof(IStorableComponent), parameters[0]) as IStorableComponent;
		} else {
			if (parameters.Length != 2)
				return "Error parameter count";
			target1 = ConsoleTargetService.GetTarget(typeof(IStorageComponent), parameters[0]) as IStorageComponent;
			target2 = ConsoleTargetService.GetTarget(typeof(IStorableComponent), parameters[1]) as IStorableComponent;
		}

		if (target1 == null)
			return typeof(IStorageComponent).Name + " not found";
		if (target2 == null)
			return typeof(IStorableComponent).Name + " not found";
		var entity = ServiceLocator.GetService<IFactory>().Instantiate(target2.Owner);
		ServiceLocator.GetService<ISimulation>().Add(entity, ServiceLocator.GetService<ISimulation>().Storables);
		var component = entity.GetComponent<IStorableComponent>();
		target1.AddItem(component, null);
		return "Add item : " + target2.Owner.Name + " , to storage : " + target1.Owner.Name;
	}

	private static IEnumerable<IStorableComponent> Storables() {
		return ServiceLocator.GetService<ITemplateService>().GetTemplates<IEntity>()
			.Select(o => o.GetComponent<IStorableComponent>()).Where(o => o != null);
	}
}