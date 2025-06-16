using Engine.Common;

namespace Engine.Source.UI;

public interface ISleepWindow : IWindow {
	IEntity Actor { get; set; }

	IEntity Target { get; set; }
}