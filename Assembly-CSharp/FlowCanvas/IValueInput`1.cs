namespace FlowCanvas;

public interface IValueInput<T> {
	ValueHandler<T> getter { get; set; }
}