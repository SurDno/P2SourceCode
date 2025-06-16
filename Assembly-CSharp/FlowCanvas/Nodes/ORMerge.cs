// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.ORMerge
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Design;
using UnityEngine;

#nullable disable
namespace FlowCanvas.Nodes
{
  [Name("OR")]
  [Category("Flow Controllers/Flow Merge")]
  [Description("Calls Out when either input is called")]
  public class ORMerge : FlowControlNode, IMultiPortNode
  {
    private FlowOutput fOut;
    [SerializeField]
    private int _portCount = 2;

    public int portCount
    {
      get => this._portCount;
      set => this._portCount = value;
    }

    protected override void RegisterPorts()
    {
      this.fOut = this.AddFlowOutput("Out");
      for (int index = 0; index < this.portCount; ++index)
        this.AddFlowInput(index.ToString(), (FlowHandler) (() => this.fOut.Call()));
    }
  }
}
