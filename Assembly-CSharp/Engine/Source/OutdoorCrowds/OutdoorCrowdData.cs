using Engine.Common.Components.Regions;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Inspectors;
using System.Collections.Generic;

namespace Engine.Source.OutdoorCrowds
{
  [Factory]
  [GenerateProxy(TypeEnum.Copyable | TypeEnum.EngineCloneable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class OutdoorCrowdData : EngineObject
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    public string TableName;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mode = ExecuteMode.EditAndRuntime)]
    public RegionEnum Region;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mode = ExecuteMode.EditAndRuntime)]
    public List<OutdoorCrowdLayout> Layouts = new List<OutdoorCrowdLayout>();
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mode = ExecuteMode.EditAndRuntime)]
    public List<OutdoorCrowdTemplates> Templates = new List<OutdoorCrowdTemplates>();
  }
}
