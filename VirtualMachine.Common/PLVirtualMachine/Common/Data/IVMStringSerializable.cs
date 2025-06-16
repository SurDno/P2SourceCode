namespace PLVirtualMachine.Common.Data
{
  public interface IVMStringSerializable
  {
    string Write();

    void Read(string data);
  }
}
