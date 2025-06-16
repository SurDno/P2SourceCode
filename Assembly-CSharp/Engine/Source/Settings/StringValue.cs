// Decompiled with JetBrains decompiler
// Type: Engine.Source.Settings.StringValue
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Inspectors;

#nullable disable
namespace Engine.Source.Settings
{
  public class StringValue : IValue<string>
  {
    [Inspected]
    private string name;
    [Inspected]
    private string defaultValue;
    private string value;

    public StringValue(string name, string defaultValue = "")
    {
      this.name = name;
      this.defaultValue = defaultValue;
      this.value = PlayerSettings.Instance.GetString(name, defaultValue);
    }

    [Inspected(Mutable = true)]
    public string Value
    {
      get => this.value;
      set
      {
        if (this.value == value)
          return;
        this.value = value;
        PlayerSettings.Instance.SetString(this.name, value);
        PlayerSettings.Instance.Save();
      }
    }

    public string DefaultValue => this.defaultValue;

    public string MinValue => this.defaultValue;

    public string MaxValue => this.defaultValue;
  }
}
