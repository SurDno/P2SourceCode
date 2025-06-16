// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.XORMerge
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Design;
using System;
using System.Collections;

#nullable disable
namespace FlowCanvas.Nodes
{
  [Name("XOR")]
  [Category("Flow Controllers/Flow Merge")]
  [Description("Calls Out when either A or B Input is called, but the other is not in the same frame.\n(A || B) && (A != B)")]
  [Obsolete]
  public class XORMerge : FlowControlNode
  {
    private bool a;
    private bool b;
    private FlowOutput fOut;

    protected override void RegisterPorts()
    {
      this.fOut = this.AddFlowOutput("Out");
      this.AddFlowInput("A", (FlowHandler) (() =>
      {
        this.a = true;
        this.Check();
      }));
      this.AddFlowInput("B", (FlowHandler) (() =>
      {
        this.b = true;
        this.Check();
      }));
    }

    private void Check()
    {
      if ((this.a || this.b) && this.a != this.b)
        this.fOut.Call();
      this.StartCoroutine(this.Reset());
    }

    private IEnumerator Reset()
    {
      yield return (object) null;
      this.a = false;
      this.b = false;
    }
  }
}
