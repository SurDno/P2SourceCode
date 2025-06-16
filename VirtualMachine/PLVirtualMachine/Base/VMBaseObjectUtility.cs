// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Base.VMBaseObjectUtility
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using PLVirtualMachine.Common;
using System.Collections.Generic;

#nullable disable
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
