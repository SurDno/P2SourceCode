using Engine.Common.Components.AttackerPlayer;
using Engine.Common.Services;
using Engine.Source.Services;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using System;

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
      ServiceLocator.GetService<WeaponVisibleListener>().OnWeaponVisibleChanged += new Action<WeaponKind, bool>(this.OnWeaponVisibleChanged);
    }

    public override void OnGraphStoped()
    {
      ServiceLocator.GetService<WeaponVisibleListener>().OnWeaponVisibleChanged -= new Action<WeaponKind, bool>(this.OnWeaponVisibleChanged);
      base.OnGraphStoped();
    }

    private void OnWeaponVisibleChanged(WeaponKind kind, bool visible)
    {
      if (this.weaponInput.value != kind)
        return;
      if (visible)
        this.showOutput.Call();
      else
        this.hideOutput.Call();
    }

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      this.showOutput = this.AddFlowOutput("Show");
      this.hideOutput = this.AddFlowOutput("Hide");
      this.weaponInput = this.AddValueInput<WeaponKind>("Weapon");
    }
  }
}
