namespace Engine.Common.Components;

public interface IMessangerComponent : IComponent {
	void StartTeleporting();

	void StopTeleporting();
}