using System;
using System.Collections.Generic;
using Cofe.Serializations.Converters;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Settings
{
  public class ListKeyItems
  {
    [Inspected]
    private string name;
    [Inspected]
    private List<KeyValuePair<string, KeyCode>> value = [];
    private static readonly char separator = '|';
    private static readonly char[] separators = [
      separator
    ];

    public ListKeyItems(string name)
    {
      this.name = name;
      string[] strArray = PlayerSettings.Instance.GetString(name).Split(separators, StringSplitOptions.RemoveEmptyEntries);
      for (int index = 0; index < strArray.Length; index += 2)
      {
        DefaultConverter.TryParseEnum(strArray[index + 1], out KeyCode result);
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
