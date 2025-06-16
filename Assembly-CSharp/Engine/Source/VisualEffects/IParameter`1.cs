namespace Engine.Source.VisualEffects;

public interface IParameter<T> : IParameter where T : struct {
	T Value { get; }
}