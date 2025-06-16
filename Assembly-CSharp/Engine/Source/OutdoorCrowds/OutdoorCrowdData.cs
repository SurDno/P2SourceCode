using System.Collections.Generic;
using Engine.Common.Components.Regions;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Inspectors;

namespace Engine.Source.OutdoorCrowds
{
  [Factory]
  [GenerateProxy(TypeEnum.Copyable | TypeEnum.EngineCloneable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class OutdoorCrowdData : EngineObject
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    public string TableName;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected(Mode = ExecuteMode.EditAndRuntime)]
    public RegionEnum Region;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected(Mode = ExecuteMode.EditAndRuntime)]
    public List<OutdoorCrowdLayout> Layouts = new List<OutdoorCrowdLayout>();
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [Inspected(Mode = ExecuteMode.EditAndRuntime)]
    public List<OutdoorCrowdTemplates> Templates = new List<OutdoorCrowdTemplates>();
  }
}
