namespace ParadoxNotion;

public class EventData<T> : EventData {
	public T value { get; private set; }

	protected override object GetValue() {
		return value;
	}

	public EventData(string name, T value)
		: base(name) {
		this.value = value;
	}
}