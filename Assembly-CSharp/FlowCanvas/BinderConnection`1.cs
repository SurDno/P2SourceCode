namespace FlowCanvas
{
  public class BinderConnection<T> : BinderConnection
  {
    public override void Bind()
    {
      if (!this.isActive)
        return;
      this.DoNormalBinding(this.sourcePort, this.targetPort);
    }

    public override void UnBind()
    {
      if (!(this.targetPort is ValueInput))
        return;
      (this.targetPort as ValueInput).UnBind();
    }

    private void DoNormalBinding(Port source, Port target)
    {
      (target as ValueInput<T>).BindTo((ValueOutput) source);
    }
  }
}
