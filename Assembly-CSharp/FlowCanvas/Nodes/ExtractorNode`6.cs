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
      node.AddValueOutput(parameters[1].Name.SplitCamelCase(), () =>
      {
        Invoke(i.value, out a, out b, out c, out d, out e);
        return a;
      });
      node.AddValueOutput(parameters[2].Name.SplitCamelCase(), () =>
      {
        Invoke(i.value, out a, out b, out c, out d, out e);
        return b;
      });
      node.AddValueOutput(parameters[3].Name.SplitCamelCase(), () =>
      {
        Invoke(i.value, out a, out b, out c, out d, out e);
        return c;
      });
      node.AddValueOutput(parameters[4].Name.SplitCamelCase(), () =>
      {
        Invoke(i.value, out a, out b, out c, out d, out e);
        return d;
      });
      node.AddValueOutput(parameters[5].Name.SplitCamelCase(), () =>
      {
        Invoke(i.value, out a, out b, out c, out d, out e);
        return e;
      });
    }
  }
}
