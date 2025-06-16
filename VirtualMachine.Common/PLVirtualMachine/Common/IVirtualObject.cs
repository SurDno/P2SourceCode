namespace PLVirtualMachine.Common
{
  public interface IVirtualObject
  {
    bool IsVirtual { get; }

    IContainer VirtualObjectTemplate { get; }
  }
}
