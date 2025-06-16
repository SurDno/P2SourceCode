using PLVirtualMachine.Common;

namespace VirtualMachine.Data
{
  public interface IDataCreator
  {
    IObject GetOrCreateObjectThreadSave(ulong id);
  }
}
