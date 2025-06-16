// Decompiled with JetBrains decompiler
// Type: Engine.Source.OutdoorCrowds.OutdoorCrowdTemplate
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Engine.Common;
using Engine.Common.Generator;
using Engine.Source.Connections;
using Inspectors;

#nullable disable
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
