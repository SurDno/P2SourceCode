using PLVirtualMachine.Common;
using System.Collections.Generic;

namespace PLVirtualMachine.Base
{
  public static class VMBaseObjectUtility
  {
    public static bool CheckOrders<T>(List<T> orderedChilsList) where T : class, IOrderedChild
    {
      if (orderedChilsList.Count > 1)
      {
        for (int index = 0; index < orderedChilsList.Count - 1; ++index)
        {
          if (orderedChilsList[index].Order >= orderedChilsList[index + 1].Order)
            return false;
        }
      }
      return true;
    }
  }
}
