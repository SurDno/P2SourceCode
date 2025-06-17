using System;
using Engine.Common;
using Engine.Common.Blenders;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Commons;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Blenders
{
  public abstract class SmoothBlender<T> : EngineObject, ISmoothBlender<T>, IUpdatable where T : class, IObject, IBlendable<T>
  {
    [Inspected]
    private T fromBlendable = ServiceLocator.GetService<IFactory>().Create<T>();
    [Inspected]
    private T currentBlendable = ServiceLocator.GetService<IFactory>().Create<T>();
    [Inspected]
    private T targetBlendable;
    [Inspected]
    private TimeSpan startTime;
    [Inspected]
    private TimeSpan interval;
    [Inspected]
    private bool compute;
    [Inspected]
    private float progress;
    private LerpBlendOperation lerpBlendOperation = new();

    [Inspected]
    public Guid SnapshotTemplateId { get; private set; }

    public T Current => currentBlendable;

    public T Target => targetBlendable;

    public float Progress => progress;

    public event Action<ISmoothBlender<T>> OnChanged;

    public void BlendTo(T value, TimeSpan interval)
    {
      SnapshotTemplateId = value.Id;
      Stop();
      startTime = ServiceLocator.GetService<TimeService>().AbsoluteGameTime;
      this.interval = interval;
      targetBlendable = value;
      compute = true;
      InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable(this);
    }

    private void Stop()
    {
      if (!compute)
        return;
      compute = false;
      InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable(this);
      ((ICopyable) currentBlendable).CopyTo(fromBlendable);
      startTime = TimeSpan.Zero;
      interval = TimeSpan.Zero;
      targetBlendable = default (T);
      progress = 0.0f;
      Action<ISmoothBlender<T>> onChanged = OnChanged;
      if (onChanged == null)
        return;
      onChanged(this);
    }

    public override void Dispose()
    {
      Stop();
      base.Dispose();
    }

    public void ComputeUpdate()
    {
      if (!compute)
        return;
      TimeSpan timeSpan = ServiceLocator.GetService<TimeService>().AbsoluteGameTime - startTime;
      if (timeSpan >= interval)
      {
        ((ICopyable) targetBlendable).CopyTo(currentBlendable);
        Stop();
      }
      else
      {
        float num = Mathf.Clamp01((float) (timeSpan.TotalSeconds / interval.TotalSeconds));
        if (num == (double) progress)
          return;
        progress = num;
        lerpBlendOperation.Time = progress;
        currentBlendable.Blend(fromBlendable, targetBlendable, lerpBlendOperation);
        Action<ISmoothBlender<T>> onChanged = OnChanged;
        if (onChanged == null)
          return;
        onChanged(this);
      }
    }
  }
}
