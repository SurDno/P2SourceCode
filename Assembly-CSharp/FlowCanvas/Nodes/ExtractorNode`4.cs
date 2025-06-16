using ParadoxNotion;

namespace FlowCanvas.Nodes
{
  public abstract class ExtractorNode<TInstance, T1, T2, T3> : ExtractorNode
  {
    private T1 a;
    private T2 b;
    private T3 c;

    public abstract void Invoke(TInstance instance, out T1 a, out T2 b, out T3 c);

    protected override sealed void OnRegisterPorts(FlowNode node)
    {
      ValueInput<TInstance> i = node.AddValueInput<TInstance>(typeof (TInstance).FriendlyName());
      node.AddValueOutput(parameters[1].Name.SplitCamelCase(), () =>
      {
        Invoke(i.value, out a, out b, out c);
        return a;
      });
      node.AddValueOutput(parameters[2].Name.SplitCamelCase(), () =>
      {
        Invoke(i.value, out a, out b, out c);
        return b;
      });
      node.AddValueOutput(parameters[3].Name.SplitCamelCase(), () =>
      {
        Invoke(i.value, out a, out b, out c);
        return c;
      });
    }
  }
}
