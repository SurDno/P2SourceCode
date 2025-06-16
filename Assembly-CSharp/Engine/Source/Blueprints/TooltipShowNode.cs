using Engine.Common.Commons;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Services;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints;

[Category("Engine")]
public class TooltipShowNode : FlowControlNode {
	private ValueInput<string> localizationTag;
	private ValueInput<float> timeout;

	protected override void RegisterPorts() {
		base.RegisterPorts();
		var output = AddFlowOutput("Out");
		AddFlowInput("In", () => {
			var text = localizationTag.value;
			if (!string.IsNullOrEmpty(text))
				text = ServiceLocator.GetService<LocalizationService>().GetText(text);
			ServiceLocator.GetService<NotificationService>().AddNotify(NotificationEnum.Tooltip, text, timeout.value);
			output.Call();
		});
		localizationTag = AddValueInput<string>("Localization Tag");
		timeout = AddValueInput<float>("Timeout");
	}
}