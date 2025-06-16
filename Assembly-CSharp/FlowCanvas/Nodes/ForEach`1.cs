// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.ForEach`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Design;
using System.Collections.Generic;

#nullable disable
namespace FlowCanvas.Nodes
{
  [Description("Enumerate a value (usualy a list or array) for each of it's elements")]
  [Category("Flow Controllers/Iterators")]
  public class ForEach<T> : FlowControlNode
  {
    private T current;
    private bool broken;

    protected override void RegisterPorts()
    {
      ValueInput<IEnumerable<T>> list = this.AddValueInput<IEnumerable<T>>("Value");
      this.AddValueOutput<T>("Current", (ValueHandler<T>) (() => this.current));
      FlowOutput fCurrent = this.AddFlowOutput("Do");
      FlowOutput fFinish = this.AddFlowOutput("Done");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        IEnumerable<T> objs = list.value;
        if (objs == null)
        {
          fFinish.Call();
        }
        else
        {
          this.broken = false;
          foreach (T obj in objs)
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
