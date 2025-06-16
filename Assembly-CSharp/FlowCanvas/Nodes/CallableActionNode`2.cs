// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.CallableActionNode`2
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion;

#nullable disable
namespace FlowCanvas.Nodes
{
  public abstract class CallableActionNode<T1, T2> : CallableActionNodeBase
  {
    public abstract void Invoke(T1 a, T2 b);

    protected override sealed void OnRegisterPorts(FlowNode node)
    {
      FlowOutput o = node.AddFlowOutput(" ");
      ValueInput<T1> p1 = node.AddValueInput<T1>(this.parameters[0].Name.SplitCamelCase());
      ValueInput<T2> p2 = node.AddValueInput<T2>(this.parameters[1].Name.SplitCamelCase());
      node.AddFlowInput(" ", (FlowHandler) (() =>
      {
        this.Invoke(p1.value, p2.value);
        o.Call();
      }));
    }
  }
}
