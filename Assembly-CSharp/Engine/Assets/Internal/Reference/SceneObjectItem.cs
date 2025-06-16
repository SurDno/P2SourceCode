using Engine.Common;
using Engine.Common.Generator;
using Engine.Source.Connections;
using Inspectors;
using System;
using System.Collections.Generic;

namespace Engine.Assets.Internal.Reference
{
  [GenerateProxy(TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class SceneObjectItem
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [Inspected(Header = true, Mode = ExecuteMode.EditAndRuntime)]
    public Guid Id;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [Inspected(Mode = ExecuteMode.EditAndRuntime)]
    public string PreserveName = "";
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [Inspected(Mode = ExecuteMode.EditAndRuntime)]
    public Typed<IEntity> Origin;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [Inspected(Mode = ExecuteMode.EditAndRuntime)]
    public Typed<IEntity> Template;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [Inspected(Mode = ExecuteMode.EditAndRuntime)]
    public List<SceneObjectItem> Items = new List<SceneObjectItem>();
  }
}
