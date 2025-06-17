using Inspectors;

namespace Engine.Source.Settings
{
  public class BoolValue(string name, bool defaultValue = false) : IValue<bool> {
    [Inspected]
    private string name = name;
    [Inspected]
    private bool defaultValue = defaultValue;
    private bool value = PlayerSettings.Instance.GetBool(name, defaultValue);

    [Inspected(Mutable = true)]
    public bool Value
    {
      get => value;
      set
      {
        if (this.value == value)
          return;
        this.value = value;
        PlayerSettings.Instance.SetBool(name, value);
        PlayerSettings.Instance.Save();
      }
    }

    public bool DefaultValue => defaultValue;

    public bool MinValue => defaultValue;

    public bool MaxValue => defaultValue;
  }
}
