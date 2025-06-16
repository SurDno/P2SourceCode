namespace Engine.Common.Services;

public interface ISteppeHerbService {
	int BrownTwyreAmount { get; set; }

	int BloodTwyreAmount { get; set; }

	int BlackTwyreAmount { get; set; }

	int SweveryAmount { get; set; }

	void Reset();
}