using ParadoxNotion;

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
      node.AddValueOutput(parameters[1].Name.SplitCamelCase(), () =>
      {
        Invoke(i.value, out a, out b);
        return a;
      });
      node.AddValueOutput(parameters[2].Name.SplitCamelCase(), () =>
      {
        Invoke(i.value, out a, out b);
        return b;
      });
    }
  }
}
