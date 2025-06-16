namespace Engine.Common.Services;

public interface IForcedDialogService {
	void AddForcedDialog(IEntity character, float distance);

	void RemoveForcedDialog(IEntity character);
}