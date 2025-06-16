using ParadoxNotion;

namespace FlowCanvas.Nodes
{
  public abstract class ExtractorNode<TInstance, T1, T2, T3, T4, T5> : ExtractorNode
  {
    private T1 a;
    private T2 b;
    private T3 c;
    private T4 d;
    private T5 e;

    public abstract void Invoke(
      TInstance instance,
      out T1 a,
      out T2 b,
      out T3 c,
      out T4 d,
      out T5 e);

    protected override sealed void OnRegisterPorts(FlowNode node)
    {
      ValueInput<TInstance> i = node.AddValueInput<TInstance>(typeof (TInstance).FriendlyName());
      node.AddValueOutput<T1>(this.parameters[1].Name.SplitCamelCase(), (ValueHandler<T1>) (() =>
      {
        this.Invoke(i.value, out this.a, out this.b, out this.c, out this.d, out this.e);
        return this.a;
      }));
      node.AddValueOutput<T2>(this.parameters[2].Name.SplitCamelCase(), (ValueHandler<T2>) (() =>
      {
        this.Invoke(i.value, out this.a, out this.b, out this.c, out this.d, out this.e);
        return this.b;
      }));
      node.AddValueOutput<T3>(this.parameters[3].Name.SplitCamelCase(), (ValueHandler<T3>) (() =>
      {
        this.Invoke(i.value, out this.a, out this.b, out this.c, out this.d, out this.e);
        return this.c;
      }));
      node.AddValueOutput<T4>(this.parameters[4].Name.SplitCamelCase(), (ValueHandler<T4>) (() =>
      {
        this.Invoke(i.value, out this.a, out this.b, out this.c, out this.d, out this.e);
        return this.d;
      }));
      node.AddValueOutput<T5>(this.parameters[5].Name.SplitCamelCase(), (ValueHandler<T5>) (() =>
      {
        this.Invoke(i.value, out this.a, out this.b, out this.c, out this.d, out this.e);
        return this.e;
      }));
    }
  }
}
