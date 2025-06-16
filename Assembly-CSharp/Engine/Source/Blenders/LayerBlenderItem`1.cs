using System;
using Engine.Common;
using Engine.Common.Blenders;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Commons;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Blenders;

public abstract class LayerBlenderItem<T> : EngineObject, ILayerBlenderItem<T>, IUpdatable
	where T : class, IObject, IBlendable<T> {
	[Inspected] private ISmoothBlender<T> blender;
	[Inspected] private float fromOpacity;
	[Inspected] private float currentOpacity;
	[Inspected] private float targetOpacity;
	[Inspected] private TimeSpan startTime;
	[Inspected] private TimeSpan interval;
	[Inspected] private bool compute;
	[Inspected] private float progress;

	public ISmoothBlender<T> Blender {
		get => blender;
		set {
			if (blender != null)
				blender.OnChanged -= BlenderOnChanged;
			blender = value;
			if (blender != null)
				blender.OnChanged += BlenderOnChanged;
			var onChanged = OnChanged;
			if (onChanged == null)
				return;
			onChanged(this);
		}
	}

	private void BlenderOnChanged(ISmoothBlender<T> obj) {
		var onChanged = OnChanged;
		if (onChanged == null)
			return;
		onChanged(this);
	}

	[Inspected] public float Opacity => currentOpacity;

	[Inspected] public float TargetOpacity => compute ? targetOpacity : currentOpacity;

	public void SetOpacity(float value) {
		SetOpacity(value, TimeSpan.Zero);
	}

	public void SetOpacity(float value, TimeSpan interval) {
		Stop();
		startTime = ServiceLocator.GetService<TimeService>().AbsoluteGameTime;
		this.interval = interval;
		targetOpacity = value;
		compute = true;
		InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable(this);
	}

	private void Stop() {
		if (!compute)
			return;
		compute = false;
		InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable(this);
		fromOpacity = currentOpacity;
		startTime = TimeSpan.Zero;
		interval = TimeSpan.Zero;
		targetOpacity = 0.0f;
		progress = 0.0f;
		var onChanged = OnChanged;
		if (onChanged == null)
			return;
		onChanged(this);
	}

	public override void Dispose() {
		Stop();
		base.Dispose();
	}

	public void ComputeUpdate() {
		if (!compute)
			return;
		var timeSpan = ServiceLocator.GetService<TimeService>().AbsoluteGameTime - startTime;
		if (timeSpan >= interval) {
			currentOpacity = targetOpacity;
			Stop();
		} else {
			var num = Mathf.Clamp01((float)(timeSpan.TotalSeconds / interval.TotalSeconds));
			if (num == (double)progress)
				return;
			progress = num;
			currentOpacity = Mathf.LerpUnclamped(fromOpacity, targetOpacity, progress);
			var onChanged = OnChanged;
			if (onChanged == null)
				return;
			onChanged(this);
		}
	}

	public event Action<ILayerBlenderItem<T>> OnChanged;
}