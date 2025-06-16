using System;

namespace Engine.Common.Blenders
{
  public interface ISmoothBlender<T> where T : class, IObject, IBlendable<T>
  {
    T Current { get; }

    event Action<ISmoothBlender<T>> OnChanged;

    void BlendTo(T value, TimeSpan interval);
  }
}
