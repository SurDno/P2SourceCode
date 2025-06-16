using Cofe.Proxies;
using Engine.Common;
using Engine.Common.Generator;
using Engine.Source.Connections;
using Inspectors;

namespace Engine.Source.OutdoorCrowds
{
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class OutdoorCrowdTemplate
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected(Header = true, Mode = ExecuteMode.EditAndRuntime)]
    public string Name;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected(Header = true, Mode = ExecuteMode.EditAndRuntime)]
    public Typed<IEntity> Template;
    [DataReadProxy(MemberEnum.OnlyCopy)]
    [DataWriteProxy]
    [CopyableProxy(MemberEnum.OnlyCopy)]
    [Inspected(Header = true, Mode = ExecuteMode.EditAndRuntime)]
    public OutdoorCrowdTemplateCount Day = ProxyFactory.Create<OutdoorCrowdTemplateCount>();
    [DataReadProxy(MemberEnum.OnlyCopy)]
    [DataWriteProxy()]
    [CopyableProxy(MemberEnum.OnlyCopy)]
    [Inspected(Header = true, Mode = ExecuteMode.EditAndRuntime)]
    public OutdoorCrowdTemplateCount Night = ProxyFactory.Create<OutdoorCrowdTemplateCount>();
  }
}
