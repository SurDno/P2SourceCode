using System.Reflection;
using Cofe.Meta;

namespace Inspectors;

[Initialisable]
public static class MethodInspectedDrawers {
	[Initialise]
	private static void Initialise() {
		InspectedDrawerService.AddConditional(type => typeof(IMethodDrawer).IsAssignableFrom(type),
			(name, type, value, mutable, context, drawer, target, member, setter) => {
				var field = type.GetField("Instance",
					BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
				if (!(field != null) || !(field.GetValue(null) is IMethodDrawer methodDrawer2))
					return;
				methodDrawer2.DrawInspected(name, type, value, mutable, context, drawer, setter);
			});
	}
}