namespace ParadoxNotion.Serialization.FullSerializer.Internal;

public static class fsOption {
	public static fsOption<T> Just<T>(T value) {
		return new fsOption<T>(value);
	}
}