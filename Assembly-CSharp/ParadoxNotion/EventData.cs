namespace ParadoxNotion
{
  public class EventData(string name) {
    public string name = name;

    public object value => GetValue();

    protected virtual object GetValue() => null;
  }
}
