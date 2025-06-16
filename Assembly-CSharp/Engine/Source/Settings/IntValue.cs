// Decompiled with JetBrains decompiler
// Type: Engine.Source.Settings.IntValue
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Inspectors;
using UnityEngine;

#nullable disable
namespace Engine.Source.Settings
{
  public class IntValue : IValue<int>
  {
    [Inspected]
    private string name;
    [Inspected]
    private int defaultValue;
    [Inspected]
    private int minValue;
    [Inspected]
    private int maxValue;
    private int value;

    public IntValue(string name, int defaultValue = 0, int minValue = -2147483648, int maxValue = 2147483647)
    {
      this.name = name;
      this.defaultValue = defaultValue;
      this.minValue = minValue;
      this.maxValue = maxValue;
      this.value = PlayerSettings.Instance.GetInt(name, defaultValue);
      this.value = Mathf.Clamp(this.value, minValue, maxValue);
    }

    [Inspected(Mutable = true)]
    public int Value
    {
      get => this.value;
      set
      {
        if (this.value == value)
          return;
        this.value = value;
        this.value = Mathf.Clamp(this.value, this.minValue, this.maxValue);
        PlayerSettings.Instance.SetInt(this.name, this.value);
        PlayerSettings.Instance.Save();
      }
    }

    public int DefaultValue => this.defaultValue;

    public int MinValue => this.minValue;

    public int MaxValue => this.maxValue;
  }
}
