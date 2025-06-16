namespace Engine.Common.Blenders;

public interface IBlendable<T> where T : class, IBlendable<T> {
	void Blend(T a, T b, IPureBlendOperation op);
}