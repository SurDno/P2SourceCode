using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Inspectors;

namespace Scripts.AssetDatabaseService
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class AssetDatabaseMapItemData
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected]
    public string Id;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [Inspected]
    public string Name;
  }
}
