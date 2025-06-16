using PLVirtualMachine.Dynamic.Components;

namespace PLVirtualMachine.Dynamic
{
  public static class VMEntityUtility
  {
    public static VMEntity RegisteredEngineRoot { get; set; }

    public static void ResetRoot()
    {
      RegisteredEngineRoot = null;
      GameComponent.ResetInstance();
      GlobalMarketManager.ResetInstance();
      GlobalStorageManager.ResetInstance();
      GlobalDoorsManager.ResetInstance();
      Support.ResetInstance();
      Weather.ResetInstance();
      Storage.ResetInstance();
    }
  }
}
