using Engine.Common.Components;

namespace Engine.Source.UI;

public interface ISelectableInventoryWindow : IWindow {
	IStorageComponent Actor { get; set; }
}