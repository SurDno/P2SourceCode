using System.Collections.Generic;
using PLVirtualMachine.Common;

namespace PLVirtualMachine.Base;

public static class VMBaseObjectUtility {
	public static bool CheckOrders<T>(List<T> orderedChilsList) where T : class, IOrderedChild {
		if (orderedChilsList.Count > 1)
			for (var index = 0; index < orderedChilsList.Count - 1; ++index)
				if (orderedChilsList[index].Order >= orderedChilsList[index + 1].Order)
					return false;
		return true;
	}
}