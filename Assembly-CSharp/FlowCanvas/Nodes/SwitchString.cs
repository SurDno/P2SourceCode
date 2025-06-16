// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.SwitchString
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Design;
using System;
using System.Collections.Generic;

#nullable disable
namespace FlowCanvas.Nodes
{
  [Category("Flow Controllers/Switchers")]
  [Description("Branch the Flow based on a string value. The Default output is called if there is no other matching output same as the input value")]
  [ContextDefinedInputs(new Type[] {typeof (string)})]
  public class SwitchString : FlowControlNode
  {
    public List<string> comparisonOutputs = new List<string>();

    protected override void RegisterPorts()
    {
      ValueInput<string> name = this.AddValueInput<string>("Value");
      List<FlowOutput> outs = new List<FlowOutput>();
      for (int index = 0; index < this.comparisonOutputs.Count; ++index)
        outs.Add(this.AddFlowOutput(string.Format("\"{0}\"", (object) this.comparisonOutputs[index]), index.ToString()));
      FlowOutput def = this.AddFlowOutput("Default");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        string str = name.value;
        if (str == null)
        {
          def.Call();
        }
        else
        {
          bool flag = false;
          for (int index = 0; index < this.comparisonOutputs.Count; ++index)
          {
            if (string.IsNullOrEmpty(str) && string.IsNullOrEmpty(this.comparisonOutputs[index]))
            {
              outs[index].Call();
              flag = true;
            }
            else if (this.comparisonOutputs[index].Trim().ToLower() == str.Trim().ToLower())
            {
              outs[index].Call();
              flag = true;
            }
          }
          if (flag)
            return;
          def.Call();
        }
      }));
    }
  }
}
