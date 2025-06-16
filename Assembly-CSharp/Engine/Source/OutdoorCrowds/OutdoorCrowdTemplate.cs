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
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Header = true, Mode = ExecuteMode.EditAndRuntime)]
    public string Name;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Header = true, Mode = ExecuteMode.EditAndRuntime)]
    public Typed<IEntity> Template;
    [DataReadProxy(MemberEnum.OnlyCopy)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.OnlyCopy)]
    [Inspected(Header = true, Mode = ExecuteMode.EditAndRuntime)]
    public OutdoorCrowdTemplateCount Day = ProxyFactory.Create<OutdoorCrowdTemplateCount>();
    [DataReadProxy(MemberEnum.OnlyCopy)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.OnlyCopy)]
    [Inspected(Header = true, Mode = ExecuteMode.EditAndRuntime)]
    public OutdoorCrowdTemplateCount Night = ProxyFactory.Create<OutdoorCrowdTemplateCount>();
  }
}
