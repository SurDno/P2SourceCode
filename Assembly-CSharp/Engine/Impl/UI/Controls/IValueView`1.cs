namespace Engine.Impl.UI.Controls;

public interface IValueView<T> {
	void SetValue(int id, T value, bool instant);

	T GetValue(int id);
}