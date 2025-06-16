using Engine.Common.Components;

namespace Engine.Source.UI;

public interface ITradeWindow : IWindow {
	IStorageComponent Actor { get; set; }

	IMarketComponent Market { get; set; }
}