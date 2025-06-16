using Engine.Common.Components;
using Engine.Common.Components.AttackerPlayer;
using Engine.Common.Services;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints;

[Category("Engine")]
public class SetWeaponNode : FlowControlNode {
	private ValueInput<WeaponKind> weaponInput;

	protected override void RegisterPorts() {
		base.RegisterPorts();
		var output = AddFlowOutput("Out");
		AddFlowInput("In", () => {
			ServiceLocator.GetService<ISimulation>().Player?.GetComponent<IAttackerPlayerComponent>()
				?.SetWeapon(weaponInput.value);
			output.Call();
		});
		weaponInput = AddValueInput<WeaponKind>("Weapon");
	}
}