using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Inspectors;

namespace Assets.Engine.Source.Services.Profiles;

[Factory]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class CustomProfileData {
	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected]
	public string Name = "";

	[DataReadProxy] [DataWriteProxy] [CopyableProxy()] [Inspected]
	public string Value = "";
}