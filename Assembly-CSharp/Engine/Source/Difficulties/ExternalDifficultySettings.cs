using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Settings.External;
using Inspectors;
using System.Collections.Generic;

namespace Engine.Source.Difficulties
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class ExternalDifficultySettings : ExternalSettingsInstance<ExternalDifficultySettings>
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    public List<DifficultyItemData> Items = new List<DifficultyItemData>();
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    public List<DifficultyGroupData> Groups = new List<DifficultyGroupData>();
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    public List<DifficultyPresetData> Presets = new List<DifficultyPresetData>();
  }
}
