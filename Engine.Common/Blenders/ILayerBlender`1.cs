using System;
using System.Collections.Generic;

namespace Engine.Common.Blenders;

public interface ILayerBlender<T> where T : class, IObject, IBlendable<T> {
	T Current { get; }

	IEnumerable<ILayerBlenderItem<T>> Items { get; }

	void AddItem(ILayerBlenderItem<T> item);

	void RemoveItem(ILayerBlenderItem<T> item);

	event Action<ILayerBlender<T>> OnChanged;
}