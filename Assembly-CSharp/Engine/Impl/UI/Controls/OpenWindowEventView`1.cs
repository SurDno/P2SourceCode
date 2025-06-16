using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.UI;
using UnityEngine;

namespace Engine.Impl.UI.Controls;

public class OpenWindowEventView<T> : EventView where T : class, IWindow {
	[SerializeField] private bool swap;

	public override void Invoke() {
		if (!PrepareWindow())
			return;
		if (swap)
			ServiceLocator.GetService<UIService>().Swap<T>();
		else
			ServiceLocator.GetService<UIService>().Push<T>();
	}

	protected virtual bool PrepareWindow() {
		return true;
	}
}