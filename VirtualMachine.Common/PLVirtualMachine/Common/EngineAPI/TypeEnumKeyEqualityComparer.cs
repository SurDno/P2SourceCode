// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.EngineAPI.TypeEnumKeyEqualityComparer
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using System.Collections.Generic;

#nullable disable
namespace PLVirtualMachine.Common.EngineAPI
{
  public class TypeEnumKeyEqualityComparer : IEqualityComparer<TypeEnumKey>
  {
    public static readonly TypeEnumKeyEqualityComparer Instance = new TypeEnumKeyEqualityComparer();

    public bool Equals(TypeEnumKey x, TypeEnumKey y) => x.Type == y.Type && x.Int == y.Int;

    public int GetHashCode(TypeEnumKey obj) => obj.Type.GetHashCode() ^ obj.Int.GetHashCode();
  }
}
