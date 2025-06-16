namespace PLVirtualMachine.Common
{
  public interface INamedElement : INamed
  {
    IContainer Parent { get; }
  }
}
