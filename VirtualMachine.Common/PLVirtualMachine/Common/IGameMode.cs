using PLVirtualMachine.Common.EngineAPI;

namespace PLVirtualMachine.Common;

public interface IGameMode : IObject, IEditorBaseTemplate {
	string Name { get; }

	bool IsMain { get; }

	GameTime StartGameTime { get; }

	GameTime StartSolarTime { get; }

	float GameTimeSpeed { get; }

	float SolarTimeSpeed { get; }

	CommonVariable PlayCharacterVariable { get; }
}