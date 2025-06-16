using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Impl.UI.Menu;
using Engine.Source.UI;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints.Sounds;

[Category("Sounds")]
public class IsPauseMenuNode : FlowControlNode {
	[Port("Value")]
	private bool Value() {
		var service = ServiceLocator.GetService<UIService>();
		if (service == null || !service.IsInitialize)
			return false;
		var active = service.Active;
		return !(active == null) && active is IPauseMenu;
	}
}