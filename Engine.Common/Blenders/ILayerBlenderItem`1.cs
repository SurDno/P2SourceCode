using System;

namespace Engine.Common.Blenders
{
  public interface ILayerBlenderItem<T> where T : class, IObject, IBlendable<T>
  {
    ISmoothBlender<T> Blender { get; set; }

    float Opacity { get; }

    float TargetOpacity { get; }

    void SetOpacity(float value);

    void SetOpacity(float value, TimeSpan interval);

    event Action<ILayerBlenderItem<T>> OnChanged;
  }
}
