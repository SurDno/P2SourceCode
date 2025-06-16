using Cofe.Serializations.Converters;
using Inspectors;
using System;
using System.Collections.Generic;
using UnityEngine;

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
      ListKeyItems.separator
    };

    public ListKeyItems(string name)
    {
      this.name = name;
      string[] strArray = PlayerSettings.Instance.GetString(name).Split(ListKeyItems.separators, StringSplitOptions.RemoveEmptyEntries);
      for (int index = 0; index < strArray.Length; index += 2)
      {
        KeyCode result;
        DefaultConverter.TryParseEnum<KeyCode>(strArray[index + 1], out result);
        this.value.Add(new KeyValuePair<string, KeyCode>(strArray[index], result));
      }
    }

    public List<KeyValuePair<string, KeyCode>> Value
    {
      get => this.value;
      set
      {
        this.value = value;
        string str = "";
        foreach (KeyValuePair<string, KeyCode> keyValuePair in value)
        {
          str += keyValuePair.Key;
          str += ListKeyItems.separator.ToString();
          str += keyValuePair.Value.ToString();
          str += ListKeyItems.separator.ToString();
        }
        PlayerSettings.Instance.SetString(this.name, str);
        PlayerSettings.Instance.Save();
      }
    }
  }
}
