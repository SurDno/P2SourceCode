using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Inspectors;
using System.Collections.Generic;

namespace Scripts.AssetDatabaseService
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class AssetDatabaseMapData
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    public List<AssetDatabaseMapItemData> Items = new List<AssetDatabaseMapItemData>();
  }
}
