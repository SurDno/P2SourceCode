namespace Engine.Source.Commons;

public interface IUpdateItem<T> {
	void ComputeUpdateItem(T item);
}