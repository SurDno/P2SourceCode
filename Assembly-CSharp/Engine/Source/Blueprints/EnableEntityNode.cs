// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.EnableEntityNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

#nullable disable
namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class EnableEntityNode : FlowControlNode
  {
    private ValueInput<IEntity> entityInput;
    private ValueInput<bool> enableInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = this.AddFlowOutput("Out");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        IEntity entity = this.entityInput.value;
        if (entity != null)
          entity.IsEnabled = this.enableInput.value;
        output.Call();
      }));
      this.entityInput = this.AddValueInput<IEntity>("Entity");
      this.enableInput = this.AddValueInput<bool>("Enable");
    }
  }
}
