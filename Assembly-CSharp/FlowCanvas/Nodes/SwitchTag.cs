// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.SwitchTag
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Design;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace FlowCanvas.Nodes
{
  [Category("Flow Controllers/Switchers")]
  [Description("Branch the Flow based on the tag of a GameObject value")]
  [ContextDefinedInputs(new System.Type[] {typeof (GameObject)})]
  public class SwitchTag : FlowControlNode
  {
    [SerializeField]
    private string[] _tagNames = (string[]) null;

    protected override void RegisterPorts()
    {
      ValueInput<GameObject> go = this.AddValueInput<GameObject>("Value");
      List<FlowOutput> outs = new List<FlowOutput>();
      for (int index = 0; index < this._tagNames.Length; ++index)
        outs.Add(this.AddFlowOutput(this._tagNames[index], index.ToString()));
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        for (int index = 0; index < this._tagNames.Length; ++index)
        {
          if (this._tagNames[index] == go.value.tag)
            outs[index].Call();
        }
      }));
    }
  }
}
