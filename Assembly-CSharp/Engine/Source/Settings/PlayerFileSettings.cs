using System;
using System.IO;
using System.Xml;
using Cofe.Serializations.Converters;
using Engine.Common.Utility;
using UnityEngine;

namespace Engine.Source.Settings;

public class PlayerFileSettings : IPlayerSettings {
	private string path =
		"{DataPath}/Settings/PlayerSettings.xml".Replace("{DataPath}", Application.persistentDataPath);

	private XmlElement element;

	public PlayerFileSettings() {
		if (File.Exists(path))
			try {
				var xmlDocument = new XmlDocument();
				xmlDocument.Load(path);
				element = xmlDocument.DocumentElement;
			} catch (Exception ex) {
				Debug.LogException(ex);
			}

		if (element != null)
			return;
		element = XmlUtility.CreateDocument().CreateNode("Root");
		Save();
	}

	public int GetInt(string name, int defaultValue) {
		var str = GetValue(name);
		return str == null ? defaultValue : DefaultConverter.ParseInt(str);
	}

	public void SetInt(string name, int value) {
		SetValue(name, DefaultConverter.ToString(value));
	}

	public bool GetBool(string name, bool defaultValue) {
		var str = GetValue(name);
		return str == null ? defaultValue : DefaultConverter.ParseBool(str);
	}

	public void SetBool(string name, bool value) {
		SetValue(name, DefaultConverter.ToString(value));
	}

	public float GetFloat(string name, float defaultValue) {
		var str = GetValue(name);
		return str == null ? defaultValue : DefaultConverter.ParseFloat(str);
	}

	public void SetFloat(string name, float value) {
		SetValue(name, DefaultConverter.ToString(value));
	}

	public string GetString(string name, string defaultValue) {
		return GetValue(name) ?? defaultValue;
	}

	public void SetString(string name, string value) {
		SetValue(name, value);
	}

	public T GetEnum<T>(string name, T defaultValue) where T : struct, IComparable, IFormattable, IConvertible {
		var str = GetValue(name);
		T result;
		return str == null || !DefaultConverter.TryParseEnum(str, out result) ? defaultValue : result;
	}

	public void SetEnum<T>(string name, T value) where T : struct, IComparable, IFormattable, IConvertible {
		SetValue(name, value.ToString());
	}

	public void Save() {
		try {
			element.SaveDocument(path);
		} catch (Exception ex) {
			Debug.LogException(ex);
		}
	}

	private string GetValue(string name) {
		foreach (XmlElement childNode in element.ChildNodes) {
			var xmlElement = childNode["Key"];
			if (xmlElement != null && xmlElement.InnerText == name)
				return childNode["Value"]?.InnerText;
		}

		return null;
	}

	private void SetValue(string name, string value) {
		foreach (XmlElement childNode in element.ChildNodes) {
			var xmlElement1 = childNode["Key"];
			if (xmlElement1 != null && xmlElement1.InnerText == name) {
				var xmlElement2 = childNode["Value"];
				if (xmlElement2 != null) {
					xmlElement2.InnerText = value;
					return;
				}

				childNode.CreateNode("Value", value);
				return;
			}
		}

		var node = element.CreateNode("Item");
		node.CreateNode("Key", name);
		node.CreateNode("Value", value);
	}
}