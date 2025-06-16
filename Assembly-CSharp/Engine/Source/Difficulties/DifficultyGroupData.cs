using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Inspectors;
using System.Collections.Generic;

namespace Engine.Source.Difficulties
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class DifficultyGroupData
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Header = true, Mutable = true, Mode = ExecuteMode.Edit)]
    public string Name;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    public List<DifficultyGroupItemData> Items = new List<DifficultyGroupItemData>();
  }
}
