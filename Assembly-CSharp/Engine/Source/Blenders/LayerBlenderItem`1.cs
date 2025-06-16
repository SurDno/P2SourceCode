using Engine.Common;
using Engine.Common.Blenders;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Commons;
using Inspectors;
using System;
using UnityEngine;

namespace Engine.Source.Blenders
{
  public abstract class LayerBlenderItem<T> : EngineObject, ILayerBlenderItem<T>, IUpdatable where T : class, IObject, IBlendable<T>
  {
    [Inspected]
    private ISmoothBlender<T> blender;
    [Inspected]
    private float fromOpacity;
    [Inspected]
    private float currentOpacity;
    [Inspected]
    private float targetOpacity;
    [Inspected]
    private TimeSpan startTime;
    [Inspected]
    private TimeSpan interval;
    [Inspected]
    private bool compute;
    [Inspected]
    private float progress;

    public ISmoothBlender<T> Blender
    {
      get => this.blender;
      set
      {
        if (this.blender != null)
          this.blender.OnChanged -= new Action<ISmoothBlender<T>>(this.BlenderOnChanged);
        this.blender = value;
        if (this.blender != null)
          this.blender.OnChanged += new Action<ISmoothBlender<T>>(this.BlenderOnChanged);
        Action<ILayerBlenderItem<T>> onChanged = this.OnChanged;
        if (onChanged == null)
          return;
        onChanged((ILayerBlenderItem<T>) this);
      }
    }

    private void BlenderOnChanged(ISmoothBlender<T> obj)
    {
      Action<ILayerBlenderItem<T>> onChanged = this.OnChanged;
      if (onChanged == null)
        return;
      onChanged((ILayerBlenderItem<T>) this);
    }

    [Inspected]
    public float Opacity => this.currentOpacity;

    [Inspected]
    public float TargetOpacity => this.compute ? this.targetOpacity : this.currentOpacity;

    public void SetOpacity(float value) => this.SetOpacity(value, TimeSpan.Zero);

    public void SetOpacity(float value, TimeSpan interval)
    {
      this.Stop();
      this.startTime = ServiceLocator.GetService<TimeService>().AbsoluteGameTime;
      this.interval = interval;
      this.targetOpacity = value;
      this.compute = true;
      InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable((IUpdatable) this);
    }

    private void Stop()
    {
      if (!this.compute)
        return;
      this.compute = false;
      InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable((IUpdatable) this);
      this.fromOpacity = this.currentOpacity;
      this.startTime = TimeSpan.Zero;
      this.interval = TimeSpan.Zero;
      this.targetOpacity = 0.0f;
      this.progress = 0.0f;
      Action<ILayerBlenderItem<T>> onChanged = this.OnChanged;
      if (onChanged == null)
        return;
      onChanged((ILayerBlenderItem<T>) this);
    }

    public override void Dispose()
    {
      this.Stop();
      base.Dispose();
    }

    public void ComputeUpdate()
    {
      if (!this.compute)
        return;
      TimeSpan timeSpan = ServiceLocator.GetService<TimeService>().AbsoluteGameTime - this.startTime;
      if (timeSpan >= this.interval)
      {
        this.currentOpacity = this.targetOpacity;
        this.Stop();
      }
      else
      {
        float num = Mathf.Clamp01((float) (timeSpan.TotalSeconds / this.interval.TotalSeconds));
        if ((double) num == (double) this.progress)
          return;
        this.progress = num;
        this.currentOpacity = Mathf.LerpUnclamped(this.fromOpacity, this.targetOpacity, this.progress);
        Action<ILayerBlenderItem<T>> onChanged = this.OnChanged;
        if (onChanged == null)
          return;
        onChanged((ILayerBlenderItem<T>) this);
      }
    }

    public event Action<ILayerBlenderItem<T>> OnChanged;
  }
}
