// Decompiled with JetBrains decompiler
// Type: Engine.Source.Settings.BoolValue
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Inspectors;

#nullable disable
namespace Engine.Source.Settings
{
  public class BoolValue : IValue<bool>
  {
    [Inspected]
    private string name;
    [Inspected]
    private bool defaultValue;
    private bool value;

    public BoolValue(string name, bool defaultValue = false)
    {
      this.name = name;
      this.value = PlayerSettings.Instance.GetBool(name, defaultValue);
      this.defaultValue = defaultValue;
    }

    [Inspected(Mutable = true)]
    public bool Value
    {
      get => this.value;
      set
      {
        if (this.value == value)
          return;
        this.value = value;
        PlayerSettings.Instance.SetBool(this.name, value);
        PlayerSettings.Instance.Save();
      }
    }

    public bool DefaultValue => this.defaultValue;

    public bool MinValue => this.defaultValue;

    public bool MaxValue => this.defaultValue;
  }
}
