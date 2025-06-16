// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.ANDMerge
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Design;
using System.Collections;
using UnityEngine;

#nullable disable
namespace FlowCanvas.Nodes
{
  [Name("AND")]
  [Category("Flow Controllers/Flow Merge")]
  [Description("Calls Out when all inputs are called together in the same frame")]
  public class ANDMerge : FlowControlNode, IMultiPortNode
  {
    [SerializeField]
    private int _portCount = 2;
    private FlowOutput fOut;
    private bool[] calls;

    public int portCount
    {
      get => this._portCount;
      set => this._portCount = value;
    }

    protected override void RegisterPorts()
    {
      this.calls = new bool[this.portCount];
      this.fOut = this.AddFlowOutput("Out");
      for (int index = 0; index < this.portCount; ++index)
      {
        int i = index;
        this.AddFlowInput(i.ToString(), (FlowHandler) (() =>
        {
          this.calls[i] = true;
          this.Check();
        }));
      }
    }

    private void Check()
    {
      this.StartCoroutine(this.Reset());
      for (int index = 0; index < this.calls.Length; ++index)
      {
        if (!this.calls[index])
          return;
      }
      this.fOut.Call();
    }

    private IEnumerator Reset()
    {
      yield return (object) null;
      for (int i = 0; i < this.calls.Length; ++i)
        this.calls[i] = false;
    }
  }
}
