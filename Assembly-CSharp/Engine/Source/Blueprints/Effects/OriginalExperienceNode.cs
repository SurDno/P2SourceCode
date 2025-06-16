using Engine.Common.Services;
using Engine.Impl.Services;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints.Effects;

[Category("Effects")]
public class OriginalExperienceNode : FlowControlNode {
	[Port("Value")]
	private bool Value() {
		var service = ServiceLocator.GetService<VirtualMachineController>();
		return service == null || service.OriginalExperienceSession;
	}
}