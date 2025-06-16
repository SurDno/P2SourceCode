// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.SwitchEnum
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Design;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace FlowCanvas.Nodes
{
  [Category("Flow Controllers/Switchers")]
  [Description("Branch the Flow based on an enum value.\nThere are 2 ways to set the Enum type:\n1) Drag and Drop an enum connection on top of the node.\n2) Select the type after it has been added in the Prefered Types Editor Window.")]
  [ContextDefinedInputs(new System.Type[] {typeof (Enum)})]
  public class SwitchEnum : FlowControlNode
  {
    [SerializeField]
    private System.Type _type = (System.Type) null;

    protected override void RegisterPorts()
    {
      if (this._type == (System.Type) null)
        return;
      ValueInput e = this.AddValueInput(this._type.Name, this._type);
      List<FlowOutput> outs = new List<FlowOutput>();
      foreach (string name in Enum.GetNames(this._type))
        outs.Add(this.AddFlowOutput(name));
      this.AddFlowInput("In", (FlowHandler) (() => outs[(int) Enum.Parse(e.value.GetType(), e.value.ToString())].Call()));
    }
  }
}
