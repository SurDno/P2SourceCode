namespace Engine.Common
{
  public struct EventArgument<TActor, TTarget>
  {
    public TActor Actor;
    public TTarget Target;
    public bool IsCanceled;
  }
}
