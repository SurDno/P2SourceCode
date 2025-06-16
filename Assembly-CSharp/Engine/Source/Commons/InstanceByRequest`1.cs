namespace Engine.Source.Commons;

public abstract class InstanceByRequest<T> where T : class, new() {
	private static T instance;

	public static T Instance {
		get {
			if (instance == null)
				instance = new T();
			return instance;
		}
	}
}