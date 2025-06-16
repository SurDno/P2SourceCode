// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.While
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Design;
using System.Collections;
using UnityEngine;

#nullable disable
namespace FlowCanvas.Nodes
{
  [Name("While True")]
  [Category("Flow Controllers/Repeaters")]
  [Description("Once called, will continuously call 'Do' while the input boolean condition is true. Once condition becomes or is false, 'Done' is called")]
  [ContextDefinedInputs(new System.Type[] {typeof (bool)})]
  public class While : FlowControlNode
  {
    private Coroutine coroutine;

    public override void OnGraphStarted() => this.coroutine = (Coroutine) null;

    public override void OnGraphStoped()
    {
      if (this.coroutine == null)
        return;
      this.StopCoroutine(this.coroutine);
      this.coroutine = (Coroutine) null;
    }

    protected override void RegisterPorts()
    {
      ValueInput<bool> c = this.AddValueInput<bool>("Condition");
      FlowOutput fCurrent = this.AddFlowOutput("Do");
      FlowOutput fFinish = this.AddFlowOutput("Done");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        if (this.coroutine != null)
          return;
        this.coroutine = this.StartCoroutine(this.DoWhile(fCurrent, fFinish, c));
      }));
    }

    private IEnumerator DoWhile(
      FlowOutput fCurrent,
      FlowOutput fFinish,
      ValueInput<bool> condition)
    {
      while (condition.value)
      {
        while (this.graph.isPaused)
          yield return (object) null;
        fCurrent.Call();
        yield return (object) null;
      }
      this.coroutine = (Coroutine) null;
      fFinish.Call();
    }
  }
}
