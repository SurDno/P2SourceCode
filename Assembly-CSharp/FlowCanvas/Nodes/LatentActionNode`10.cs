﻿using System.Collections;
using ParadoxNotion;

namespace FlowCanvas.Nodes
{
  public abstract class LatentActionNode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : 
    LatentActionNodeBase
  {
    public abstract IEnumerator Invoke(
      T1 a,
      T2 b,
      T3 c,
      T4 d,
      T5 e,
      T6 f,
      T7 g,
      T8 h,
      T9 i,
      T10 j);

    protected override sealed void OnRegisterPorts(FlowNode node)
    {
      base.OnRegisterPorts(node);
      ValueInput<T1> p1 = node.AddValueInput<T1>(parameters[0].Name.SplitCamelCase());
      ValueInput<T2> p2 = node.AddValueInput<T2>(parameters[1].Name.SplitCamelCase());
      ValueInput<T3> p3 = node.AddValueInput<T3>(parameters[2].Name.SplitCamelCase());
      ValueInput<T4> p4 = node.AddValueInput<T4>(parameters[3].Name.SplitCamelCase());
      ValueInput<T5> p5 = node.AddValueInput<T5>(parameters[4].Name.SplitCamelCase());
      ValueInput<T6> p6 = node.AddValueInput<T6>(parameters[5].Name.SplitCamelCase());
      ValueInput<T7> p7 = node.AddValueInput<T7>(parameters[6].Name.SplitCamelCase());
      ValueInput<T8> p8 = node.AddValueInput<T8>(parameters[7].Name.SplitCamelCase());
      ValueInput<T9> p9 = node.AddValueInput<T9>(parameters[8].Name.SplitCamelCase());
      ValueInput<T10> p10 = node.AddValueInput<T10>(parameters[9].Name.SplitCamelCase());
      node.AddFlowInput("In", () => Begin(Invoke(p1.value, p2.value, p3.value, p4.value, p5.value, p6.value, p7.value, p8.value, p9.value, p10.value)));
      node.AddFlowInput("Break", () => Break());
    }
  }
}
