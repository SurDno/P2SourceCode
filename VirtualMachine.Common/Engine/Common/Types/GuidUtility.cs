// Decompiled with JetBrains decompiler
// Type: Engine.Common.Types.GuidUtility
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using PLVirtualMachine.Common;
using System;

#nullable disable
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
