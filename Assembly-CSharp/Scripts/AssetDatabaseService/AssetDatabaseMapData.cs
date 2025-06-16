using System.Collections.Generic;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Inspectors;

namespace Scripts.AssetDatabaseService
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class AssetDatabaseMapData
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [Inspected]
    public List<AssetDatabaseMapItemData> Items = new List<AssetDatabaseMapItemData>();
  }
}
