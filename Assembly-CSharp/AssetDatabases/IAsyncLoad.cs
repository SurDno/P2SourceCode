namespace AssetDatabases
{
  public interface IAsyncLoad
  {
    bool IsDone { get; }

    object Asset { get; }
  }
}
