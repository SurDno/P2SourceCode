// Decompiled with JetBrains decompiler
// Type: Engine.Source.Settings.FloatValue
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Inspectors;
using UnityEngine;

#nullable disable
namespace Engine.Source.Settings
{
  public class FloatValue : IValue<float>
  {
    [Inspected]
    private string name;
    [Inspected]
    private float defaultValue;
    [Inspected]
    private float minValue;
    [Inspected]
    private float maxValue;
    private float value;

    public FloatValue(string name, float defaultValue = 0.0f, float minValue = -3.40282347E+38f, float maxValue = 3.40282347E+38f)
    {
      this.name = name;
      this.defaultValue = defaultValue;
      this.minValue = minValue;
      this.maxValue = maxValue;
      this.value = PlayerSettings.Instance.GetFloat(name, defaultValue);
      this.value = Mathf.Clamp(this.value, minValue, maxValue);
    }

    [Inspected(Mutable = true)]
    public float Value
    {
      get => this.value;
      set
      {
        if ((double) this.value == (double) value)
          return;
        this.value = value;
        this.value = Mathf.Clamp(this.value, this.minValue, this.maxValue);
        PlayerSettings.Instance.SetFloat(this.name, this.value);
        PlayerSettings.Instance.Save();
      }
    }

    public float DefaultValue => this.defaultValue;

    public float MinValue => this.minValue;

    public float MaxValue => this.maxValue;
  }
}
