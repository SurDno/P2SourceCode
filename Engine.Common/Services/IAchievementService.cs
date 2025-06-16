namespace Engine.Common.Services;

public interface IAchievementService {
	void Unlock(string id);

	void Reset(string id);
}