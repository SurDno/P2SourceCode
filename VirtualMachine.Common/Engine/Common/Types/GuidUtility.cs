﻿using System;
using PLVirtualMachine.Common;

namespace Engine.Common.Types
{
  public static class GuidUtility
  {
    public static ushort GetTypeId(ulong value) => (ushort) (value / 281470681677825UL);

    public static EGuidFormat GetGuidFormat(string guidStr)
    {
      if (ulong.TryParse(guidStr, out ulong _))
        return EGuidFormat.GT_BASE;
      Guid result = Guid.Empty;
      if (Guid.TryParse(guidStr, out result))
        return EGuidFormat.GT_ENGINE;
      return HierarchyGuid.TryParse(guidStr, out HierarchyGuid _) ? EGuidFormat.GT_HIERARCHY : EGuidFormat.GT_NOTGUID;
    }

    public static string GetGuidString(Guid value) => value.ToString("N");
  }
}
