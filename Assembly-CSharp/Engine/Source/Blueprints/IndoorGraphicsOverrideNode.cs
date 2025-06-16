// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.IndoorGraphicsOverrideNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

#nullable disable
namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class IndoorGraphicsOverrideNode : FlowControlNode
  {
    private ValueInput<bool> insideIndoorInput;
    private ValueInput<bool> isolatedInput;
    private ValueInput<bool> cutsceneIsolatedInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = this.AddFlowOutput("Out");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        bool flag = this.insideIndoorInput.value;
        PlayerIndoorCheck.Override(flag);
        PlayerIsolatedIndoorCheck.Override(flag && this.isolatedInput.value);
        CutsceneIndoorCheck.Set(this.cutsceneIsolatedInput.value);
        output.Call();
      }));
      this.insideIndoorInput = this.AddValueInput<bool>("InsideIndoor");
      this.isolatedInput = this.AddValueInput<bool>("Isolated");
      this.cutsceneIsolatedInput = this.AddValueInput<bool>("CutsceneIsolated");
    }
  }
}
