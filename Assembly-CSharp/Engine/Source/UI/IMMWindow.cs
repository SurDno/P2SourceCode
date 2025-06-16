using Engine.Common;

namespace Engine.Source.UI;

public interface IMMWindow : IWindow, IPauseMenu {
	IEntity Actor { get; set; }
}