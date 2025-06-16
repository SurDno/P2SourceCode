using System;
using System.Collections.Generic;
using Cofe.Serializations.Converters;
using Inspectors;

namespace Engine.Source.Settings
{
  public class ListKeyItems
  {
    [Inspected]
    private string name;
    [Inspected]
    private List<KeyValuePair<string, KeyCode>> value = new List<KeyValuePair<string, KeyCode>>();
    private static readonly char separator = '|';
    private static readonly char[] separators = new char[1]
    {
      separator
    };

    public ListKeyItems(string name)
    {
      this.name = name;
      string[] strArray = PlayerSettings.Instance.GetString(name).Split(separators, StringSplitOptions.RemoveEmptyEntries);
      for (int index = 0; index < strArray.Length; index += 2)
      {
        KeyCode result;
        DefaultConverter.TryParseEnum<KeyCode>(strArray[index + 1], out result);
        value.Add(new KeyValuePair<string, KeyCode>(strArray[index], result));
      }
    }

    public List<KeyValuePair<string, KeyCode>> Value
    {
      get => value;
      set
      {
        this.value = value;
        string str = "";
        foreach (KeyValuePair<string, KeyCode> keyValuePair in value)
        {
          str += keyValuePair.Key;
          str += separator.ToString();
          str += keyValuePair.Value.ToString();
          str += separator.ToString();
        }
        PlayerSettings.Instance.SetString(name, str);
        PlayerSettings.Instance.Save();
      }
    }
  }
}
