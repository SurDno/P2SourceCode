using Engine.Common;

namespace Engine.Source.UI;

public interface IRepairingWindow : ISelectableInventoryWindow, IWindow {
	IEntity Target { get; set; }
}