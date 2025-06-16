using Engine.Common.Components;

namespace Engine.Source.UI;

public interface IHealingWindow : IWindow {
	IStorageComponent Actor { get; set; }

	IStorageComponent Target { get; set; }
}