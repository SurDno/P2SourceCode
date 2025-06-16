namespace SRDebugger.Services
{
  public interface ISystemInfo
  {
    string Title { get; }

    object Value { get; }

    bool IsPrivate { get; }
  }
}
