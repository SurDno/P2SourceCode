namespace PLVirtualMachine.Common
{
  public interface IEngineRTInstance : IEngineInstanced, IEngineTemplated
  {
    bool IsDisposed { get; }

    bool Instantiated { get; }
  }
}
