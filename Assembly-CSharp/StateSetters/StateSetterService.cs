using System.Collections.Generic;
using Cofe.Utility;
using UnityEngine;

namespace StateSetters;

public static class StateSetterService {
	private static Dictionary<string, IStateSetterItemController> controllers = new();

	public static void Register(string id, IStateSetterItemController controller) {
		controllers.Add(id, controller);
	}

	public static void Apply(StateSetterItem item, float value) {
		var type = item.Type;
		if (type.IsNullOrEmpty())
			return;
		IStateSetterItemController setterItemController;
		if (controllers.TryGetValue(type, out setterItemController))
			setterItemController.Apply(item, value);
		else
			Debug.LogError("Controller not found : " + item.Type);
	}
}