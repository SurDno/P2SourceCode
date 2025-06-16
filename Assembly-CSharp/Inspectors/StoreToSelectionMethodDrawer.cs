using System;

namespace Inspectors;

public class StoreToSelectionMethodDrawer : BaseMethodDrawer<StoreToSelectionMethodDrawer> {
	public override void DrawInspected(
		string name,
		Type type,
		object value,
		bool mutable,
		IInspectedProvider context,
		IInspectedDrawer drawer,
		Action<object> setter) {
		if (!drawer.ButtonField(name))
			return;
		var menu = drawer.CreateMenu();
		for (var index = 0; index < 10; ++index) {
			var storeIndex = index;
			menu.AddItem("Store Object To/Slot " + storeIndex, false, (Action)(() => {
				var action = setter;
				if (action == null)
					return;
				action(new object[1] {
					storeIndex
				});
			}));
		}

		menu.Show();
	}
}