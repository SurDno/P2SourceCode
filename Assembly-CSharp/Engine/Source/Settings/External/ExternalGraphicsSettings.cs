using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Inspectors;

namespace Engine.Source.Settings.External;

[Factory]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class ExternalGraphicsSettings : ExternalSettingsInstance<ExternalGraphicsSettings> {
	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
	public float MindMapIconSize;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
	public float Fov;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
	public float MinFov;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
	public float MaxFov;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
	public float MinLevelOfDetails;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
	public float MaxLevelOfDetails;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
	public float MinShadowDistance;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
	public float MaxShadowDistance;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy()] [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
	public bool StreamingMipmapsActive;
}