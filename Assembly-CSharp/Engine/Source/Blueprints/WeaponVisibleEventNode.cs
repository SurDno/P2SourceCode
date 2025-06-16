using Engine.Common.Components.AttackerPlayer;
using Engine.Common.Services;
using Engine.Source.Services;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class WeaponVisibleEventNode : EventNode<FlowScriptController>
  {
    private ValueInput<WeaponKind> weaponInput;
    private FlowOutput showOutput;
    private FlowOutput hideOutput;

    public override void OnGraphStarted()
    {
      base.OnGraphStarted();
      ServiceLocator.GetService<WeaponVisibleListener>().OnWeaponVisibleChanged += OnWeaponVisibleChanged;
    }

    public override void OnGraphStoped()
    {
      ServiceLocator.GetService<WeaponVisibleListener>().OnWeaponVisibleChanged -= OnWeaponVisibleChanged;
      base.OnGraphStoped();
    }

    private void OnWeaponVisibleChanged(WeaponKind kind, bool visible)
    {
      if (weaponInput.value != kind)
        return;
      if (visible)
        showOutput.Call();
      else
        hideOutput.Call();
    }

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      showOutput = AddFlowOutput("Show");
      hideOutput = AddFlowOutput("Hide");
      weaponInput = AddValueInput<WeaponKind>("Weapon");
    }
  }
}
