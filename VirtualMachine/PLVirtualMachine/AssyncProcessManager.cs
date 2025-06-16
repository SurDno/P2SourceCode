using System;
using System.Collections.Generic;
using PLVirtualMachine.Dynamic;

namespace PLVirtualMachine;

public static class AssyncProcessManager {
	private static List<IAssyncUpdateable> updateableObjectsList = new();

	public static void RegistrAssyncUpdateableObject(IAssyncUpdateable updateableObj) {
		updateableObjectsList.Add(updateableObj);
	}

	public static void Update(TimeSpan delta) {
		for (var index = 0; index < updateableObjectsList.Count; ++index)
			if (updateableObjectsList[index].Active)
				updateableObjectsList[index].Update(delta);
	}

	public static void Clear() {
		for (var index = 0; index < updateableObjectsList.Count; ++index)
			updateableObjectsList[index].Clear();
		updateableObjectsList.Clear();
	}
}