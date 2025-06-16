using System;
using System.Collections.Generic;
using Cofe.Utility;
using Engine.Common;
using Engine.Common.Services;
using Inspectors;
using UnityEngine;
using UnityEngine.Profiling;
using Random = UnityEngine.Random;

namespace Engine.Source.Commons;

public class DelayUpdater : IUpdater {
	[Inspected] private List<IUpdatable> updatable = new();
	[Inspected(Mutable = true)] private float delay;
	[Inspected] private float accumulator;

	public DelayUpdater(float delay) {
		this.delay = delay;
		accumulator = delay * Random.value;
	}

	public void AddUpdatable(IUpdatable up) {
		updatable.Add(up);
	}

	public void RemoveUpdatable(IUpdatable up) {
		var updatableIndex = GetUpdatableIndex(up);
		if (updatableIndex == -1)
			throw new Exception();
		updatable[updatableIndex] = null;
	}

	private int GetUpdatableIndex(IUpdatable up) {
		if (up == null)
			throw new Exception();
		for (var index = 0; index < updatable.Count; ++index)
			if (updatable[index] == up)
				return index;
		return -1;
	}

	public void ComputeUpdate() {
		accumulator += Time.deltaTime;
		if (accumulator < (double)delay || ServiceCache.OptimizationService.FrameHasSpike)
			return;
		accumulator = 0.0f;
		var index = 0;
		while (index < this.updatable.Count) {
			var updatable = this.updatable[index];
			if (updatable == null) {
				this.updatable[index] = this.updatable[this.updatable.Count - 1];
				this.updatable.RemoveAt(this.updatable.Count - 1);
			} else {
				if (Profiler.enabled)
					Profiler.BeginSample(TypeUtility.GetTypeName(updatable.GetType()));
				try {
					updatable.ComputeUpdate();
				} catch (Exception ex) {
					Debug.LogException(ex);
				}

				if (Profiler.enabled)
					Profiler.EndSample();
				++index;
			}
		}
	}
}