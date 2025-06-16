using Engine.Common.Services;
using Engine.Source.Services;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints;

[Category("Engine")]
public class FocusModeEnabledNode : FlowControlNode {
	[Port("Enabled")]
	public bool IsEnabled() {
		var service = ServiceLocator.GetService<QuestCompassService>();
		return service != null && service.IsEnabled;
	}
}