using Engine.Common;
using Engine.Common.Generator;
using Engine.Common.MindMap;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Connections;
using Inspectors;
using UnityEngine;

namespace Engine.Impl.MindMap
{
  [Factory(typeof (IMMPlaceholder))]
  [GenerateProxy(TypeEnum.Copyable | TypeEnum.EngineCloneable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class MMPlaceholder : EngineObject, IMMPlaceholder, IObject
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected UnityAsset<Texture> image;

    public UnityAsset<Texture> Image => image;
  }
}
