namespace PLVirtualMachine.Common.Data
{
  public abstract class IStaticDataContainer
  {
    public abstract IObject GetObjectByGuid(ulong id);

    public abstract IObject GetOrCreateObject(ulong id);

    public abstract IGameRoot GameRoot { get; }

    public static IStaticDataContainer StaticDataContainer { get; protected set; }
  }
}
