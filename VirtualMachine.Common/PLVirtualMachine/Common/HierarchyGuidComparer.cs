// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.HierarchyGuidComparer
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using System.Collections.Generic;

#nullable disable
namespace PLVirtualMachine.Common
{
  public class HierarchyGuidComparer : IEqualityComparer<HierarchyGuid>
  {
    public static readonly HierarchyGuidComparer Instance = new HierarchyGuidComparer();

    public bool Equals(HierarchyGuid x, HierarchyGuid y) => x == y;

    public int GetHashCode(HierarchyGuid obj) => obj.GetHashCode();
  }
}
