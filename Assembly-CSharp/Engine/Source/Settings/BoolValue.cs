using Inspectors;

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
