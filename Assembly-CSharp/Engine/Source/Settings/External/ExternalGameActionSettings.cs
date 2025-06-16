using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Services.Inputs;
using Inspectors;
using System.Collections.Generic;

namespace Engine.Source.Settings.External
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class ExternalGameActionSettings : ExternalSettingsInstance<ExternalGameActionSettings>
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    public List<ActionGroup> Groups_Set_1 = new List<ActionGroup>();
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    public List<ActionGroup> Groups_Set_2 = new List<ActionGroup>();
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    public List<ActionGroup> Groups_Set_3 = new List<ActionGroup>();
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    public List<JoystickLayout> Layouts = new List<JoystickLayout>();
  }
}
