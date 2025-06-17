using System;
using System.Collections.Generic;
using Engine.Common;
using Engine.Common.Generator;
using Engine.Source.Connections;
using Inspectors;

namespace Engine.Assets.Internal.Reference
{
  [GenerateProxy(TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class SceneObjectItem
  {
    [DataReadProxy]
    [DataWriteProxy]
    [Inspected(Header = true, Mode = ExecuteMode.EditAndRuntime)]
    public Guid Id;
    [DataReadProxy]
    [DataWriteProxy]
    [Inspected(Mode = ExecuteMode.EditAndRuntime)]
    public string PreserveName = "";
    [DataReadProxy]
    [DataWriteProxy]
    [Inspected(Mode = ExecuteMode.EditAndRuntime)]
    public Typed<IEntity> Origin;
    [DataReadProxy]
    [DataWriteProxy]
    [Inspected(Mode = ExecuteMode.EditAndRuntime)]
    public Typed<IEntity> Template;
    [DataReadProxy]
    [DataWriteProxy()]
    [Inspected(Mode = ExecuteMode.EditAndRuntime)]
    public List<SceneObjectItem> Items = [];
  }
}
