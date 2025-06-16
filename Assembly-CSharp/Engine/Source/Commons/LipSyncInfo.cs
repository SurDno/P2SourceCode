using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Connections;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Commons;

[Factory]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class LipSyncInfo {
	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	public UnityAsset<AudioClip> Clip;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected(Mode = ExecuteMode.EditAndRuntime)]
	public byte[] Data;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy()] [Inspected(Mode = ExecuteMode.EditAndRuntime)]
	public string Tag;
}