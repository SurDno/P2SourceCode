// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.ResetWeaponNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Components;
using Engine.Common.Services;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

#nullable disable
namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class ResetWeaponNode : FlowControlNode
  {
    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = this.AddFlowOutput("Out");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        ServiceLocator.GetService<ISimulation>().Player?.GetComponent<IAttackerPlayerComponent>()?.ResetWeapon();
        output.Call();
      }));
    }
  }
}
