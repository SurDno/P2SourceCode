using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Components
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class PositionComponent : EngineComponent
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected(Mode = ExecuteMode.EditAndRuntime)]
    protected Vector3 position;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [Inspected(Mode = ExecuteMode.EditAndRuntime)]
    protected Vector3 rotation;

    public Vector3 Position
    {
      get => position;
      set => position = value;
    }

    public Vector3 Rotation
    {
      get => rotation;
      set => rotation = value;
    }

    public override void OnAdded()
    {
      base.OnAdded();
      ((IEntityView) Owner).Position = position;
      ((IEntityView) Owner).Rotation = Quaternion.Euler(rotation);
    }
  }
}
