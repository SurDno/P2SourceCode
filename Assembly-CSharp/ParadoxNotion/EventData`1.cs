namespace ParadoxNotion
{
  public class EventData<T>(string name, T value) : EventData(name) {
    public T value { get; private set; } = value;

    protected override object GetValue() => value;
  }
}
