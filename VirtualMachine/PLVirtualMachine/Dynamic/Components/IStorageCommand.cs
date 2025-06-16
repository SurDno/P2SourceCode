namespace PLVirtualMachine.Dynamic.Components
{
  public interface IStorageCommand
  {
    EStorageCommandType StorageCommandType { get; }

    void Clear();
  }
}
