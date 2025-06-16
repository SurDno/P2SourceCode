// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.ExtractorNode`3
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion;

#nullable disable
namespace FlowCanvas.Nodes
{
  public abstract class ExtractorNode<TInstance, T1, T2> : ExtractorNode
  {
    private T1 a;
    private T2 b;

    public abstract void Invoke(TInstance instance, out T1 a, out T2 b);

    protected override sealed void OnRegisterPorts(FlowNode node)
    {
      ValueInput<TInstance> i = node.AddValueInput<TInstance>(typeof (TInstance).FriendlyName());
      node.AddValueOutput<T1>(this.parameters[1].Name.SplitCamelCase(), (ValueHandler<T1>) (() =>
      {
        this.Invoke(i.value, out this.a, out this.b);
        return this.a;
      }));
      node.AddValueOutput<T2>(this.parameters[2].Name.SplitCamelCase(), (ValueHandler<T2>) (() =>
      {
        this.Invoke(i.value, out this.a, out this.b);
        return this.b;
      }));
    }
  }
}
