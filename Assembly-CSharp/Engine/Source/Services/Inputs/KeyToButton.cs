using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using InputServices;
using Inspectors;

namespace Engine.Source.Services.Inputs;

[Factory]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class KeyToButton {
	[DataReadProxy]
	[DataWriteProxy]
	[CopyableProxy]
	[Inspected(Header = true, Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
	public string Name;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy()] [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
	public JoystickKeyCode KeyCode;
}