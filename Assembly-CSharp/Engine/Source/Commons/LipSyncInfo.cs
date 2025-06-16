using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Connections;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Commons
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class LipSyncInfo
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    public UnityAsset<AudioClip> Clip;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mode = ExecuteMode.EditAndRuntime)]
    public byte[] Data;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mode = ExecuteMode.EditAndRuntime)]
    public string Tag;
  }
}
