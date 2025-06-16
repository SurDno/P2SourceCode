// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blenders.SmoothBlender`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Blenders;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Commons;
using Inspectors;
using System;
using UnityEngine;

#nullable disable
namespace Engine.Source.Blenders
{
  public abstract class SmoothBlender<T> : EngineObject, ISmoothBlender<T>, IUpdatable where T : class, IObject, IBlendable<T>
  {
    [Inspected]
    private T fromBlendable;
    [Inspected]
    private T currentBlendable;
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
    private LerpBlendOperation lerpBlendOperation = new LerpBlendOperation();

    [Inspected]
    public Guid SnapshotTemplateId { get; private set; }

    public T Current => this.currentBlendable;

    public T Target => this.targetBlendable;

    public float Progress => this.progress;

    public event Action<ISmoothBlender<T>> OnChanged;

    public SmoothBlender()
    {
      this.fromBlendable = ServiceLocator.GetService<IFactory>().Create<T>();
      this.currentBlendable = ServiceLocator.GetService<IFactory>().Create<T>();
    }

    public void BlendTo(T value, TimeSpan interval)
    {
      this.SnapshotTemplateId = value.Id;
      this.Stop();
      this.startTime = ServiceLocator.GetService<TimeService>().AbsoluteGameTime;
      this.interval = interval;
      this.targetBlendable = value;
      this.compute = true;
      InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable((IUpdatable) this);
    }

    private void Stop()
    {
      if (!this.compute)
        return;
      this.compute = false;
      InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable((IUpdatable) this);
      ((ICopyable) (object) this.currentBlendable).CopyTo((object) this.fromBlendable);
      this.startTime = TimeSpan.Zero;
      this.interval = TimeSpan.Zero;
      this.targetBlendable = default (T);
      this.progress = 0.0f;
      Action<ISmoothBlender<T>> onChanged = this.OnChanged;
      if (onChanged == null)
        return;
      onChanged((ISmoothBlender<T>) this);
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
        ((ICopyable) (object) this.targetBlendable).CopyTo((object) this.currentBlendable);
        this.Stop();
      }
      else
      {
        float num = Mathf.Clamp01((float) (timeSpan.TotalSeconds / this.interval.TotalSeconds));
        if ((double) num == (double) this.progress)
          return;
        this.progress = num;
        this.lerpBlendOperation.Time = this.progress;
        this.currentBlendable.Blend(this.fromBlendable, this.targetBlendable, (IPureBlendOperation) this.lerpBlendOperation);
        Action<ISmoothBlender<T>> onChanged = this.OnChanged;
        if (onChanged == null)
          return;
        onChanged((ISmoothBlender<T>) this);
      }
    }
  }
}
