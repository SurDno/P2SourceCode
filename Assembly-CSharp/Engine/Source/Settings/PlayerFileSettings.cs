using Cofe.Serializations.Converters;
using Engine.Common.Utility;
using System;
using System.IO;
using System.Xml;
using UnityEngine;

namespace Engine.Source.Settings
{
  public class PlayerFileSettings : IPlayerSettings
  {
    private string path = "{DataPath}/Settings/PlayerSettings.xml".Replace("{DataPath}", Application.persistentDataPath);
    private XmlElement element;

    public PlayerFileSettings()
    {
      if (File.Exists(this.path))
      {
        try
        {
          XmlDocument xmlDocument = new XmlDocument();
          xmlDocument.Load(this.path);
          this.element = xmlDocument.DocumentElement;
        }
        catch (Exception ex)
        {
          Debug.LogException(ex);
        }
      }
      if (this.element != null)
        return;
      this.element = XmlUtility.CreateDocument().CreateNode("Root");
      this.Save();
    }

    public int GetInt(string name, int defaultValue)
    {
      string str = this.GetValue(name);
      return str == null ? defaultValue : DefaultConverter.ParseInt(str);
    }

    public void SetInt(string name, int value)
    {
      this.SetValue(name, DefaultConverter.ToString(value));
    }

    public bool GetBool(string name, bool defaultValue)
    {
      string str = this.GetValue(name);
      return str == null ? defaultValue : DefaultConverter.ParseBool(str);
    }

    public void SetBool(string name, bool value)
    {
      this.SetValue(name, DefaultConverter.ToString(value));
    }

    public float GetFloat(string name, float defaultValue)
    {
      string str = this.GetValue(name);
      return str == null ? defaultValue : DefaultConverter.ParseFloat(str);
    }

    public void SetFloat(string name, float value)
    {
      this.SetValue(name, DefaultConverter.ToString(value));
    }

    public string GetString(string name, string defaultValue)
    {
      return this.GetValue(name) ?? defaultValue;
    }

    public void SetString(string name, string value) => this.SetValue(name, value);

    public T GetEnum<T>(string name, T defaultValue) where T : struct, IComparable, IFormattable, IConvertible
    {
      string str = this.GetValue(name);
      T result;
      return str == null || !DefaultConverter.TryParseEnum<T>(str, out result) ? defaultValue : result;
    }

    public void SetEnum<T>(string name, T value) where T : struct, IComparable, IFormattable, IConvertible
    {
      this.SetValue(name, value.ToString());
    }

    public void Save()
    {
      try
      {
        this.element.SaveDocument(this.path);
      }
      catch (Exception ex)
      {
        Debug.LogException(ex);
      }
    }

    private string GetValue(string name)
    {
      foreach (XmlElement childNode in this.element.ChildNodes)
      {
        XmlElement xmlElement = childNode["Key"];
        if (xmlElement != null && xmlElement.InnerText == name)
          return childNode["Value"]?.InnerText;
      }
      return (string) null;
    }

    private void SetValue(string name, string value)
    {
      foreach (XmlElement childNode in this.element.ChildNodes)
      {
        XmlElement xmlElement1 = childNode["Key"];
        if (xmlElement1 != null && xmlElement1.InnerText == name)
        {
          XmlElement xmlElement2 = childNode["Value"];
          if (xmlElement2 != null)
          {
            xmlElement2.InnerText = value;
            return;
          }
          childNode.CreateNode("Value", value);
          return;
        }
      }
      XmlElement node = this.element.CreateNode("Item");
      node.CreateNode("Key", name);
      node.CreateNode("Value", value);
    }
  }
}
