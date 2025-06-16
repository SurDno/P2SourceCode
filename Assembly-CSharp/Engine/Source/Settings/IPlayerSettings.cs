using System;

namespace Engine.Source.Settings;

public interface IPlayerSettings {
	int GetInt(string name, int defaultValue = 0);

	void SetInt(string name, int value);

	bool GetBool(string name, bool defaultValue = false);

	void SetBool(string name, bool value);

	float GetFloat(string name, float defaultValue = 0.0f);

	void SetFloat(string name, float value);

	string GetString(string name, string defaultValue = "");

	void SetString(string name, string value);

	T GetEnum<T>(string name, T defaultValue = default) where T : struct, IComparable, IFormattable, IConvertible;

	void SetEnum<T>(string name, T value) where T : struct, IComparable, IFormattable, IConvertible;

	void Save();
}