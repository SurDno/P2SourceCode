using Engine.Common.DateTime;
using Engine.Common.Services;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints.Sounds;

[Category("Sounds")]
public class IsTimesNode : FlowControlNode {
	[FromLocator] private ITimeService timeService;
	[Port("Value")] private ValueInput<TimesOfDay> timesInput;

	[Port("Value")]
	private bool Value() {
		return TimesOfDayUtility.HasValue(timesInput.value, timeService.SolarTime.GetTimesOfDay());
	}
}