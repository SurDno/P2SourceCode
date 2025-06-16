// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.ForEach
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Design;
using ParadoxNotion.FlowCanvas.Module;
using System;
using System.Collections;

#nullable disable
namespace FlowCanvas.Nodes
{
  [Description("Enumerate a value (usualy a list or array) for each of it's elements")]
  [Category("Flow Controllers/Iterators")]
  [ContextDefinedInputs(new Type[] {typeof (IEnumerable)})]
  [ContextDefinedOutputs(new Type[] {typeof (object)})]
  public class ForEach : FlowControlNode
  {
    private object current;
    private bool broken;

    protected override void RegisterPorts()
    {
      ValueInput<IEnumerable> list = this.AddValueInput<IEnumerable>("Value");
      this.AddValueOutput<object>("Current", (ValueHandler<object>) (() => this.current));
      FlowOutput fCurrent = this.AddFlowOutput("Do");
      FlowOutput fFinish = this.AddFlowOutput("Done");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        IEnumerable enumerable = list.value;
        if (enumerable == null)
        {
          fFinish.Call();
        }
        else
        {
          this.broken = false;
          foreach (object obj in enumerable)
          {
            if (!this.broken)
            {
              this.current = obj;
              fCurrent.Call();
            }
            else
              break;
          }
          fFinish.Call();
        }
      }));
      this.AddFlowInput("Break", (FlowHandler) (() => this.broken = true));
    }
  }
}
