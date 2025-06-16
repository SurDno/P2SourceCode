// Decompiled with JetBrains decompiler
// Type: Engine.Source.Settings.EnumValue`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Inspectors;
using System;

#nullable disable
namespace Engine.Source.Settings
{
  public class EnumValue<T> : IValue<T> where T : struct, IComparable, IFormattable, IConvertible
  {
    [Inspected]
    private string name;
    [Inspected]
    private T defaultValue;
    private T value;

    public EnumValue(string name, T defaultValue = default (T))
    {
      this.name = name;
      this.value = PlayerSettings.Instance.GetEnum<T>(name, defaultValue);
      this.defaultValue = defaultValue;
    }

    [Inspected(Mutable = true)]
    public T Value
    {
      get => this.value;
      set
      {
        if (this.value.CompareTo((object) value) == 0)
          return;
        this.value = value;
        PlayerSettings.Instance.SetEnum<T>(this.name, value);
        PlayerSettings.Instance.Save();
      }
    }

    public T DefaultValue => this.defaultValue;

    public T MinValue => this.defaultValue;

    public T MaxValue => this.defaultValue;
  }
}
