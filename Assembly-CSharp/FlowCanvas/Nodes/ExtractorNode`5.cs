using ParadoxNotion;

namespace FlowCanvas.Nodes
{
  public abstract class ExtractorNode<TInstance, T1, T2, T3, T4> : ExtractorNode
  {
    private T1 a;
    private T2 b;
    private T3 c;
    private T4 d;

    public abstract void Invoke(TInstance instance, out T1 a, out T2 b, out T3 c, out T4 d);

    protected override sealed void OnRegisterPorts(FlowNode node)
    {
      ValueInput<TInstance> i = node.AddValueInput<TInstance>(typeof (TInstance).FriendlyName());
      node.AddValueOutput(parameters[1].Name.SplitCamelCase(), () =>
      {
        Invoke(i.value, out a, out b, out c, out d);
        return a;
      });
      node.AddValueOutput(parameters[2].Name.SplitCamelCase(), () =>
      {
        Invoke(i.value, out a, out b, out c, out d);
        return b;
      });
      node.AddValueOutput(parameters[3].Name.SplitCamelCase(), () =>
      {
        Invoke(i.value, out a, out b, out c, out d);
        return c;
      });
      node.AddValueOutput(parameters[4].Name.SplitCamelCase(), () =>
      {
        Invoke(i.value, out a, out b, out c, out d);
        return d;
      });
    }
  }
}
