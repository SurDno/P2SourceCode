namespace ParadoxNotion
{
  public class EventData
  {
    public string name;

    public object value => GetValue();

    protected virtual object GetValue() => null;

    public EventData(string name) => this.name = name;
  }
}
