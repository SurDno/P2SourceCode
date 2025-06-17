using Inspectors;

namespace Engine.Source.Settings
{
  public class StringValue(string name, string defaultValue = "") : IValue<string> {
    [Inspected]
    private string name = name;
    [Inspected]
    private string defaultValue = defaultValue;
    private string value = PlayerSettings.Instance.GetString(name, defaultValue);

    [Inspected(Mutable = true)]
    public string Value
    {
      get => value;
      set
      {
        if (this.value == value)
          return;
        this.value = value;
        PlayerSettings.Instance.SetString(name, value);
        PlayerSettings.Instance.Save();
      }
    }

    public string DefaultValue => defaultValue;

    public string MinValue => defaultValue;

    public string MaxValue => defaultValue;
  }
}
