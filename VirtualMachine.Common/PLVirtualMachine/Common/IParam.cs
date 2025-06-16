namespace PLVirtualMachine.Common
{
  public interface IParam : IVariable, INamed
  {
    object Value { get; set; }

    bool Implicit { get; }

    IGameObjectContext OwnerContext { get; }
  }
}
