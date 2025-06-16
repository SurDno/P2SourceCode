namespace Engine.Common.Services;

public interface IProfilesService {
	string GetValue(string name);

	void SetValue(string name, string value);

	int GetIntValue(string name);

	void SetIntValue(string name, int value);

	bool GetBoolValue(string name);

	void SetBoolValue(string name, bool value);

	float GetFloatValue(string name);

	void SetFloatValue(string name, float value);
}