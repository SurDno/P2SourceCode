using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Connections;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Commons
{
  [Factory(typeof (IMapTooltipResource))]
  [GenerateProxy(TypeEnum.Copyable | TypeEnum.EngineCloneable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class MapTooltipResource : EngineObject, IMapTooltipResource, IObject
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected UnityAsset<Texture> image;

    public UnityAsset<Texture> Image => this.image;
  }
}
