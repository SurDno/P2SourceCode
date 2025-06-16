namespace ParadoxNotion
{
  public class EventData
  {
    public string name;

    public object value => this.GetValue();

    protected virtual object GetValue() => (object) null;

    public EventData(string name) => this.name = name;
  }
}
