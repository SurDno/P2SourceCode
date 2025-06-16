// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.WaitingPlayerCanControllingNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Source.Utility;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using System.Collections;

#nullable disable
namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class WaitingPlayerCanControllingNode : FlowControlNode
  {
    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = this.AddFlowOutput("Out");
      this.AddFlowInput("In", (FlowHandler) (() => this.StartCoroutine(this.WaitingPlayerCanControlling(output))));
    }

    private IEnumerator WaitingPlayerCanControlling(FlowOutput output)
    {
      while (!PlayerUtility.IsPlayerCanControlling)
        yield return (object) null;
      output.Call();
    }
  }
}
