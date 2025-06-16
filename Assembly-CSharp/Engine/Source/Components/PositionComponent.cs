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
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mode = ExecuteMode.EditAndRuntime)]
    protected Vector3 position;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mode = ExecuteMode.EditAndRuntime)]
    protected Vector3 rotation;

    public Vector3 Position
    {
      get => this.position;
      set => this.position = value;
    }

    public Vector3 Rotation
    {
      get => this.rotation;
      set => this.rotation = value;
    }

    public override void OnAdded()
    {
      base.OnAdded();
      ((IEntityView) this.Owner).Position = this.position;
      ((IEntityView) this.Owner).Rotation = Quaternion.Euler(this.rotation);
    }
  }
}
