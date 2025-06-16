using Engine.Common;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Connections;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Commons
{
  [Factory(typeof (IModel))]
  [GenerateProxy(TypeEnum.Copyable | TypeEnum.EngineCloneable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class Model : EngineObject, IModel, IObject
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Name = "Prefab", Mutable = true, Mode = ExecuteMode.Edit)]
    protected UnityAsset<GameObject> connection;

    [Inspected]
    public UnityAsset<GameObject> Connection => this.connection;
  }
}
