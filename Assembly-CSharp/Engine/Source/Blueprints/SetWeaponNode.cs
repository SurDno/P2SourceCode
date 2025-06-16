using Engine.Common.Components;
using Engine.Common.Components.AttackerPlayer;
using Engine.Common.Services;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class SetWeaponNode : FlowControlNode
  {
    private ValueInput<WeaponKind> weaponInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = this.AddFlowOutput("Out");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        ServiceLocator.GetService<ISimulation>().Player?.GetComponent<IAttackerPlayerComponent>()?.SetWeapon(this.weaponInput.value);
        output.Call();
      }));
      this.weaponInput = this.AddValueInput<WeaponKind>("Weapon");
    }
  }
}
