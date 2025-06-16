using System;
using Engine.Common.Components;
using Engine.Common.Generator;

namespace Engine.Source.Components.Saves;

[GenerateProxy(TypeEnum.StateSave | TypeEnum.StateLoad)]
public class StorableData {
	[StateSaveProxy(MemberEnum.CustomReference)] [StateLoadProxy(MemberEnum.CustomReference)]
	public IStorageComponent Storage;

	[StateSaveProxy] [StateLoadProxy()] public Guid TemplateId;
}