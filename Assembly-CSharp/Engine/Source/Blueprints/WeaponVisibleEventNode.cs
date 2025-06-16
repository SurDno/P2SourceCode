// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.WeaponVisibleEventNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Components.AttackerPlayer;
using Engine.Common.Services;
using Engine.Source.Services;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using System;

#nullable disable
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
