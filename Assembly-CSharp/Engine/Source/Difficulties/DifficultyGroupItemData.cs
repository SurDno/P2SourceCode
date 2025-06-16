using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Inspectors;

namespace Engine.Source.Difficulties
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class DifficultyGroupItemData
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Header = true, Mutable = true, Mode = ExecuteMode.Edit)]
    public string Name;
  }
}
