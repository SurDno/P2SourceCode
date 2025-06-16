using System.Collections.Generic;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services.Factories;
using Inspectors;

namespace Engine.Source.Commons;

[Factory]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class LipSyncLanguage {
	[DataReadProxy]
	[DataWriteProxy]
	[CopyableProxy]
	[Inspected(Header = true)]
	[Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	public LanguageEnum Language;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy()] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	public List<LipSyncInfo> LipSyncs = new();
}